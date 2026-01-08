using Dominio;
using Excepciones;
using Utilidades;

namespace Tests.UtilidadesTests
{
    [TestClass]
    public class PermisosUsuariosTests
    {
        private Usuario adminSistema;
        private Usuario adminProyecto;
        private Usuario usuarioComun;
        private Proyecto proyecto;

        [TestInitialize]
        public void Setup()
        {
            adminSistema = new Usuario { Id = 1, EsAdministradorSistema = true };
            adminProyecto = new Usuario { Id = 2, EsAdministradorProyecto = true };
            usuarioComun = new Usuario { Id = 3 };

            proyecto = new Proyecto("nombre", "descrip", DateTime.Now, adminProyecto,
                new List<Usuario> { adminProyecto, usuarioComun });
        }

        [TestMethod]
        public void VerificarPermisoAdminSistema_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarPermisoAdminSistema(adminSistema, "realizar una acción");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarPermisoAdminSistema_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarPermisoAdminSistema(usuarioComun, "hacer algo");
        }

        [TestMethod]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, adminProyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioEsAdminProyectoDeEseProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioEsAdminProyectoDeEseProyecto(proyecto, usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioMiembroDelProyecto_NoLanzaExcepcion_SiEsMiembro()
        {
            PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(usuarioComun.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioMiembroDelProyecto_LanzaExcepcion_SiNoEsMiembro()
        {
            var otroUsuario = new Usuario { Id = 999 };
            PermisosUsuarios.VerificarUsuarioMiembroDelProyecto(otroUsuario.Id, proyecto);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoAdministraOtroProyecto_LanzaExcepcion_SiAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioNoAdministraOtroProyecto_NoLanzaExcepcion_SiNoAdministra()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuarios.VerificarUsuarioNoAdministraOtroProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_LanzaExcepcion_SiNoEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(usuarioComun, "usuario común");
        }

        [TestMethod]
        public void VerificarUsuarioTengaPermisosDeAdminProyecto_NoLanzaExcepcion_SiEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioTengaPermisosDeAdminProyecto(adminProyecto, "admin");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_LanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = true };
            PermisosUsuarios.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto_NoLanzaExcepcion()
        {
            var usuario = new Usuario { EstaAdministrandoUnProyecto = false };
            PermisosUsuarios.VerificarUsuarioADesasignarNoEsteAdmistrandoUnProyecto(usuario);
        }

        [TestMethod]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_NoLanzaExcepcion_SiSolicitanteEsAdmin()
        {
            PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(adminSistema, usuarioComun,
                "modificar contraseña");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioPuedaReiniciarOModificarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuarios.VerificarUsuarioPuedaReiniciarOModificarContrasena(usuarioComun, adminSistema,
                "reiniciar contraseña");
        }

        [TestMethod]
        public void VerificarPermisoAdminSistemaOAdminProyecto_NoLanzaExcepcion_SiEsAdminSistema()
        {
            PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(adminSistema, "acc");
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarPermisoAdminSistemaOAdminProyecto_LanzaExcepcion_SiNoEsNinguno()
        {
            PermisosUsuarios.VerificarPermisoAdminSistemaOAdminProyecto(usuarioComun, "acc");
        }

        [TestMethod]
        public void VerificarSolicitantePuedaAutogenerarContrasena_NoLanzaExcepcion_SiTienePermisos()
        {
            PermisosUsuarios.VerificarSolicitantePuedaAutogenerarContrasena(adminSistema);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarSolicitantePuedaAutogenerarContrasena_LanzaExcepcion_SiNoTienePermisos()
        {
            PermisosUsuarios.VerificarSolicitantePuedaAutogenerarContrasena(usuarioComun);
        }

        [TestMethod]
        public void VerificarUsuarioNoEsMiembroDeProyecto_NoLanzaExcepcion_SiNoTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 0 };
            PermisosUsuarios.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }

        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioNoEsMiembroDeProyecto_LanzaExcepcion_SiTieneProyectos()
        {
            var usuario = new Usuario { CantidadProyectosAsignados = 2 };
            PermisosUsuarios.VerificarUsuarioNoEsMiembroDeProyecto(usuario);
        }
        
        [TestMethod]
        public void VerificarUsuarioEsAdminOLiderDelProyecto_NoLanzaExcepcion_SiEsLider()
        {
            var lider = new Usuario { Id = 4 };
            proyecto.Miembros.Add(lider);
            proyecto.AsignarLider(lider);

            PermisosUsuarios.VerificarUsuarioEsAdminOLiderDelProyecto(proyecto, lider);
        }
        
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioEsAdminOLiderDelProyecto_LanzaExcepcionSiNoEsAdminNiLider()
        {
            var usuario = new Usuario { Id = 5 };
            proyecto.Miembros.Add(usuario);

            PermisosUsuarios.VerificarUsuarioEsAdminOLiderDelProyecto(proyecto, usuario);
        }
        
        [TestMethod]
        public void VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLider_NoLanzaSiUsuarioEsMiembroDeTarea()
        {
            Usuario usuario = new Usuario { Id = 10 };
            Tarea tarea = new Tarea("Tarea", "Desc", 3, DateTime.Today.AddDays(1));
            tarea.AsignarUsuario(usuario);
    
            Proyecto proyecto = new Proyecto("nombre", "desc", DateTime.Now, new Usuario { Id = 1 }, new List<Usuario> { usuario });

            PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(usuario, tarea, proyecto);
        }
        
        [TestMethod]
        public void VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLider_NoLanza_SiUsuarioEsAdminDelProyecto()
        {
            Usuario usuario = new Usuario { Id = 20 };
            Tarea tarea = new Tarea("Tarea", "Desc", 3, DateTime.Today.AddDays(1));

            Proyecto proyecto = new Proyecto("nombre", "desc", DateTime.Now, usuario, new List<Usuario> { usuario });

            PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(usuario, tarea, proyecto);
        }
        
        [TestMethod]
        public void VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLider_NoLanza_SiUsuarioEsLiderDelProyecto()
        {
            Usuario usuario = new Usuario { Id = 30 };
            Tarea tarea = new Tarea("Tarea", "Desc", 3, DateTime.Today.AddDays(1));

            Usuario admin = new Usuario { Id = 99 }; // otro admin
            Proyecto proyecto = new Proyecto("nombre", "desc", DateTime.Now, admin, new List<Usuario> { usuario, admin });
            proyecto.AsignarLider(usuario);

            PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(usuario, tarea, proyecto);
        }
        
        [TestMethod]
        [ExpectedException(typeof(ExcepcionPermisos))]
        public void VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLider_Lanza_SiNoTienePermisos()
        {
            Usuario usuario = new Usuario { Id = 40 };
            Tarea tarea = new Tarea("Tarea", "Desc",  3, DateTime.Today.AddDays(1));

            Usuario admin = new Usuario { Id = 99 };
            Proyecto proyecto = new Proyecto("nombre", "desc", DateTime.Now, admin, new List<Usuario> { admin });

            PermisosUsuarios.VerificarUsuarioTengaLaTareaAsignadaOSeaAdminOLiderDelProyecto(usuario, tarea, proyecto);
        }
    }
}