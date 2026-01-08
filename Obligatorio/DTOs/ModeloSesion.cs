using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class ModeloSesion
{
    [EmailAddress] public string Email { get; set; } = string.Empty;
    public string Contraseña { get; set; } = string.Empty;
}