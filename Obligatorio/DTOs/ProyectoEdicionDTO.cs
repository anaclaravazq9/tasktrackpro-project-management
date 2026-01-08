using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class ProyectoEdicionDTO
{
    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(400, ErrorMessage = "La descripción no puede tener más de 400 caracteres")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [CustomValidation(typeof(ProyectoEdicionDTO), nameof(ValidarFechaInicio))]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    public static ValidationResult ValidarFechaInicio(DateTime fecha, ValidationContext context)
    {
        if (fecha < DateTime.Today)
        {
            return new ValidationResult("La fecha de inicio debe ser la actual o posterior.");
        }

        return ValidationResult.Success;
    }

    public static ProyectoEdicionDTO DesdeEntidad(Proyecto proyecto)
    {
        return new ProyectoEdicionDTO()
        {
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            FechaInicio = proyecto.FechaInicio
        };
    }
}