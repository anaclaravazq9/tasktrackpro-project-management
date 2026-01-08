using DTOs;

namespace IServicios.IGestores;

public interface IGestorUsuarios
{
    void CrearYAgregarUsuario(UsuarioDTO solicitanteDTO, UsuarioDTO nuevoUsuarioDTO);
    
    void EliminarUsuario(UsuarioDTO solicitanteDTO, int id);
    
    List<UsuarioListarDTO> ObtenerTodos();
    
    UsuarioDTO ObtenerUsuarioPorId(int idUsuario);
    
    void AgregarAdministradorSistema(UsuarioDTO solicitanteDTO, int idUsuario);
    
    void AsignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario);
    
    void DesasignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario);
    
    void ReiniciarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo);
    
    string AutogenerarContrasenaValida();

    void AutogenerarYAsignarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo);

    void ModificarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo, string nuevaContrasena);
    
    void BorrarNotificacion(int idUsuario, int idNotificacion);
    
    UsuarioDTO LogIn(string email, string contrasena);
    
    List<UsuarioListarDTO> ObtenerUsuariosDiferentes(List<UsuarioListarDTO> usuarios);
    
    void ValidarUsuarioNoEsAdministradorInicial(int idUsuario);
}