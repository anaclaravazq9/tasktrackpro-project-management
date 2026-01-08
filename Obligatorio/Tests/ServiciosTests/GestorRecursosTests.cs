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
public class GestorRecursosTests
{
    private Notificador _notificador;
    private CaminoCritico _caminoCritico;
    
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    private RepositorioRecursos _repositorioRecursos;
    private RepositorioUsuarios _repositorioUsuarios;
    private RepositorioProyectos _repositorioProyectos;

    private GestorRecursos _gestorRecursos;
    private GestorProyectos _gestorProyectos;
    private UsuarioDTO _adminSistemaDTO;

    [TestInitialize]
    public void SetUp()
    {
        _repositorioRecursos = new RepositorioRecursos(_contexto);
        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _repositorioProyectos = new RepositorioProyectos(_contexto);
        _notificador = new Notificador(_repositorioUsuarios);
        _caminoCritico = new CaminoCritico(_notificador);

        _gestorProyectos =
            new GestorProyectos(_repositorioUsuarios, _repositorioProyectos, _notificador, _caminoCritico);

        _gestorRecursos =
            new GestorRecursos(_repositorioRecursos, _gestorProyectos, _repositorioUsuarios, _notificador);

        _adminSistemaDTO = CrearAdministradorSistemaDTO();
    }

