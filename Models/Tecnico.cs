using System.ComponentModel.DataAnnotations;

namespace DigiturnoAML.Models;

public class Tecnico
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string PasswordHash { get; set; } = string.Empty;

    [Required]
    public string NombreCompleto { get; set; } = string.Empty;
}
