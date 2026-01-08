using Dominio;
using Excepciones;
using Excepciones.MensajesError;

namespace Utilidades;

public static class PermisosUsuarios
{
    public static void VerificarPermisoAdminSistema(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioNoAdminSistema(accion));
        }
    }

    public static void VerificarUsuarioEsAdminProyectoDeEseProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (!proyecto.EsAdministrador(usuario))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.NoEsAdminDelProyecto);
        }
    }

    public static void VerificarUsuarioMiembroDelProyecto(int idUsuario, Proyecto proyecto)
    {
        Usuario usuario = ObtenerMiembro(idUsuario, proyecto);

        if (usuario is null)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioNoMiembroDelProyecto);
        }
    }
    
    public static void VerificarUsuarioNoAdministraOtroProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioAdministrandoProyecto);
        }
    }

    public static void VerificarUsuarioTengaPermisosDeAdminProyecto(Usuario solicitante, string tipoUsuario)
    {
        if (!solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoDenegadoPorTipo(tipoUsuario));
        }
    }
    
    public static void VerificarUsuarioEsAdminOLiderDelProyecto(Proyecto proyecto, Usuario usuario)
    {
        if (proyecto.Administrador.Id != usuario.Id && (proyecto.Lider == null || proyecto.Lider.Id != usuario.Id))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioNoEsAdminNiLider);
        }
    }

    public static void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(Usuario usuario)
    {
        if (usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioAdministrandoProyecto);
        }
    }

    public static void VerificarUsuarioPuedaReiniciarOModificarContrasena(Usuario solicitante, Usuario usuario,
        string accion)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto && !solicitante.Equals(usuario))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoDenegadoPara($"{accion}"));
        }
    }

    public static void VerificarPermisoAdminSistemaOAdminProyecto(Usuario usuario, string accion)
    {
        if (!usuario.EsAdministradorSistema && !usuario.EstaAdministrandoUnProyecto)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoDenegadoPara(accion));
        }
    }

    public static void VerificarSolicitantePuedaAutogenerarContrasena(Usuario solicitante)
    {
        if (!solicitante.EsAdministradorSistema && !solicitante.EsAdministradorProyecto)
        {
            throw new ExcepcionPermisos(
                MensajesErrorServicios.PermisoDenegadoPara("autogenerar la contraseña del usuario"));
        }
    }

    public static void VerificarUsuarioNoEsMiembroDeProyecto(Usuario usuario)
    {
        if (usuario.CantidadProyectosAsignados > 0)
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioMiembroDeProyecto);
        }
    }

    public static void VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(Usuario usuario, Tarea tarea, Proyecto proyecto)
    {
        if (!tarea.EsMiembro(usuario) && proyecto.Administrador.Id != usuario.Id && (proyecto.Lider == null || proyecto.Lider.Id != usuario.Id))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.PermisoParaCambiarDeEstado);
        }
    }

    public static void VerificarUsuarioEsAdminDeEseProyectoOAdminSistema(Usuario usuario, Proyecto proyecto)
    {
        if (!(proyecto.EsAdministrador(usuario) || usuario.EsAdministradorSistema))
        {
            throw new ExcepcionPermisos(MensajesErrorServicios.UsuarioNoAdministraProyectoOEsAdminSistema);
        }
    }

    private static Usuario ObtenerMiembro(int idMiembro, Proyecto proyecto)
    {
        Usuario miembro = proyecto.Miembros.FirstOrDefault(usuario => usuario.Id == idMiembro);
        return miembro;
    }

}