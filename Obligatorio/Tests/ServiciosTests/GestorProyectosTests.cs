using Dominio;
using DTOs;
using Repositorios;
using Servicios.CaminoCritico;
using Excepciones;
using Servicios.Gestores;
using Servicios.Notificaciones;
using Utilidades;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorProyectosTests
{
    private Notificador _notificador;
    private CaminoCritico _caminoCritico;
    
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    private RepositorioUsuarios _repositorioUsuarios;
    private RepositorioProyectos _repositorioProyectos;
    private RepositorioRecursos _repositorioRecursos;
    
    private GestorProyectos _gestor;
    
    private Usuario _admin;
    private Usuario _usuarioNoAdmin;
    private UsuarioDTO _adminDTO;
    private UsuarioDTO _adminSistema;
    private ProyectoDTO _proyecto;

    [TestInitialize]
    public void Inicializar()
    {
        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _repositorioProyectos = new RepositorioProyectos(_contexto);
        _repositorioRecursos = new RepositorioRecursos(_contexto);

        _notificador = new Notificador(_repositorioUsuarios);
        _caminoCritico = new CaminoCritico(_notificador);
        
        _gestor = new GestorProyectos(_repositorioUsuarios, _repositorioProyectos, _notificador, _caminoCritico);
        
        _admin = CrearAdminProyecto();
        _adminDTO = UsuarioDTO.DesdeEntidad(_admin);
        _adminSistema = CrearAdminSistema();
        _usuarioNoAdmin = CrearMiembro();
        _proyecto = CrearProyectoCon(_admin);
    }

    private Usuario CrearAdminProyecto()
    {
        //simulación de gestorUsuarios 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorProyecto = true;
        _repositorioUsuarios.Agregar(admin);
        return admin;
    }

    private UsuarioDTO CrearAdminSistema()
    {
        //simulación de gestorUsuarios 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        _repositorioUsuarios.Agregar(admin);
        return UsuarioDTO.DesdeEntidad(admin);
    }

    private Usuario CrearMiembro()
    {
        //simulación de gestorUsuarios 
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario miembro = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        _repositorioUsuarios.Agregar(miembro);
        return miembro;
    }

    private ProyectoDTO CrearProyectoCon(Usuario admin)
    {
        return new ProyectoDTO()
        {
            Nombre = "Proyecto",
            Descripcion = "Descripcion",
            FechaInicio = DateTime.Today.AddDays(1),
            Administrador = UsuarioDTO.DesdeEntidad(admin),
        };
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void CrearProyecto_AsignarIdCorrectamente()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);

        Assert.AreEqual(1, proyecto.Id);
        Assert.AreEqual(proyecto.Id, _gestor.ObtenerTodosDominio().ElementAt(0).Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CrearProyecto_LanzaExcepcionSiUsuarioNoTienePermisosDeAdminProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_usuarioNoAdmin);

        _gestor.CrearProyecto(proyecto, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CrearProyecto_LanzaExcepcionSiSolicitanteYaAdministraOtroProyecto()
    {
        _admin.EstaAdministrandoUnProyecto = true;

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
    }

    [TestMethod]
    public void CrearProyecto_EstableceEstaAdministrandoProyectoEnTrue()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);

        Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void CrearProyecto_LanzaExcepcionSiNombreYaExiste()
    {
        Usuario otroAdmin = CrearAdminProyecto();
        ProyectoDTO proyecto1 = CrearProyectoCon(_admin);
        ProyectoDTO proyecto2 = CrearProyectoCon(otroAdmin);

        _gestor.CrearProyecto(proyecto1, _adminDTO);
        _gestor.CrearProyecto(proyecto2, UsuarioDTO.DesdeEntidad(otroAdmin));
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void CrearProyecto_LanzaExcepcionSiSolicitanteNoExiste()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        UsuarioDTO usuarioInexistente = new UsuarioDTO() { Id = 9999, Nombre = "Inexistente" };

        _gestor.CrearProyecto(proyecto, usuarioInexistente);
    }

    [TestMethod]
    public void CrearProyecto_NotificaAMiembrosDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);

        string mensajeEsperado = MensajesNotificacion.ProyectoCreado(proyecto.Nombre);
        Assert.AreEqual(mensajeEsperado, _admin.Notificaciones.First().Mensaje);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarProyecto_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.EliminarProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void EliminarProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        _gestor.EliminarProyecto(1000, _adminDTO);
    }

    [TestMethod]
    public void EliminarProyecto_EliminaDeListaAlProyecto()
    {
        _gestor.CrearProyecto(_proyecto, _adminDTO);
        
        int idProyectoCreado = _gestor.ObtenerTodosDominio().First().Id;

        Assert.AreEqual(1, _gestor.ObtenerTodosDominio().Count);

        _gestor.EliminarProyecto(idProyectoCreado, _adminDTO);

        Assert.AreEqual(0, _gestor.ObtenerTodosDominio().Count);
    }
    
    [TestMethod]
    public void EliminarProyecto_EliminaDeListaAProyectoConTareasYDependencias()
    {
        GestorTareas gestorTareas = new GestorTareas(_repositorioProyectos, _repositorioUsuarios, _repositorioRecursos, _notificador, _caminoCritico);
        TareaDTO tarea1 = new TareaDTO()
        {
            Titulo = "Tarea 1", Descripcion = "Descripcion", DuracionEnDias = 2,
            FechaInicioMasTemprana = DateTime.Today.AddDays(5)
        };
        TareaDTO tarea2 = new TareaDTO()
        {
            Titulo = "Tarea 2", Descripcion = "Descripcion", DuracionEnDias = 2,
            FechaInicioMasTemprana = DateTime.Today.AddDays(5)
        };
        UsuarioDTO usuarioNoAdminDTO = UsuarioDTO.DesdeEntidad(_usuarioNoAdmin);

        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO,usuarioNoAdminDTO);

        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea1);
        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea2);
        
        gestorTareas.AgregarMiembroATarea(_adminDTO, tarea1.Id, proyecto.Id, usuarioNoAdminDTO);
        
        gestorTareas.AgregarDependenciaATarea(_adminDTO, tarea1.Id, tarea2.Id, proyecto.Id, "SS");
        
        _gestor.EliminarProyecto(proyecto.Id, _adminDTO);
        
        Assert.AreEqual(0, _gestor.ObtenerTodosDominio().Count);
    }

    [TestMethod]
    public void EliminarProyecto_CambiaLaFLagEstaAdministrandoProyectoDelAdministrador()
    {
        _gestor.CrearProyecto(_proyecto, _adminDTO);

        Assert.IsTrue(_admin.EstaAdministrandoUnProyecto);

        _gestor.EliminarProyecto(_proyecto.Id, _adminDTO);

        Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
    }

    [TestMethod]
    public void EliminarProyecto_NotificaAMiembrosDelProyecto()
    {
        UsuarioDTO miembro1 = UsuarioDTO.DesdeEntidad(CrearMiembro());
        UsuarioDTO miembro2 = UsuarioDTO.DesdeEntidad(CrearMiembro());

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, miembro1);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, miembro2);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        _gestor.EliminarProyecto(proyecto.Id, _adminDTO);

        miembro1 = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(miembro1.Id)); // actualización
        miembro2 = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(miembro2.Id));

        string mensajeEsperado = MensajesNotificacion.ProyectoEliminado(proyecto.Nombre);

        Assert.AreEqual(mensajeEsperado, miembro1.Notificaciones.Last().Mensaje);
        Assert.AreEqual(mensajeEsperado, miembro2.Notificaciones.Last().Mensaje);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void ModificarNombreDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    public void ModificarNombreDelProyecto_ModificaNombreDelProyecto()
    {
        _gestor.CrearProyecto(_proyecto, _adminDTO);

        _gestor.ModificarNombreDelProyecto(_proyecto.Id, "Nuevo Nombre", _adminDTO);

        _proyecto = _gestor.ObtenerProyectoPorId(_proyecto.Id); // actualización

        Assert.AreEqual("Nuevo Nombre", _proyecto.Nombre);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void ModificarNombreDelProyecto_LanzaExcepcionSiNombreYaExiste()
    {
        ProyectoDTO proyecto1 = CrearProyectoCon(_admin);
        ProyectoDTO proyecto2 = CrearProyectoCon(CrearAdminProyecto());
        proyecto2.Nombre = "Otro Nombre";

        _gestor.CrearProyecto(proyecto1, _adminDTO);
        _gestor.CrearProyecto(proyecto2, proyecto2.Administrador);

        _gestor.ModificarNombreDelProyecto(proyecto2.Id, proyecto1.Nombre, proyecto2.Administrador);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void ModificarNombreDelProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        _gestor.ModificarNombreDelProyecto(1000, "nuevo", _adminDTO);
    }


    [TestMethod]
    public void ModificarNombreDelProyecto_NotificaALosMiembrosDelProyecto()
    {
        Usuario miembro1 = CrearMiembro();
        Usuario miembro2 = CrearMiembro();
        List<Usuario> miembros = new List<Usuario> { miembro1, miembro2 };
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = miembros.Select(UsuarioListarDTO.DesdeEntidad).ToList();

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.ModificarNombreDelProyecto(proyecto.Id, "Nuevo Nombre", _adminDTO);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        string mensajeEsperado = MensajesNotificacion.NombreProyectoModificado("Proyecto", "Nuevo Nombre");
        foreach (UsuarioListarDTO usuario in proyecto.Miembros)
        {
            Assert.AreEqual(2, usuario.Notificaciones.Count);
            Assert.AreEqual(mensajeEsperado, usuario.Notificaciones[1].Mensaje);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void ModificarDescripcionDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva Descripcion",
            UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void ModificarDescripcionDelProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        _gestor.ModificarDescripcionDelProyecto(1000, "Nueva descripcion", _adminDTO);
    }

    [TestMethod]
    public void ModificarDescripcionDelProyecto_ModificaDescripcionDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.ModificarDescripcionDelProyecto(proyecto.Id, "Nueva descripcion", _adminDTO);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        Assert.AreEqual("Nueva descripcion", proyecto.Descripcion);
    }

    [TestMethod]
    public void ModificarDescripcionDelProyecto_NotificaALosMiembrosDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        string nuevaDescripcion = "Nueva descripcion";
        _gestor.ModificarDescripcionDelProyecto(proyecto.Id, nuevaDescripcion, _adminDTO);

        string mensajeEsperado = MensajesNotificacion.DescripcionProyectoModificada(proyecto.Nombre, nuevaDescripcion);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        foreach (var usuario in proyecto.Miembros)
        {
            Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        DateTime nuevaFecha = DateTime.Now;
        _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha,
            UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void ModificarFechaDeInicioDelProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        DateTime nuevaFecha = DateTime.Now;
        _gestor.ModificarFechaDeInicioDelProyecto(1000, nuevaFecha, _adminDTO);
    }

    [TestMethod]
    public void ModificarFechaDeInicioDelProyecto_ModificaFechaDeInicioDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        DateTime nuevaFecha = new DateTime(2026, 5, 1);
        _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _adminDTO);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        Assert.AreEqual(nuevaFecha, proyecto.FechaInicio);
    }

    [TestMethod]
    public void ModificarFechaDeInicioDelProyecto_NotificaALosMiembrosDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        proyecto.Miembros = new List<UsuarioListarDTO> { UsuarioListarDTO.DesdeEntidad(_usuarioNoAdmin) };

        _gestor.CrearProyecto(proyecto, _adminDTO);

        DateTime nuevaFecha = new DateTime(2026, 5, 1);
        _gestor.ModificarFechaDeInicioDelProyecto(proyecto.Id, nuevaFecha, _adminDTO);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        string mensajeEsperado = MensajesNotificacion.FechaInicioProyectoModificada(proyecto.Nombre, nuevaFecha);

        foreach (var usuario in proyecto.Miembros)
        {
            Assert.AreEqual(mensajeEsperado, usuario.Notificaciones.Last().Mensaje);
        }
    }

    [TestMethod]
    public void CambiarAdministradorDeProyecto_AsignaNuevoAdminOK()
    {
        Usuario nuevoAdmin = CrearAdminProyecto();

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(nuevoAdmin));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        Assert.AreEqual(nuevoAdmin.Id, proyecto.Administrador.Id);
    }

    [TestMethod]
    public void CambiarAdministradorDeProyecto_DesactivaFlagDelAdminAnterior()
    {
        Usuario adminNuevo = CrearAdminProyecto();

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(adminNuevo));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

        Assert.IsFalse(_admin.EstaAdministrandoUnProyecto);
    }

    [TestMethod]
    public void CambiarAdministradorDeProyecto_ActivaFlagDelAdminNuevo()
    {
        Usuario adminNuevo = CrearAdminProyecto();

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(adminNuevo));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, adminNuevo.Id);

        Assert.IsTrue(adminNuevo.EstaAdministrandoUnProyecto);
    }


    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CambiarAdministradorDeProyecto_LanzaExcepcionSiSolicitanteNoEsAdminSistema()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.CambiarAdministradorDeProyecto(_adminDTO, proyecto.Id, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void CambiarAdministradorDeProyecto_LanzaExcepcionSiProyectoNoExiste()
    {
        _gestor.CambiarAdministradorDeProyecto(_adminSistema, 1000, 1);
    }


    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CambiarAdministradorDeProyecto_LanzaExcepcionSiNuevoAdminNoEsMiembro()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CambiarAdministradorDeProyecto_LanzaExcepcionNuevoAdminYaAdministraOtroProyecto()
    {
        Usuario nuevoAdmin = CrearAdminProyecto();
        nuevoAdmin.EstaAdministrandoUnProyecto = true;

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(nuevoAdmin));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void CambiarAdministradorDeProyecto_LanzaExcepcion_NuevoAdminNoTienePermisosDeAdminProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    public void CambiarAdministradorDeProyecto_NotificaALosMiembros()
    {
        Usuario nuevoAdmin = CrearAdminProyecto();

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(nuevoAdmin));
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.CambiarAdministradorDeProyecto(_adminSistema, proyecto.Id, nuevoAdmin.Id);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        string msg = MensajesNotificacion.AdministradorProyectoModificado(proyecto.Nombre,
            $"{nuevoAdmin.Nombre} {nuevoAdmin.Apellido} ({nuevoAdmin.Email})");

        Assert.AreEqual(msg, _admin.Notificaciones.Last().Mensaje);
        Assert.AreEqual(msg, nuevoAdmin.Notificaciones.Last().Mensaje);
        Assert.AreEqual(msg, _usuarioNoAdmin.Notificaciones.Last().Mensaje);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void AgregarMiembro_LanzaExcepcionSiProyectoNoExiste()
    {
        _gestor.AgregarMiembroAProyecto(1000, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarMiembro_LanzaExcepcionSiSolicitanteNoEsAdminProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro());

        _gestor.AgregarMiembroAProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin), nuevo);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AgregarMiembro_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
    {
        UsuarioDTO solicitante = UsuarioDTO.DesdeEntidad(CrearAdminProyecto());
        UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro());
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.AgregarMiembroAProyecto(proyecto.Id, solicitante, nuevo);
    }

    [TestMethod]
    public void AgregarMiembro_AgregaElMiembroALaListaOK()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        UsuarioDTO nuevo = UsuarioDTO.DesdeEntidad(CrearMiembro());

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, nuevo);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización
        Assert.IsTrue(proyecto.Miembros.Any(u => u.Id == nuevo.Id));
    }

    [TestMethod]
    public void AgregarMiembro_NotificaALosMiembros()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        string esperado = MensajesNotificacion.MiembroAgregado(proyecto.Nombre, _usuarioNoAdmin.Id);

        foreach (var usuario in proyecto.Miembros)
        {
            Assert.IsTrue(usuario.Notificaciones.Any(n => n.Mensaje == esperado));
        }

        Assert.IsTrue(proyecto.Administrador.Notificaciones.Any(n => n.Mensaje == esperado));
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void EliminarMiembroDelProyecto_ProyectoNoExiste_LanzaExcepcion()
    {
        _gestor.EliminarMiembroDelProyecto(1000, _adminDTO, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarMiembroDelProyecto_LanzaExcepcionSiSolicitanteNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin),
            _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarMiembroDelProyecto_LanzaExcepcionSolicitanteNoEsAdministradorDelProyecto()
    {
        UsuarioDTO solicitante = UsuarioDTO.DesdeEntidad(CrearAdminProyecto());
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, solicitante, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void EliminarMiembroDelProyecto_LanzaExcepcionSiUsuarioNoEsMiembroDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void EliminarMiembroConTareaAsignada_LanzaExcepcion()
    {
        GestorTareas gestorTareas = new GestorTareas(_repositorioProyectos, _repositorioUsuarios, _repositorioRecursos, _notificador, _caminoCritico);
        TareaDTO tarea = new TareaDTO()
        {
            Titulo = "Tarea", Descripcion = "Descripcion", DuracionEnDias = 2,
            FechaInicioMasTemprana = DateTime.Today.AddDays(5)
        };

        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea);
        gestorTareas.AgregarMiembroATarea(_adminDTO, tarea.Id, proyecto.Id,
            UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);
    }

    [TestMethod]
    public void EliminarMiembroDelProyecto_EliminaMiembroOK()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, _usuarioNoAdmin.Id);

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id); // actualización

        Assert.IsFalse(proyecto.Miembros.Any(u => u.Id == _usuarioNoAdmin.Id));
    }

    [TestMethod]
    public void EliminarMiembroDelProyecto_NotificaALosMiembros()
    {
        UsuarioDTO miembro1 = UsuarioDTO.DesdeEntidad(CrearMiembro());
        UsuarioDTO miembro2 = UsuarioDTO.DesdeEntidad(CrearMiembro());

        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, miembro1);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, miembro2);

        _gestor.EliminarMiembroDelProyecto(proyecto.Id, _adminDTO, miembro1.Id);

        string esperado = MensajesNotificacion.MiembroEliminado(proyecto.Nombre, miembro1.Id);

        _adminDTO = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(_adminDTO.Id)); // actualización
        miembro2 = UsuarioDTO.DesdeEntidad(_repositorioUsuarios.ObtenerPorId(miembro2.Id));

        Assert.IsTrue(_adminDTO.Notificaciones.Any(n => n.Mensaje == esperado));
        Assert.IsTrue(miembro2.Notificaciones.Any(n => n.Mensaje == esperado));
    }

    [TestMethod]
    public void ObtenerProyectosPorUsuario_DevuelveProyectosDelMiembro()
    {
        Usuario admin1 = CrearAdminProyecto();
        Usuario admin2 = CrearAdminProyecto();
        UsuarioDTO miembro1 = UsuarioDTO.DesdeEntidad(CrearMiembro());
        UsuarioDTO miembro2 = UsuarioDTO.DesdeEntidad(CrearMiembro());

        ProyectoDTO proyecto1 = CrearProyectoCon(admin1);
        proyecto1.Nombre = "Proyecto 1";

        _gestor.CrearProyecto(proyecto1, UsuarioDTO.DesdeEntidad(admin1));
        _gestor.AgregarMiembroAProyecto(proyecto1.Id, UsuarioDTO.DesdeEntidad(admin1), miembro1);
        _gestor.AgregarMiembroAProyecto(proyecto1.Id, UsuarioDTO.DesdeEntidad(admin1), miembro2);

        ProyectoDTO proyecto2 = CrearProyectoCon(admin2);
        proyecto2.Nombre = "Proyecto 2";

        _gestor.CrearProyecto(proyecto2, UsuarioDTO.DesdeEntidad(admin2));
        _gestor.AgregarMiembroAProyecto(proyecto2.Id, UsuarioDTO.DesdeEntidad(admin2), miembro1);

        List<ProyectoDTO> proyectosDeMiembro1 = _gestor.ObtenerProyectosPorUsuario(miembro1.Id);
        List<ProyectoDTO> proyectosDeMiembro2 = _gestor.ObtenerProyectosPorUsuario(miembro2.Id);

        Assert.AreEqual(2, proyectosDeMiembro1.Count);
        Assert.IsTrue(proyectosDeMiembro1.Any(p => p.Id == proyecto1.Id));
        Assert.IsTrue(proyectosDeMiembro1.Any(p => p.Id == proyecto2.Id));

        Assert.AreEqual(1, proyectosDeMiembro2.Count);
        Assert.AreEqual(proyecto1.Id, proyectosDeMiembro2[0].Id);
    }

    [TestMethod]
    public void ObtenerProyectoDelAdministrador_DevuelveProyectoCorrecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        Proyecto resultado = _gestor.ObtenerProyectoDelAdministrador(_admin.Id);

        Assert.AreEqual(proyecto.Id, resultado.Id);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionProyecto))]
    public void ObtenerProyectoDelAdministrador_LanzaExcepcionSiNoExisteProyectoConEseAdmin()
    {
        _gestor.ObtenerProyectoDelAdministrador(_admin.Id);
    }

    [TestMethod]
    public void NotificarAdministradoresDeProyectos_NotificaAdministradores()
    {
        Proyecto proyecto1 = new Proyecto("Proyecto 1", "Descripción 1", DateTime.Today.AddDays(1), _admin,
            new List<Usuario>());
        Usuario otroAdmin = CrearAdminProyecto();
        Proyecto proyecto2 = new Proyecto("Proyecto 2", "Descripción 2", DateTime.Today.AddDays(2), otroAdmin,
            new List<Usuario>());

        List<Proyecto> proyectos = new List<Proyecto> { proyecto1, proyecto2 };
        _gestor.NotificarAdministradoresDeProyectos(proyectos, "notificación");

        Notificacion ultimaNotificacionAdmin = _admin.Notificaciones.Last();
        Notificacion ultimaNotificacionOtroAdmin = otroAdmin.Notificaciones.Last();
        Assert.AreEqual("notificación", ultimaNotificacionAdmin.Mensaje);
        Assert.AreEqual("notificación", ultimaNotificacionOtroAdmin.Mensaje);
    }

    [TestMethod]
    public void EsAdministradorDeProyecto_DevuelveTrueSiEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        bool esAdmin = _gestor.EsAdministradorDeProyecto(_adminDTO, proyecto.Id);
        Assert.IsTrue(esAdmin);
    }

    [TestMethod]
    public void EsAdministradorDeProyecto_DevuelveFalseSiNoEsAdmin()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        bool esAdmin = _gestor.EsAdministradorDeProyecto(UsuarioDTO.DesdeEntidad(_usuarioNoAdmin), proyecto.Id);
        Assert.IsFalse(esAdmin);
    }

    [TestMethod]
    public void EsMiembroDeProyecto_DevuelveTrueSiEsMiembro()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(_usuarioNoAdmin));

        bool esMiembro = _gestor.EsMiembroDeProyecto(_usuarioNoAdmin.Id, proyecto.Id);

        Assert.IsTrue(esMiembro);
    }

    [TestMethod]
    public void EsMiembroDeProyecto_DevuelveFalseSiNoEsMiembro()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        bool esMiembro = _gestor.EsMiembroDeProyecto(_usuarioNoAdmin.Id, proyecto.Id);
        Assert.IsFalse(esMiembro);
    }

    [TestMethod]
    public void ObtenerTodosLosProyectosDevuelveListaCorrecta()
    {
        Usuario admin1 = CrearAdminProyecto();
        Usuario admin2 = CrearAdminProyecto();
        ProyectoDTO proyecto1 = CrearProyectoCon(admin1);
        proyecto1.Nombre = "Proyecto 1";
        ProyectoDTO proyecto2 = CrearProyectoCon(admin2);
        proyecto2.Nombre = "Proyecto 2";

        _gestor.CrearProyecto(proyecto1, UsuarioDTO.DesdeEntidad(admin1));
        _gestor.CrearProyecto(proyecto2, UsuarioDTO.DesdeEntidad(admin2));

        List<ProyectoDTO> proyectos = _gestor.ObtenerTodos();

        Assert.AreEqual(2, proyectos.Count);
        Assert.IsTrue(proyectos.Any(p => p.Nombre == "Proyecto 1"));
        Assert.IsTrue(proyectos.Any(p => p.Nombre == "Proyecto 2"));
    }

    [TestMethod]
    public void ObtenerTodosLosProyectosDevuelveListaVaciaSiNoHayProyectos()
    {
        List<ProyectoDTO> proyectos = _gestor.ObtenerTodos();
        Assert.AreEqual(0, proyectos.Count);
    }

    [TestMethod]
    public void CalcularCaminoCritico_MarcaTareasCriticasCorrectamente()
    {
        GestorTareas gestorTareas = new GestorTareas(_repositorioProyectos, _repositorioUsuarios, _repositorioRecursos, _notificador, _caminoCritico);

        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        TareaDTO tarea1 = new TareaDTO
        {
            Titulo = "Inicio", Descripcion = "desc", DuracionEnDias = 2,
            FechaInicioMasTemprana = DateTime.Today.AddDays(2)
        };
        TareaDTO tarea2 = new TareaDTO
        {
            Titulo = "Intermedia", Descripcion = "desc", DuracionEnDias = 3,
            FechaInicioMasTemprana = DateTime.Today.AddDays(2)
        };
        TareaDTO tarea3 = new TareaDTO
        {
            Titulo = "Final", Descripcion = "desc", DuracionEnDias = 2,
            FechaInicioMasTemprana = DateTime.Today.AddDays(2)
        };

        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea1);
        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea2);
        gestorTareas.AgregarTareaAlProyecto(proyecto.Id, _adminDTO, tarea3);
        gestorTareas.AgregarDependenciaATarea(_adminDTO, tarea2.Id, tarea1.Id, proyecto.Id, "FS");
        gestorTareas.AgregarDependenciaATarea(_adminDTO, tarea3.Id, tarea2.Id, proyecto.Id, "FS");

        _gestor.CalcularCaminoCritico(proyecto);

        Proyecto proyectoActualizadoDominio = _gestor.ObtenerProyectoDominioPorId(proyecto.Id);
        List<Tarea> tareasCriticas = proyectoActualizadoDominio.Tareas.Where(t => t.EsCritica()).ToList();

        Assert.AreEqual(3, tareasCriticas.Count); // Todas las tareas deberían estar en el camino crítico
    }
    
    [TestMethod]
    public void AsignarLider_AsignaCorrectamenteElLider()
    {
        Usuario nuevoLider = CrearMiembro(); 
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(nuevoLider));

        _gestor.AsignarLider(proyecto.Id, _adminDTO, nuevoLider.Id);

        Proyecto proyectoActualizado = _gestor.ObtenerProyectoDominioPorId(proyecto.Id);

        Assert.AreEqual(nuevoLider.Id, proyectoActualizado.Lider.Id);
        Assert.IsTrue(proyectoActualizado.Lider.EsLider);
    }
    
    [TestMethod]
    [ExpectedException(typeof(ExcepcionPermisos))]
    public void AsignarLider_LanzaExcepcionSiSolicitanteNoEsAdminDelProyecto()
    {
        Usuario nuevoLider = CrearMiembro();
        ProyectoDTO proyecto = CrearProyectoCon(_admin);

        _gestor.CrearProyecto(proyecto, _adminDTO);
        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, UsuarioDTO.DesdeEntidad(nuevoLider));

        _gestor.AsignarLider(proyecto.Id, UsuarioDTO.DesdeEntidad(nuevoLider), nuevoLider.Id);
    }
    
    [TestMethod]
    public void AsignarLider_NotificaATodosLosMiembrosDelProyecto()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        Usuario nuevoLider = CrearMiembro();
        UsuarioDTO nuevoLiderDTO = UsuarioDTO.DesdeEntidad(nuevoLider);

        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, nuevoLiderDTO);

        _gestor.AsignarLider(proyecto.Id, _adminDTO, nuevoLider.Id);

        string mensajeEsperado = MensajesNotificacion.LiderAsignado(proyecto.Nombre, nuevoLider.ToString());

        proyecto = _gestor.ObtenerProyectoPorId(proyecto.Id);

        foreach (var m in proyecto.Miembros)
        {
            Assert.IsTrue(m.Notificaciones.Any(n => n.Mensaje == mensajeEsperado));
        }
    }
    
    [TestMethod]
    public void DesasignarLider_DesasignaElLiderCorrectamente()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        Usuario lider = CrearMiembro();
        UsuarioDTO liderDTO = UsuarioDTO.DesdeEntidad(lider);

        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, liderDTO);
        _gestor.AsignarLider(proyecto.Id, _adminDTO, lider.Id);

        _gestor.DesasignarLider(proyecto.Id, _adminDTO);

        Proyecto proyectoDominio = _gestor.ObtenerProyectoDominioPorId(proyecto.Id); // actualización

        Assert.IsNull(proyectoDominio.Lider);
    }
    
    [TestMethod]
    public void EsLiderDeProyecto_DevuelveTrueSiEsLider()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        Usuario lider = CrearMiembro();
        UsuarioDTO liderDTO = UsuarioDTO.DesdeEntidad(lider);

        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, liderDTO);

        _gestor.AsignarLider(proyecto.Id, _adminDTO, lider.Id);

        bool esLider = _gestor.EsLiderDeProyecto(liderDTO, proyecto.Id);
        Assert.IsTrue(esLider);
    }

    [TestMethod]
    public void EsLiderDeProyecto_DevuelveFalseSiNoEsLider()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);

        bool esLider = _gestor.EsLiderDeProyecto(UsuarioDTO.DesdeEntidad(_admin), proyecto.Id);
        Assert.IsFalse(esLider);
    }

    [TestMethod]
    public void ExisteLiderEnProyecto_RetornaTrueSiExiste()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        Usuario lider = CrearMiembro();
        UsuarioDTO liderDTO = UsuarioDTO.DesdeEntidad(lider);

        _gestor.AgregarMiembroAProyecto(proyecto.Id, _adminDTO, liderDTO);

        _gestor.AsignarLider(proyecto.Id, _adminDTO, lider.Id);
        
        bool ExisteLider = _gestor.ExisteLiderEnProyecto(proyecto.Id);
        Assert.IsTrue(ExisteLider);
    }
    
    [TestMethod]
    public void ExisteLiderEnProyecto_RetornaFalseSiNoExiste()
    {
        ProyectoDTO proyecto = CrearProyectoCon(_admin);
        _gestor.CrearProyecto(proyecto, _adminDTO);
        
        bool ExisteLider = _gestor.ExisteLiderEnProyecto(proyecto.Id);
        Assert.IsFalse(ExisteLider);
    }
}