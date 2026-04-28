using System.ComponentModel.DataAnnotations;

namespace DigiturnoAML.Models;

public class CreateTicketDto
{
    [Required(ErrorMessage = "La categoría es obligatoria.")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del empleado es obligatorio.")]
    [MaxLength(150)]
    public string NombreEmpleado { get; set; } = string.Empty;

    [Required(ErrorMessage = "El cargo es obligatorio.")]
    [MaxLength(100)]
    public string CargoEmpleado { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; }
}

public class LoginDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
