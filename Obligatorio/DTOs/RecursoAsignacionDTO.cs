using System.ComponentModel.DataAnnotations;

namespace DTOs;

public class RecursoAsignacionDTO
{
    [Required(ErrorMessage = "Debe seleccionar un recurso.")]
    public int Id { get; set; }
    
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad { get; set; } = 1;
}