    private UsuarioDTO CrearAdministradorSistemaDTO()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario admin = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        admin.EsAdministradorSistema = true;
        _repositorioUsuarios.Agregar(admin);
        return UsuarioDTO.DesdeEntidad(admin); // dto
    }

    private Usuario CrearAdministradorProyecto()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario adminProyecto = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        adminProyecto.EsAdministradorProyecto = true;
        _repositorioUsuarios.Agregar(adminProyecto);
        return adminProyecto;
    }

    private UsuarioDTO CrearUsuarioNoAdminDTO()
    {
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Contraseña#3");
        Usuario usuario = new Usuario("Juan", "Pérez", new DateTime(2000, 01, 01), "unemail@gmail.com",
            contrasenaEncriptada);
        _repositorioUsuarios.Agregar(usuario);
        return UsuarioDTO.DesdeEntidad(usuario); // dto
    }
    
    private Tarea CrearTarea()
    {
        return new Tarea("Un título", "una descripcion", 3, DateTime.Today.AddDays(10));
    }

    private Proyecto CrearYAgregarProyecto(Usuario adminProyecto, string nombre)
    {
        DateTime fechaInicio = DateTime.Today.AddDays(1);
        ProyectoDTO proyecto = new ProyectoDTO()
        {
            Nombre = nombre, Descripcion = "Descripción", FechaInicio = fechaInicio,
            Administrador = UsuarioDTO.DesdeEntidad(adminProyecto)
        };
        _gestorProyectos.CrearProyecto(proyecto, UsuarioDTO.DesdeEntidad(adminProyecto));
        return _gestorProyectos.ObtenerProyectoDominioPorId(proyecto.Id);
    }

    private RecursoDTO CrearRecursoDTO()
    {
        return new RecursoDTO()
            { Nombre = "Analista Senior", Tipo = "Humano", Descripcion = "Un analista Senior con experiencia", Capacidad = 2};
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorRecursos);
        Assert.AreEqual(0, _gestorRecursos.ObtenerRecursosGenerales().Count);
    }

    [TestMethod]
    public void AdminSistemaAgregaRecursosCorrectamente()
    {
        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(2, _gestorRecursos.ObtenerRecursosGenerales().Count);
        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(0).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(1).Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiProyectoNoAgregaRecurso()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(usuario, recurso, false);
    }

    [TestMethod]
    public void AdminProyectoAgregaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        RecursoDTO recursoDTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recursoDTO, true);

        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        Assert.IsTrue(recurso.EsExclusivo());
    }

    [TestMethod]
    public void GestorObtieneRecursoPorIdOk()
    {
        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursoPorId(1).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursoPorId(2).Id);
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void GestorNoObtieneRecursoConIdInexistente()
    {
        RecursoDTO recurso = _gestorRecursos.ObtenerRecursoPorId(20);
    }

    [TestMethod]
    public void SeEliminaUnRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);
        Assert.AreEqual(0, _gestorRecursos.ObtenerRecursosGenerales().Count());
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void NoSeEliminaRecursoSiEstaEnUso()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        recurso.CantidadDeTareasUsandolo++;
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoEliminaRecursos()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.EliminarRecurso(usuario, recurso.Id);
    }

    [TestMethod]
    public void AdminProyectoEliminaRecursoExclusivo()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id);
        Assert.AreEqual(0, _repositorioRecursos.ObtenerTodos().Count());
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.EliminarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeEliminarRecursosExclusivosDeOtrosProyectos()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        UsuarioDTO otroAdminProyecto = UsuarioDTO.DesdeEntidad(CrearAdministradorProyecto());
        ProyectoDTO otroProyecto = new ProyectoDTO()
        {
            Nombre = "Otro Nombre", Descripcion = "Descripción", FechaInicio = DateTime.Today.AddDays(1),
            Administrador = otroAdminProyecto
        };
        _gestorProyectos.CrearProyecto(otroProyecto, otroAdminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(otroAdminProyecto, recurso.Id);
    }

    [TestMethod]
    public void EliminarRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado =
            MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion);

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void EliminarRecursoNoExclusivoNotificaAdminDeProyectos()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        _gestorRecursos.EliminarRecurso(_adminSistemaDTO, recurso.Id);

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado =
            MensajesNotificacion.RecursoEliminado(recurso.Nombre, recurso.Tipo, recurso.Descripcion);

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void AdminSistemaModificaNombreDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Nuevo nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [TestMethod]
    public void AdminProyectoModificaNombreDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarNombreRecurso(adminProyectoDTO, recurso.Id, "Nuevo nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo nombre", recurso.Nombre);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarNombre()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarNombreRecurso(usuario, recurso.Id, "Nuevo nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otro nombre");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarNombreDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.Id = 1; // lo hace el repo de usuarios
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        otroAdminProyecto.Id = 2; // lo hace el repo de usuarios

        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id, "Nuevo nombre");
    }

    [TestMethod]
    public void AdminSistemaModificaTipoDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Nuevo tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [TestMethod]
    public void AdminProyectoModificaTipoDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarTipoRecurso(adminProyectoDTO, recurso.Id, "Nuevo tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nuevo tipo", recurso.Tipo);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarTipo()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarTipoRecurso(usuario, recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));


        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarTipoRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id, "Nuevo tipo");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarTipoDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarTipoRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otro tipo");
    }

    [TestMethod]
    public void AdminSistemaModificaDescripcionDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Nueva descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [TestMethod]
    public void AdminProyectoModificaDescripcionDeRecursoExclusivoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recurso, true);

        _gestorRecursos.ModificarDescripcionRecurso(adminProyectoDTO, recurso.Id, "Nueva descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización
        Assert.AreEqual("Nueva descripción", recurso.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoPuedeModificarDescripcion()
    {
        UsuarioDTO usuario = CrearUsuarioNoAdminDTO();
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);
        _gestorRecursos.ModificarDescripcionRecurso(usuario, recurso.Id, "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripciónDeRecursosNoExclusivosDeSuProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");

        Usuario otroAdminProyecto = CrearAdministradorProyecto();
        Proyecto otroProyecto = new Proyecto("Otro Nombre", "Descripción", DateTime.Today.AddDays(1), otroAdminProyecto,
            new List<Usuario>());
        _gestorProyectos.CrearProyecto(ProyectoDTO.DesdeEntidad(otroProyecto),
            UsuarioDTO.DesdeEntidad(otroAdminProyecto));

        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);

        _gestorRecursos.ModificarDescripcionRecurso(UsuarioDTO.DesdeEntidad(otroAdminProyecto), recurso.Id,
            "Nueva descripción");
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void AdminProyectoNoPuedeModificarDescripcionDeRecursoNoExclusivo()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        adminProyecto.EstaAdministrandoUnProyecto =
            true; // hardcodeado por simplicidad de tests (para no crear un proyecto)

        _gestorRecursos.ModificarNombreRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso.Id, "otra descripción");
    }

    [TestMethod]
    public void ModificarNombreDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Otro nombre");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
/*
    [TestMethod]
    public void ModificarNombreDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        RecursoDTO recursoDTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recursoDTO, false);
        
        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        Tarea tarea = CrearTarea();
        tarea.AsignarRecurso(recurso, 1);
        proyecto.AgregarTarea(tarea);

        _gestorRecursos.ModificarNombreRecurso(_adminSistemaDTO, recurso.Id, "Otro nombre");

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }*/

    [TestMethod]
    public void ModificarTipoDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Otro tipo");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }
