using Dominio;
using DTOs;

namespace IServicios.IGestores;

public interface IGestorProyectos
{
    void CrearProyecto(ProyectoDTO proyectoDTO, UsuarioDTO solicitanteDTO);

    void EliminarProyecto(int idProyecto, UsuarioDTO solicitanteDTO);

    List<ProyectoDTO> ObtenerTodos();

    List<Proyecto> ObtenerTodosDominio();

    ProyectoDTO ObtenerProyectoPorId(int id);

    void ModificarNombreDelProyecto(int idProyecto, string nuevoNombre, UsuarioDTO solicitanteDTO);

    void ModificarDescripcionDelProyecto(int idProyecto, string descripcion, UsuarioDTO solicitanteDTO);

    void ModificarFechaDeInicioDelProyecto(int idProyecto, DateTime nuevaFecha, UsuarioDTO solicitanteDTO);

    void CambiarAdministradorDeProyecto(UsuarioDTO solicitanteDTO, int idProyecto, int idNuevoAdmin);

    void AgregarMiembroAProyecto(int idProyecto, UsuarioDTO solicitanteDTO, UsuarioDTO nuevoMiembroDTO);
    
    void EliminarMiembroDelProyecto(int idProyecto, UsuarioDTO solicitanteDTO, int idMiembroAEliminar);
    
    List<ProyectoDTO> ObtenerProyectosPorUsuario(int idUsuario);
    
    Proyecto ObtenerProyectoDelAdministrador(int idAdministrador);
    
    void VerificarUsuarioNoTieneTareasAsignadas(int idProyecto, int idMiembroAEliminar);
    
    void NotificarAdministradoresDeProyectos(List<Proyecto> proyectos, string mensaje);
    
    void CalcularCaminoCritico(ProyectoDTO proyectoDTO);

    bool EsAdministradorDeProyecto(UsuarioDTO usuarioDTO, int idProyecto);
    
    bool EsMiembroDeProyecto(int idUsuario, int idProyecto);
    
    bool EsLiderDeProyecto(UsuarioDTO usuarioDTO, int idProyecto);
    
    Proyecto ObtenerProyectoDominioPorId(int id);

    void AsignarLider(int idProyecto, UsuarioDTO solicitanteDTO, int idNuevoLider);

    void DesasignarLider(int idProyecto, UsuarioDTO solicitanteDTO);

    bool ExisteLiderEnProyecto(int idProyecto);
}