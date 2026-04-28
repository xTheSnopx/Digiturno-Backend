using System.ComponentModel.DataAnnotations;

namespace DigiturnoAML.Models;

public enum TicketEstado
{
    Esperando,
    EnAtencion,
    Resuelto,
    Cancelado
}

public class Ticket
{
    public int Id { get; set; }

    [Required]
    public string Numero { get; set; } = string.Empty; // AML-001

    [Required]
    public string Categoria { get; set; } = string.Empty;

    [Required]
    public string NombreEmpleado { get; set; } = string.Empty;

    [Required]
    public string CargoEmpleado { get; set; } = string.Empty;

    public string? Descripcion { get; set; }

    public TicketEstado Estado { get; set; } = TicketEstado.Esperando;

    /// <summary>Técnico que está atendiendo o resolvió el ticket.</summary>
    public string? NombreTecnico { get; set; }

    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? AtendidoEn { get; set; }
    public DateTime? ResueltoEn { get; set; }
}