/*
    [TestMethod]
    public void ModificarTipoDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        RecursoDTO recursoDTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recursoDTO, false);
        
        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        Tarea tarea = CrearTarea();
        tarea.AsignarRecurso(recurso, 1);
        proyecto.AgregarTarea(tarea);

        _gestorRecursos.ModificarTipoRecurso(_adminSistemaDTO, recurso.Id, "Otro tipo");
        

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }*/
/*
    [TestMethod]
    public void ModificarDescripcionDeRecursoExclusivoNotificaAlAdministradorDeProyecto()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso, true);
        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Otra descripción");

        recurso = _gestorRecursos.ObtenerRecursoPorId(recurso.Id); // actualización

        Notificacion ultimaNotificacion = adminProyecto.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.RecursoModificado("Analista Senior",
            $"Nombre: '{recurso.Nombre}', tipo: '{recurso.Tipo}', descripción: '{recurso.Descripcion}'");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }*/

    [TestMethod]
    public void ModificarDescripcionDeRecursoNoExclusivoNotificaAdminDeProyectosQueLoNecesitan()
    {
        Usuario admin1 = CrearAdministradorProyecto();
        Usuario admin2 = CrearAdministradorProyecto();
        
        Proyecto proyecto1 = CrearYAgregarProyecto(admin1, "Nombre1");
        Proyecto proyecto2 = CrearYAgregarProyecto(admin2, "Nombre2");;
        
        RecursoDTO recursoDTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recursoDTO, false);
        
        Recurso recurso = _repositorioRecursos.ObtenerPorId(recursoDTO.Id);
        
        Tarea tarea1 = CrearTarea();
        tarea1.AsignarRecurso(recurso, 1);
        proyecto1.AgregarTarea(tarea1);
        _repositorioProyectos.Actualizar(proyecto1);
        
        Tarea tarea2 = CrearTarea();
        tarea2.AsignarRecurso(recurso, 1);
        proyecto2.AgregarTarea(tarea2);
        _repositorioProyectos.Actualizar(proyecto2);

        _gestorRecursos.ModificarDescripcionRecurso(_adminSistemaDTO, recurso.Id, "Otra descripción");

        Notificacion ultimaNotificacion1 = admin1.Notificaciones.Last();
        Notificacion ultimaNotificacion2 = admin2.Notificaciones.Last();
        Assert.AreEqual(
            "El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Humano', descripción: 'Otra descripción'",
            ultimaNotificacion1.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion1.Fecha);
        Assert.AreEqual(
            "El recurso 'Analista Senior' ha sido modificado. Nuevos valores: Nombre: 'Analista Senior', tipo: 'Humano', descripción: 'Otra descripción'",
            ultimaNotificacion2.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion2.Fecha);
    }
    
    [TestMethod]
    public void SeMuestranRecursosGeneralesOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        CrearYAgregarProyecto(adminProyecto, "Nombre");
        RecursoDTO recursoExclusivo = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recursoExclusivo, true);

        RecursoDTO recurso1 = CrearRecursoDTO();
        RecursoDTO recurso2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso1, false);
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso2, false);

        Assert.AreEqual(2, _gestorRecursos.ObtenerRecursosGenerales().Count);
        Assert.AreEqual(recurso1.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(0).Id);
        Assert.AreEqual(recurso2.Id, _gestorRecursos.ObtenerRecursosGenerales().ElementAt(1).Id);
    }

    [TestMethod]
    public void SeMuestranRecursosExclusivosDeUnProyectoOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");

        RecursoDTO recursoExclusivo1 = CrearRecursoDTO();
        RecursoDTO recursoExclusivo2 = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo1, true);
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo2, true);

        List<RecursoDTO> recursosExclusivosDelProyecto = _gestorRecursos.ObtenerRecursosExclusivos(proyecto.Id);
        Assert.AreEqual(2, recursosExclusivosDelProyecto.Count());
        Assert.AreEqual(recursoExclusivo1.Id, recursosExclusivosDelProyecto.ElementAt(0).Id);
        Assert.AreEqual(recursoExclusivo2.Id, recursosExclusivosDelProyecto.ElementAt(1).Id);
    }

    [TestMethod]
    public void SeObtieneUnRecursoExclusivoPorIdOk()
    {
        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        UsuarioDTO adminProyectoDTO = UsuarioDTO.DesdeEntidad(adminProyecto);

        RecursoDTO recursoExclusivo = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(adminProyectoDTO, recursoExclusivo, true);

        RecursoDTO recursoObtenido = _gestorRecursos.ObtenerRecursoExclusivoPorId(proyecto.Id, recursoExclusivo.Id);

        Assert.AreEqual(recursoObtenido.Id, recursoExclusivo.Id);
        Assert.AreEqual(recursoObtenido.Nombre, recursoExclusivo.Nombre);
        Assert.AreEqual(recursoObtenido.Tipo, recursoExclusivo.Tipo);
        Assert.AreEqual(recursoObtenido.Descripcion, recursoExclusivo.Descripcion);
    }

    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void ObtenerUnRecursoExclusivoInexistenteLanzaExcepcion()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        Usuario adminProyecto = CrearAdministradorProyecto();
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");

        _gestorRecursos.ObtenerRecursoExclusivoPorId(proyecto.Id, recurso.Id);
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void NoPuedeAgregarUnRecursoUnSolicitanteInexistente()
    {
        UsuarioDTO usuarioNoEnRepositorio = new UsuarioDTO()
        {
            Id = 99999, Nombre = "Juan", Apellido = "Pérez", FechaNacimiento = new DateTime(2000, 01, 01),
            Email = "juan@perez.com", Contrasena = "Juan123>$%"
        };
        RecursoDTO recurso = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(usuarioNoEnRepositorio, recurso, false);
    }
    
    [TestMethod]
    public void AdminSistemaModificaCapacidadDeRecursoOk()
    {
        RecursoDTO recurso = CrearRecursoDTO();
        recurso.Capacidad = 5;
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        _gestorRecursos.ModificarCapacidadRecurso(_adminSistemaDTO, recurso.Id, 10);

        Recurso recursoModificado = _repositorioRecursos.ObtenerPorId(recurso.Id);
        Assert.AreEqual(10, recursoModificado.Capacidad);
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void ModificarCapacidadLanzaExcepcionSiCapacidadMenorAlUsoActual()
    {
        Recurso recurso = new Recurso("Servidor", "Hardware", "Alta disponibilidad", 5);
        recurso.AgregarRangoDeUso(DateTime.Today, DateTime.Today.AddDays(1), 3);
        recurso.AgregarRangoDeUso(DateTime.Today, DateTime.Today.AddDays(1), 3); // uso total = 6
        recurso.Id = 1;
        _repositorioRecursos.Agregar(recurso);

        _gestorRecursos.ModificarCapacidadRecurso(_adminSistemaDTO, recurso.Id, 4);
    }
    
    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void UsuarioSinPermisosNoPuedeModificarCapacidad()
    {
        UsuarioDTO usuarioNoAdmin = CrearUsuarioNoAdminDTO();

        RecursoDTO recurso = CrearRecursoDTO();
        recurso.Capacidad = 5;
        _gestorRecursos.AgregarRecurso(_adminSistemaDTO, recurso, false);

        _gestorRecursos.ModificarCapacidadRecurso(usuarioNoAdmin, recurso.Id, 10);
    }

    [TestMethod]
    public void ObtenerRecursoParaPanelDevuelveUnaListaCorrecta()
    {
        Usuario adminProyecto = CrearAdministradorProyecto(); 
        Proyecto proyecto = CrearYAgregarProyecto(adminProyecto, "Nombre");
        
        RecursoDTO recurso1DTO = CrearRecursoDTO();
        RecursoDTO recurso2DTO = CrearRecursoDTO();
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso1DTO, true);
        _gestorRecursos.AgregarRecurso(UsuarioDTO.DesdeEntidad(adminProyecto), recurso2DTO, true);
        
        Recurso recurso1 = _repositorioRecursos.ObtenerPorId(recurso1DTO.Id);
        Recurso recurso2 = _repositorioRecursos.ObtenerPorId(recurso2DTO.Id);
        
        Tarea tarea1 = CrearTarea();
        tarea1.Estado = EstadoTarea.EnProceso;
        tarea1.AsignarRecurso(recurso1, 1);
        proyecto.AgregarTarea(tarea1);
        _repositorioProyectos.Actualizar(proyecto);
        
        Tarea tarea2 = CrearTarea();
        tarea2.Estado = EstadoTarea.Bloqueada;
        tarea2.AsignarRecurso(recurso2, 1);
        proyecto.AgregarTarea(tarea2);
        _repositorioProyectos.Actualizar(proyecto);

        List<RecursoPanelDTO> recursosPanel = _gestorRecursos.ObtenerRecursosParaPanel(proyecto.Id);

        Assert.AreEqual(2, recursosPanel.Count);
        Assert.IsTrue(recursosPanel.Any(r => r.Id == recurso1.Id && r.NivelDeUso == 1));
        Assert.IsTrue(recursosPanel.Any(r => r.Id == recurso2.Id && r.NivelDeUso == 0));
    }
}