using DigiturnoAML.Data;
using DigiturnoAML.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigiturnoAML.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TicketsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/tickets/cola  — Tickets en espera (público)
    [HttpGet("cola")]
    public async Task<IActionResult> GetCola()
    {
        var cola = await _db.Tickets
            .Where(t => t.Estado == TicketEstado.Esperando || t.Estado == TicketEstado.EnAtencion)
            .OrderBy(t => t.CreadoEn)
            .Select(t => new
            {
                t.Id,
                t.Numero,
                t.Categoria,
                t.NombreEmpleado,
                t.CargoEmpleado,
                Estado = t.Estado.ToString(),
                t.CreadoEn
            })
            .ToListAsync();

        return Ok(cola);
    }

    // POST /api/tickets  — Crear ticket (público)
    [HttpPost]
    public async Task<IActionResult> CrearTicket([FromBody] CreateTicketDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var hoyInicio = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var hoyFin = hoyInicio.AddDays(1);
        var countHoy = await _db.Tickets
            .CountAsync(t => t.CreadoEn >= hoyInicio && t.CreadoEn < hoyFin);

        var numero = $"AML-{(countHoy + 1):D3}";

        var ticket = new Ticket
        {
            Numero = numero,
            Categoria = dto.Categoria,
            NombreEmpleado = dto.NombreEmpleado,
            CargoEmpleado = dto.CargoEmpleado,
            Descripcion = dto.Descripcion,
            Estado = TicketEstado.Esperando,
            CreadoEn = DateTime.UtcNow
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        // Calcular posición en cola
        var posicion = await _db.Tickets
            .CountAsync(t => t.Estado == TicketEstado.Esperando && t.CreadoEn <= ticket.CreadoEn);

        return Ok(new
        {
            ticket.Id,
            ticket.Numero,
            ticket.Categoria,
            ticket.NombreEmpleado,
            ticket.CargoEmpleado,
            Estado = ticket.Estado.ToString(),
            ticket.CreadoEn,
            Posicion = posicion
        });
    }
}
