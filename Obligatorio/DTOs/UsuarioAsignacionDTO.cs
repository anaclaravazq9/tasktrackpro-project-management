using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class UsuarioAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un usuario.")]
    public int Id { get; set; }
}