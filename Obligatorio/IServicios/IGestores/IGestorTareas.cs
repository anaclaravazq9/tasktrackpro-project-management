using DTOs;

namespace IServicios.IGestores;

public interface IGestorTareas
{
    void AgregarTareaAlProyecto(int idProyecto, UsuarioDTO solicitanteDTO, TareaDTO nuevaTareaDTO);
    
    void EliminarTareaDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idTareaAEliminar);
    
    TareaDTO ObtenerTareaPorId(int idProyecto, int idTarea);
    
    void ModificarTituloTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevoTitulo);
    
    void ModificarDescripcionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, string nuevaDescripcion);
    
    void ModificarDuracionTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, int nuevaDuracion);
    
    void ModificarFechaInicioTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, DateTime nuevaFecha);
    
    void CambiarEstadoTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, EstadoTareaDTO nuevoEstadoDTO);
    
    void AgregarDependenciaATarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto,
        string tipoDependencia);

    void EliminarDependenciaDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idTareaDependencia, int idProyecto);
    
    void AgregarMiembroATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO nuevoMiembroDTO);
    
    void EliminarMiembroDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, UsuarioDTO miembroDTO);
    
    void AsignarRecursoATarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO nuevoRecursoDTO, int cantidad);
    
    void EliminarRecursoDeTarea(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO);
    
    bool EsMiembroDeTarea(UsuarioDTO usuarioDTO, int idTarea, int idProyecto);

    void EncontrarRecursosAlternativosMismoTipo(UsuarioDTO solicitanteDTO, int idProyecto,
        RecursoDTO recursoOriginalDTO, DateTime FechaInicio, DateTime FechaFin, int cantidad);

    void ReprogramarTarea(UsuarioDTO solicitanteDTO, int idProyecto, int idTarea, RecursoDTO recursoDTO, int cantidad);

    void ForzarAsignacion(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto, RecursoDTO recursoDTO,
        int cantidad);

    public void ValidarYAsignarRecurso(UsuarioDTO solicitanteDTO, int idTarea, int idProyecto,
        RecursoDTO nuevoRecursoDTO, int cantidad);
    
}