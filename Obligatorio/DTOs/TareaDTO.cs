using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class TareaDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "La tarea debe tener un título.")]
    public string Titulo { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una descripción.")]
    public string Descripcion { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una duración en días.")]
    [Range(1, int.MaxValue, ErrorMessage = "La duración debe ser mayor que 0.")]
    public int DuracionEnDias { get; set; }

    [Required(ErrorMessage = "La tarea debe tener una fecha de inicio.")]
    [CustomValidation(typeof(TareaDTO), nameof(ValidarFechaInicio))]
    public DateTime FechaInicioMasTemprana { get; set; }

    public EstadoTareaDTO Estado { get; set; }

    public int Holgura { get; set; }

    public DateTime FechaFinMasTemprana { get; set; }

    public List<UsuarioListarDTO> UsuariosAsignados { get; private set; } = new List<UsuarioListarDTO>();

    public List<RecursoNecesarioDTO> RecursosNecesarios { get; private set; } = new List<RecursoNecesarioDTO>();
    public List<DependenciaDTO> Dependencias { get; private set; } = new List<DependenciaDTO>();

    public Tarea AEntidad()
    {
        Tarea tarea = new Tarea(Titulo, Descripcion, DuracionEnDias, FechaInicioMasTemprana);
        tarea.Id = Id;
        return tarea;
    }

    public static TareaDTO DesdeEntidad(Tarea tarea)
    {
        return new TareaDTO()
        {
            Id = tarea.Id,
            Titulo = tarea.Titulo,
            Descripcion = tarea.Descripcion,
            DuracionEnDias = tarea.DuracionEnDias,
            FechaInicioMasTemprana = tarea.FechaInicioMasTemprana,
            FechaFinMasTemprana = tarea.FechaFinMasTemprana,
            Holgura = tarea.Holgura,
            Estado = (EstadoTareaDTO)tarea.Estado,
            UsuariosAsignados = tarea.UsuariosAsignados.Select(UsuarioListarDTO.DesdeEntidad).ToList(),
            RecursosNecesarios = tarea.RecursosNecesarios.Select(RecursoNecesarioDTO.DesdeEntidad).ToList(),
            Dependencias = tarea.Dependencias.Select(DependenciaDTO.DesdeEntidad).ToList()
        };
    }

    public static ValidationResult ValidarFechaInicio(DateTime fecha, ValidationContext context)
    {
        if (fecha < DateTime.Today)
        {
            return new ValidationResult("La fecha de inicio debe ser la actual o posterior.");
        }

        return ValidationResult.Success;
    }
}