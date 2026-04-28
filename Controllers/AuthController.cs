using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using DigiturnoAML.Data;
using DigiturnoAML.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace DigiturnoAML.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;

    public AuthController(AppDbContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // POST /api/auth/login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var tecnico = await _db.Tecnicos
            .FirstOrDefaultAsync(t => t.Username == dto.Username);

        if (tecnico == null || !BCrypt.Net.BCrypt.Verify(dto.Password, tecnico.PasswordHash))
            return Unauthorized(new { message = "Credenciales inválidas." });

        var token = GenerarJwt(tecnico);

        return Ok(new
        {
            token,
            tecnico.NombreCompleto,
            tecnico.Username
        });
    }

    [HttpGet("reset")]
    public async Task<IActionResult> Reset()
    {
        var admin = await _db.Tecnicos.FirstOrDefaultAsync(t => t.Username == "admin");
        if (admin != null)
        {
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin");
            await _db.SaveChangesAsync();
            return Ok("Password reseteada a 'admin'");
        }
        return NotFound();
    }

    private string GenerarJwt(Tecnico tecnico)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = double.Parse(_config["Jwt:ExpirationHours"]!);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, tecnico.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, tecnico.Username),
            new Claim("nombre", tecnico.NombreCompleto),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(expiration),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
