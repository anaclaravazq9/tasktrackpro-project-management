using Dominio;
using DTOs;
using Excepciones;
using Excepciones.MensajesError;
using IRepositorios;
using IServicios;
using IServicios.IGestores;
using Servicios.Notificaciones;
using Utilidades;

namespace Servicios.Gestores;

public class GestorUsuarios : IGestorUsuarios
{
    private string _contrasenaPorDefecto = "TaskTrackPro@2025";
    public Usuario AdministradorInicial { get; private set; }
    private IRepositorioUsuarios _usuarios;
    private readonly INotificador _notificador;

    public GestorUsuarios(IRepositorioUsuarios repositorioUsuarios, INotificador notificador)
    {
        _usuarios = repositorioUsuarios;
        _notificador = notificador;
        AdministradorInicial = _usuarios.ObtenerPorId(1); // Por diseño, al crear la BDD el usuario con ID 1 es el administrador inicial.
    }

    public void CrearYAgregarUsuario(UsuarioDTO solicitanteDTO, UsuarioDTO nuevoUsuarioDTO)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario nuevoUsuario = CrearUsuario(nuevoUsuarioDTO);
        VerificarEmailNoRepetido(nuevoUsuario.Email);
        AgregarUsuario(solicitante, nuevoUsuario);
        nuevoUsuarioDTO.Id = nuevoUsuario.Id;
    }

    public void EliminarUsuario(UsuarioDTO solicitanteDTO, int id)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        ValidarUsuarioNoEsAdministradorInicial(id);
        Usuario usuario = obtenerUsuarioDominioPorId(id);
        if (!solicitante.EsAdministradorSistema && !solicitante.Equals(usuario))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoDenegado);
        }

        PermisosUsuarios.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        _usuarios.Eliminar(usuario.Id);
        string mensajeNotificacion = MensajesNotificacion.UsuarioEliminado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    public List<UsuarioListarDTO> ObtenerTodos()
    {
        return _usuarios.ObtenerTodos().Select(UsuarioListarDTO.DesdeEntidad).ToList();
    }

    public UsuarioDTO ObtenerUsuarioPorId(int idUsuario)
    {
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        return UsuarioDTO.DesdeEntidad(usuario);
    }

    public void AgregarAdministradorSistema(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "asignar un administrador de sistema");
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        usuario.EsAdministradorSistema = true;
        _usuarios.Actualizar(usuario);
    }

    public void AsignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "asignar administradores de proyecto");
        Usuario nuevoAdministradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        nuevoAdministradorProyecto.EsAdministradorProyecto = true;
        _usuarios.Actualizar(nuevoAdministradorProyecto);
    }

    public void DesasignarAdministradorProyecto(UsuarioDTO solicitanteDTO, int idUsuario)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "desasignar administradores de proyecto");
        Usuario administradorProyecto = obtenerUsuarioDominioPorId(idUsuario);
        PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(administradorProyecto, "solicitante");
        PermisosUsuarios.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(administradorProyecto);
        administradorProyecto.EsAdministradorProyecto = false;
        _usuarios.Actualizar(administradorProyecto);
    }

    public void ReiniciarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo,
            "reiniciar la contraseña del usuario");

        string contrasenaPorDefectoEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(_contrasenaPorDefecto);
        usuarioObjetivo.EstablecerContrasenaEncriptada(contrasenaPorDefectoEncriptada);
        _usuarios.Actualizar(usuarioObjetivo);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaReiniciada(_contrasenaPorDefecto));
    }

    public string AutogenerarContrasenaValida() // solo genera, permite mostrar en pantalla la contraseña
    {
        return UtilidadesContrasena.AutogenerarContrasenaValida();
    }

    public void AutogenerarYAsignarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo) // genera y asigna. Prevee el caso de notificar por mail y no tener que mostrar contraseña en pantalla
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        PermisosUsuarios.VerificarSolicitantePuedaAutogenerarContrasena(solicitante);
        string nuevaContrasena = AutogenerarContrasenaValida();
        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);

        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        _usuarios.Actualizar(usuarioObjetivo);
        
        Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaModificada(nuevaContrasena));
    }

    public void ModificarContrasena(UsuarioDTO solicitanteDTO, int idUsuarioObjetivo, string nuevaContrasena)
    {
        Usuario solicitante = obtenerUsuarioDominioPorId(solicitanteDTO.Id);
        Usuario usuarioObjetivo = obtenerUsuarioDominioPorId(idUsuarioObjetivo);
        PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(solicitante, usuarioObjetivo,
            "modificar la contraseña del usuario");

        string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
        usuarioObjetivo.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
        _usuarios.Actualizar(usuarioObjetivo);
        
        NotificarUsuarioModificacionSiNoEsElMismo(solicitante, usuarioObjetivo, nuevaContrasena);
    }

    public void BorrarNotificacion(int idUsuario, int idNotificacion)
    {
        Usuario usuario = obtenerUsuarioDominioPorId(idUsuario);
        usuario.BorrarNotificacion(idNotificacion);
        _usuarios.Actualizar(usuario);
    }

    public UsuarioDTO LogIn(string email, string contrasena)
    {
        Usuario usuario = _usuarios.ObtenerUsuarioPorEmail(email);
        VerificarUsuarioRegistrado(usuario);
        VerificarContrasenaCorrecta(usuario, contrasena);
        return UsuarioDTO.DesdeEntidad(usuario);
    }

    public List<UsuarioListarDTO> ObtenerUsuariosDiferentes(List<UsuarioListarDTO> usuarios)
    {
        List<int> idsAExcluir = usuarios.Select(u => u.Id).ToList();
        return ObtenerTodos().Where(u => !idsAExcluir.Contains(u.Id)).ToList();
    }

    public void ValidarUsuarioNoEsAdministradorInicial(int idUsuario)
    {
        if (idUsuario == AdministradorInicial.Id)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PrimerAdminSistema);
        }
    }

    private Usuario CrearUsuario(UsuarioDTO nuevoUsuarioDTO)
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevoUsuarioDTO.Contrasena);
        return new Usuario(nuevoUsuarioDTO.Nombre, nuevoUsuarioDTO.Apellido, nuevoUsuarioDTO.FechaNacimiento,
            nuevoUsuarioDTO.Email, contrasenaEncriptada);
    }

    private void AgregarUsuario(Usuario solicitante, Usuario usuario)
    {
        PermisosUsuarios.VerificarPermisoAdminSistema(solicitante, "crear usuarios");
        _usuarios.Agregar(usuario);
        string mensajeNotificacion = MensajesNotificacion.UsuarioCreado(usuario.Nombre, usuario.Apellido);
        NotificarAdministradoresSistema(solicitante, mensajeNotificacion);
    }

    private Usuario obtenerUsuarioDominioPorId(int idUsuario)
    {
        Usuario usuario = _usuarios.ObtenerPorId(idUsuario);
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.UsuarioNoEncontrado);
        }

        return usuario;
    }
    
    private void VerificarUsuarioRegistrado(Usuario usuario)
    {
        if (usuario == null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.UsuarioNoEncontrado);
        }
    }

    private void VerificarContrasenaCorrecta(Usuario usuario, string contrasena)
    {
        if (!usuario.Autenticar(contrasena))
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.ContrasenaIncorrecta);
        }
    }
    
    private void VerificarEmailNoRepetido(string nuevoEmail)
    {
        if (_usuarios.ObtenerUsuarioPorEmail(nuevoEmail) != null)
        {
            throw new ExcepcionUsuario(MensajesErrorServicios.EmailRepetido);
        }
    }

    private void NotificarUsuarioModificacionSiNoEsElMismo(Usuario solicitante, Usuario usuarioObjetivo,
        string nuevaContrasena)
    {
        if (!solicitante.Equals(usuarioObjetivo))
        {
            Notificar(usuarioObjetivo, MensajesNotificacion.ContrasenaModificada(nuevaContrasena));
        }
    }

    private void NotificarAdministradoresSistema(Usuario solicitante, string mensajeNotificacion)
    {
        List<Usuario> administradores = _usuarios.ObtenerTodos()
            .Where(usuario => usuario.EsAdministradorSistema && !usuario.Equals(solicitante)).ToList();
        administradores.ForEach(admin => Notificar(admin, mensajeNotificacion));
    }

    private void Notificar(Usuario usuario, string mensajeNotificacion)
    {
        _notificador.NotificarUno(usuario, mensajeNotificacion);
    }
}