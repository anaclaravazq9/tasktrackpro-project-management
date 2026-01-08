using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class ProyectoDTO

{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria")]
    [StringLength(400, ErrorMessage = "La descripción no puede tener más de 400 caracteres")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La fecha de inicio es obligatoria")]
    [CustomValidation(typeof(ProyectoDTO), nameof(ValidarFechaInicio))]
    public DateTime FechaInicio { get; set; } = DateTime.Today;

    public DateTime FechaFinMasTemprana { get; set; }

    public List<TareaDTO> Tareas { get; private set; } = new List<TareaDTO>();

    public UsuarioDTO Administrador { get; set; }

    public List<UsuarioListarDTO> Miembros { get; set; }

    public static ValidationResult ValidarFechaInicio(DateTime fecha, ValidationContext context)
    {
        if (fecha < DateTime.Today)
        {
            return new ValidationResult("La fecha de inicio debe ser la actual o posterior.");
        }

        return ValidationResult.Success;
    }

    public Proyecto AEntidad(Usuario administrador)
    {
        Proyecto proyecto = new Proyecto(Nombre, Descripcion, FechaInicio, administrador, new List<Usuario>());
        proyecto.Id = Id;
        return proyecto;
    }

    public static ProyectoDTO DesdeEntidad(Proyecto proyecto)
    {
        return new ProyectoDTO()
        {
            Id = proyecto.Id,
            Nombre = proyecto.Nombre,
            Descripcion = proyecto.Descripcion,
            FechaInicio = proyecto.FechaInicio,
            Tareas = proyecto.Tareas.Select(TareaDTO.DesdeEntidad).ToList(),
            FechaFinMasTemprana = proyecto.FechaFinMasTemprana,
            Administrador = UsuarioDTO.DesdeEntidad(proyecto.Administrador),
            Miembros = proyecto.Miembros.Select(UsuarioListarDTO.DesdeEntidad).ToList()
        };
    }
}