using DigiturnoAML.Data;
using DigiturnoAML.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigiturnoAML.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;

    public AdminController(AppDbContext db)
    {
        _db = db;
    }

    // GET /api/admin/tickets  — Todos los tickets del día (excluye cancelados del pasado)
    [HttpGet("tickets")]
    public async Task<IActionResult> GetTicketsHoy()
    {
        var hoyInicio = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var hoyFin = hoyInicio.AddDays(1);
        var tickets = await _db.Tickets
            .Where(t => t.CreadoEn >= hoyInicio && t.CreadoEn < hoyFin)
            .OrderBy(t => t.CreadoEn)
            .Select(t => new
            {
                t.Id,
                t.Numero,
                t.Categoria,
                t.NombreEmpleado,
                t.CargoEmpleado,
                t.Descripcion,
                Estado = t.Estado.ToString(),
                t.NombreTecnico,
                t.CreadoEn,
                t.AtendidoEn,
                t.ResueltoEn
            })
            .ToListAsync();

        return Ok(tickets);
    }

    // PUT /api/admin/tickets/{id}/atender
    [HttpPut("tickets/{id}/atender")]
    public async Task<IActionResult> AtenderTicket(int id, [FromBody] UpdateTicketDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        if (ticket.Estado != TicketEstado.Esperando)
            return BadRequest(new { message = "El ticket no está en espera." });

        ticket.Estado = TicketEstado.EnAtencion;
        ticket.AtendidoEn = DateTime.UtcNow;
        ticket.NombreTecnico = dto.NombreTecnico;
        await _db.SaveChangesAsync();

        return Ok(new { message = "Ticket en atención.", ticket.Numero, ticket.NombreTecnico });
    }

    // PUT /api/admin/tickets/{id}/resolver
    [HttpPut("tickets/{id}/resolver")]
    public async Task<IActionResult> ResolverTicket(int id, [FromBody] UpdateTicketDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null) return NotFound();

        if (ticket.Estado == TicketEstado.Resuelto)
            return BadRequest(new { message = "El ticket ya está resuelto." });

        ticket.Estado = TicketEstado.Resuelto;
        ticket.ResueltoEn = DateTime.UtcNow;
        // Solo sobreescribir si resuelve sin haber atendido antes
        if (string.IsNullOrEmpty(ticket.NombreTecnico))
            ticket.NombreTecnico = dto.NombreTecnico;

        await _db.SaveChangesAsync();

        return Ok(new { message = "Ticket resuelto.", ticket.Numero, ticket.NombreTecnico });
    }

    // GET /api/admin/stats  — Resumen del día
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        var hoyInicio = DateTime.SpecifyKind(DateTime.UtcNow.Date, DateTimeKind.Utc);
        var hoyFin = hoyInicio.AddDays(1);
        var tickets = await _db.Tickets
            .Where(t => t.CreadoEn >= hoyInicio && t.CreadoEn < hoyFin)
            .ToListAsync();

        return Ok(new
        {
            Total     = tickets.Count,
            Esperando = tickets.Count(t => t.Estado == TicketEstado.Esperando),
            EnAtencion = tickets.Count(t => t.Estado == TicketEstado.EnAtencion),
            Resueltos = tickets.Count(t => t.Estado == TicketEstado.Resuelto)
        });
    }
}
