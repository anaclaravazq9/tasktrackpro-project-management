using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class RecursoDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El tipo es obligatorio.")]
    public string Tipo { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    public string Descripcion { get; set; }
    public int? IdProyectoAsociado { get; set; }
    public int CantidadDeTareasUsandolo { get; set; } = 0;
    
    public int Capacidad { get; set; }

    public Recurso AEntidad()
    {
        Recurso recurso = new Recurso(Nombre, Tipo, Descripcion, Capacidad);
        recurso.Id = Id;
        for (int i = 0; i < CantidadDeTareasUsandolo; i++)
        {
            recurso.IncrementarCantidadDeTareasUsandolo();
        }

        return recurso;
    }

    public static RecursoDTO DesdeEntidad(Recurso recurso)
    {
        return new RecursoDTO()
        {
            Id = recurso.Id,
            Nombre = recurso.Nombre,
            Tipo = recurso.Tipo,
            Descripcion = recurso.Descripcion,
            Capacidad = recurso.Capacidad,
            IdProyectoAsociado = recurso.ProyectoAsociado?.Id
        };
    }
}