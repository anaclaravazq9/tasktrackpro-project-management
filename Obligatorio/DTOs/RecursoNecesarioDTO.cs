using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class RecursoNecesarioDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La cantidad de recurso es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor que cero.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "El recurso es obligatorio.")]
    public RecursoDTO Recurso { get; set; }

    public static RecursoNecesarioDTO DesdeEntidad(RecursoNecesario rn)
    {
        return new RecursoNecesarioDTO
        {
            Id = rn.Id,
            Cantidad = rn.Cantidad,
            Recurso = RecursoDTO.DesdeEntidad(rn.Recurso)
        };
    }

    public RecursoNecesario AEntidad()
    {
        return new RecursoNecesario(Recurso.AEntidad(), Cantidad)
        {
            Id = this.Id
        };
    }
}