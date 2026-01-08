using Dominio;
using DTOs;
using Microsoft.EntityFrameworkCore;
using Repositorios;
using Tests.Contexto;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioProyectosTest
{
    private RepositorioProyectos _repositorioProyectos;
    private SqlContext _contexto;
    private Usuario _usuario;
    private Proyecto _proyecto;

    [TestInitialize]
    public void SetUp()
    {
        _contexto = SqlContextFactory.CrearContextoEnMemoria();

        _repositorioProyectos = new RepositorioProyectos(_contexto);

        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
        _usuario.EsAdministradorProyecto = true;
        _contexto.Usuarios.Add(_usuario);

        List<Usuario> miembros = new();
        _proyecto = new Proyecto("Proyecto", "hacer algo", DateTime.Today.AddDays(10), _usuario, miembros);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void ConstructorCreaRepositorioOk()
    {
        Proyecto proyecto = _repositorioProyectos.ObtenerPorId(1);
        Assert.IsNull(proyecto);
    }

    [TestMethod]
    public void SeAgregaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Assert.AreEqual(_proyecto, _repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        _repositorioProyectos.Agregar(_proyecto);

        Usuario admin2 = new Usuario("Otro", "Usuario", new DateTime(1995, 5, 15), "otro@email.com", "hashhxxx");
        _contexto.Usuarios.Add(admin2);
        Proyecto proyecto2 = new Proyecto("Proyecto2", "hacer algo2", DateTime.Today.AddDays(10), admin2, new List<Usuario>());
        _repositorioProyectos.Agregar(proyecto2);

        Assert.AreEqual(1, _proyecto.Id);
        Assert.AreEqual(2, proyecto2.Id);
    }

    [TestMethod]
    public void SeEliminaProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        _repositorioProyectos.Eliminar(_proyecto.Id);
        Assert.IsNull(_repositorioProyectos.ObtenerPorId(_proyecto.Id));
    }

    [TestMethod]
    public void SeObtieneLaListaDeProyectosOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        var proyectos = _repositorioProyectos.ObtenerTodos();
        Assert.IsNotNull(proyectos);
        Assert.AreEqual(1, proyectos.Count);
        Assert.AreEqual(_proyecto, proyectos.Last());
    }

    [TestMethod]
    public void SeActualizaNombreDescripcionYFechasDeProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);

        Proyecto proyectoModificado = new Proyecto("Nuevo proyecto", "Nueva descripción", DateTime.Today.AddDays(5),
            _usuario, new List<Usuario>())
        {
            Id = _proyecto.Id,
            FechaFinMasTemprana = DateTime.Today.AddDays(15)
        };

        _repositorioProyectos.Actualizar(proyectoModificado);

        Proyecto proyectoActualizado = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual("Nuevo proyecto", proyectoActualizado.Nombre);
        Assert.AreEqual("Nueva descripción", proyectoActualizado.Descripcion);
        Assert.AreEqual(DateTime.Today.AddDays(5), proyectoActualizado.FechaInicio);
        Assert.AreEqual(DateTime.Today.AddDays(15), proyectoActualizado.FechaFinMasTemprana);
    }

    [TestMethod]
    public void SeActualizaAdministradorDelProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Usuario nuevoAdmin = new Usuario("Ana", "López", new DateTime(1990, 1, 1), "ana@mail.com", "otraClave");
        _contexto.Usuarios.Add(nuevoAdmin);
        _proyecto.Miembros.Add(nuevoAdmin);
        _repositorioProyectos.Actualizar(_proyecto);

        Proyecto proyectoModificado = new Proyecto(_proyecto.Nombre, _proyecto.Descripcion, _proyecto.FechaInicio,
            nuevoAdmin, new List<Usuario>())
        {
            Id = _proyecto.Id
        };

        _repositorioProyectos.Actualizar(proyectoModificado);
        Proyecto proyectoActualizado = _repositorioProyectos.ObtenerPorId(_proyecto.Id);

        Assert.AreEqual(nuevoAdmin.Id, proyectoActualizado.Administrador.Id);
    }
    
    [TestMethod]
    public void SeActualizaLiderDelProyectoOk()
    {
        _repositorioProyectos.Agregar(_proyecto);
        Usuario lider = new Usuario("Ana", "López", new DateTime(1990, 1, 1), "ana@mail.com", "otraClave");
        lider.EsLider = true;
        _contexto.Usuarios.Add(lider);
        _proyecto.Miembros.Add(lider);
        _repositorioProyectos.Actualizar(_proyecto);

        Proyecto proyectoModificado = new Proyecto(_proyecto.Nombre, _proyecto.Descripcion, _proyecto.FechaInicio,
            _proyecto.Administrador, _proyecto.Miembros.ToList())
        {
            Id = _proyecto.Id
        };
        proyectoModificado.AsignarLider(lider);

        _repositorioProyectos.Actualizar(proyectoModificado);
        Proyecto proyectoActualizado = _repositorioProyectos.ObtenerPorId(_proyecto.Id);

        Assert.AreEqual(lider.Id, proyectoActualizado.Lider.Id);
    }

    [TestMethod]
    public void SeAgreganYEliminanMiembrosOk()
    {
        DateTime fechaValida = new DateTime(1999, 1, 1);
        Usuario miembro1 = new("M1", "A", fechaValida, "m1@mail.com", "pass");
        Usuario miembro2 = new("M2", "B", fechaValida, "m2@mail.com", "pass");
        _contexto.Usuarios.Add(miembro1);
        _contexto.Usuarios.Add(miembro2);

        _proyecto.Miembros.Add(miembro1);
        _repositorioProyectos.Agregar(_proyecto); //proyecto con admin y miembro1

        Proyecto proyectoModificado = new Proyecto(_proyecto.Nombre, _proyecto.Descripcion, _proyecto.FechaInicio,
            _usuario, new List<Usuario> { miembro2 })
        {
            Id = _proyecto.Id
        }; // proyecto con admin y miembro2

        _repositorioProyectos.Actualizar(proyectoModificado);

        Proyecto proyectoActualizado = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual(2, proyectoActualizado.Miembros.Count);
        Assert.AreEqual(_usuario.Id, proyectoActualizado.Miembros.First().Id);
        Assert.AreEqual(miembro2.Id, proyectoActualizado.Miembros.Last().Id);
    }

    [TestMethod]
    public void SeAgreganYEliminanTareasOk()
    {
        Tarea tareaAnterior = new Tarea("Tarea Anterior", "detalle", 2, DateTime.Today.AddDays(5));
        _proyecto.AgregarTarea(tareaAnterior);
        _repositorioProyectos.Agregar(_proyecto);

        Tarea tarea = new Tarea("Tarea 1", "detalle", 3, DateTime.Today.AddDays(1));
        Proyecto proyectoModificado = new(_proyecto.Nombre, _proyecto.Descripcion, _proyecto.FechaInicio, _usuario,
            new List<Usuario>())
        {
            Id = _proyecto.Id,
        };
        proyectoModificado.AgregarTarea(tarea);

        _repositorioProyectos.Actualizar(proyectoModificado);

        Proyecto proyectoActualizado = _repositorioProyectos.ObtenerPorId(_proyecto.Id);
        Assert.AreEqual(1, proyectoActualizado.Tareas.Count);
        Assert.AreEqual("Tarea 1", proyectoActualizado.Tareas.First().Titulo);
    }

    [TestMethod]
    public void SeActualizanPropiedadesSimplesDeTarea()
    {
        Tarea tarea = new Tarea("Tarea Original", "detalle", 4, DateTime.Today.AddDays(10));
        tarea.CambiarEstado(EstadoTarea.EnProceso);
        _proyecto.AgregarTarea(tarea);
        _repositorioProyectos.Agregar(_proyecto);

        Tarea tareaModificada = new Tarea("Tarea Modificada", "nuevo detalle", 1, DateTime.Today)
        {
            Id = tarea.Id,
            Holgura = 0
        };
        tareaModificada.CambiarEstado(EstadoTarea.EnProceso);
        tareaModificada.CambiarEstado(EstadoTarea.Completada);


        _repositorioProyectos.ActualizarTarea(tareaModificada);

        Tarea tareaActualizada = _contexto.Set<Tarea>().First(t => t.Id == tarea.Id);
        Assert.AreEqual("Tarea Modificada", tareaActualizada.Titulo);
        Assert.AreEqual("nuevo detalle", tareaActualizada.Descripcion);
        Assert.AreEqual(DateTime.Today, tareaActualizada.FechaInicioMasTemprana);
        Assert.AreEqual(1, tareaActualizada.DuracionEnDias);
        Assert.AreEqual(0, tareaActualizada.Holgura);
        Assert.AreEqual(EstadoTarea.Completada, tareaActualizada.Estado);
        Assert.AreEqual(DateTime.Today, tareaActualizada.FechaDeEjecucion);
    }

    [TestMethod]
    public void SeSincronizanUsuariosAsignadosEnTarea()
    {
        Usuario usuarioA = new("U1", "A", new DateTime(1999, 1, 1), "a@a.com", "pass");
        Usuario usuarioB = new("U2", "B", new DateTime(1999, 1, 1), "b@b.com", "pass");
        _contexto.Usuarios.Add(usuarioA);
        _contexto.Usuarios.Add(usuarioB);

        Tarea tarea = new("Tarea", "detalle", 3, DateTime.Today.AddDays(1));
        tarea.AsignarUsuario(usuarioA);
        _proyecto.AgregarTarea(tarea);
        _repositorioProyectos.Agregar(_proyecto);

        Tarea tareaModificada = new("Tarea", "detalle", 3, DateTime.Today.AddDays(1))
        {
            Id = tarea.Id
        };
        tareaModificada.AsignarUsuario(usuarioB);

        _repositorioProyectos.ActualizarTarea(tareaModificada);

        Tarea tareaActualizada = _contexto.Set<Tarea>().Include(t => t.UsuariosAsignados).First(t => t.Id == tarea.Id);
        Assert.AreEqual(1, tareaActualizada.UsuariosAsignados.Count);
        Assert.AreEqual(usuarioB.Id, tareaActualizada.UsuariosAsignados.First().Id);
    }

    [TestMethod]
    public void SeSincronizanRecursosNecesariosEnTarea()
    {
        Recurso recursoA = new Recurso("RecursoA", "tipoA", "descripciónA",1);
        Recurso recursoB = new Recurso("RecursoB", "tipoB", "descripcionB",1 );
        _contexto.Recursos.Add(recursoA);
        _contexto.Recursos.Add(recursoB);

        Tarea tarea = new("Tarea", "detalle", 3, DateTime.Today.AddDays(1));
        tarea.AsignarRecurso(recursoA,1);
        _proyecto.AgregarTarea(tarea);
        _repositorioProyectos.Agregar(_proyecto);

        Tarea tareaModificada = new("Tarea", "detalle", 3, DateTime.Today.AddDays(1))
        {
            Id = tarea.Id
        };
        tareaModificada.AsignarRecurso(recursoB,1);

        _repositorioProyectos.ActualizarTarea(tareaModificada);

        Tarea tareaActualizada = _contexto.Set<Tarea>().Include(t => t.RecursosNecesarios).First(t => t.Id == tarea.Id);
        Assert.AreEqual(1, tareaActualizada.RecursosNecesarios.Count);
        Assert.AreEqual("RecursoB", tareaActualizada.RecursosNecesarios.First().Recurso.Nombre);
    }

    [TestMethod]
    public void SeAgreganYEliminanDependenciasCorrectamente()
    {
        Tarea tarea1 = new("Tarea1", "detalle", 3, DateTime.Today.AddDays(1));
        Tarea tarea2 = new("Tarea2", "detalle", 2, DateTime.Today.AddDays(2));
        Tarea tarea3 = new("Tarea3", "detalle", 1, DateTime.Today.AddDays(3));
        
        _proyecto.AgregarTarea(tarea1);
        _repositorioProyectos.Agregar(_proyecto);
        _proyecto.AgregarTarea(tarea2);
        _repositorioProyectos.Actualizar(_proyecto);
        _proyecto.AgregarTarea(tarea3);
        _repositorioProyectos.Actualizar(_proyecto);
        
        tarea1.AgregarDependencia(new Dependencia("FS", tarea2));
        _repositorioProyectos.ActualizarTarea(tarea1);
        
        Tarea tareaModificada = new("Tarea1", "detalle", 3, DateTime.Today.AddDays(1))
        {
            Id = tarea1.Id
        };
        tareaModificada.AgregarDependencia(new Dependencia("FS", tarea3));

        _repositorioProyectos.ActualizarTarea(tareaModificada);

        Tarea tareaActualizada = _contexto.Set<Tarea>()
            .Include(t => t.Dependencias)
            .ThenInclude(d => d.Tarea)
            .First(t => t.Id == tarea1.Id);

        Assert.AreEqual(1, tareaActualizada.Dependencias.Count);
        Assert.AreEqual(tarea3.Id, tareaActualizada.Dependencias.First().Tarea.Id);
    }
}