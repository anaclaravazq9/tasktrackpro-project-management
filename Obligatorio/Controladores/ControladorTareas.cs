using DTOs;
using IServicios.IGestores;

namespace Controladores;

public class ControladorTareas
{
    private IGestorTareas _gestorTareas;

    public ControladorTareas(IGestorTareas gestorTareas)
    {
        _gestorTareas = gestorTareas;
    }

    public void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO)
    {
        _gestorTareas.AgregarTareaAlProyecto(idProyecto, solicitanteDTO, nuevaTareaDTO);
    }

    public void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar)
    {
        _gestorTareas.EliminarTareaDelProyecto(idProyecto, solicitanteDTO, idTareaAEliminar);
    }

    public bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto)
    {
        return _gestorTareas.EsMiembroDeTarea(usuarioDTO, idTarea, idProyecto);
    }

    public void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo)
    {
        _gestorTareas.ModificarTituloTarea(solicitanteDTO, idTarea, idProyecto, nuevoTitulo);
    }

    public void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        string nuevaDescripcion)
    {
        _gestorTareas.ModificarDescripcionTarea(solicitanteDTO, idTarea, idProyecto, nuevaDescripcion);
    }

    public void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion)
    {
        _gestorTareas.ModificarDuracionTarea(solicitanteDTO, idTarea, idProyecto, nuevaDuracion);
    }

    public void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha)
    {
        _gestorTareas.ModificarFechaInicioTarea(solicitanteDTO, idTarea, idProyecto, nuevaFecha);
    }

    public void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        EstadoTareaDTO nuevoEstadoDTO)
    {
        _gestorTareas.CambiarEstadoTarea(solicitanteDTO, idTarea, idProyecto, nuevoEstadoDTO);
    }

    public void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia)
    {
        _gestorTareas.AgregarDependenciaATarea(solicitanteDTO, idTarea, idTareaDependencia, idProyecto,
            tipoDependencia);
    }

    public void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia,
        int idProyecto)
    {
        _gestorTareas.EliminarDependenciaDeTarea(solicitanteDTO, idTarea, idTareaDependencia, idProyecto);
    }

    public void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO)
    {
        _gestorTareas.AgregarMiembroATarea(solicitanteDTO, idTarea, idProyecto, nuevoMiembroDTO);
    }

    public void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO)
    {
        _gestorTareas.EliminarMiembroDeTarea(solicitanteDTO, idTarea, idProyecto, miembroDTO);
    }

    public void ValidarYAsignarRecurso(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO, int cantidad)
    {
        _gestorTareas.ValidarYAsignarRecurso(solicitanteDTO, idTarea, idProyecto, nuevoRecursoDTO, cantidad);
    }

    public void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO)
    {
        _gestorTareas.EliminarRecursoDeTarea(solicitanteDTO, idTarea, idProyecto, recursoDTO);
    }

    public TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea)
    {
        return _gestorTareas.ObtenerTareaPorId(idProyecto, idTarea);
    }

    public void EncontrarRecursosAlternativosMismoTipo(UsuarioDTO solicitanteDTO, int idProyecto,
        RecursoDTO recursoOriginalDTO, DateTime FechaInicio, DateTime FechaFin, int cantidad)
    {
        _gestorTareas.EncontrarRecursosAlternativosMismoTipo(solicitanteDTO, idProyecto, recursoOriginalDTO, FechaInicio, FechaFin, cantidad);
    }

    public void ReprogramarTarea(UsuarioDTO solicitanteDTO, int idProyecto, int idTarea, RecursoDTO recursoDTO,
        int cantidad)
    {
        _gestorTareas.ReprogramarTarea(solicitanteDTO, idProyecto, idTarea, recursoDTO, cantidad);
    }

    public void ForzarAsignacion(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO,
        int cantidad)
    {
        _gestorTareas.ForzarAsignacion(solicitanteDTO, idTarea, idProyecto, recursoDTO, cantidad);
    }
}