using Dominio;
using Excepciones;
using Repositorios;
using Servicios.CaminoCritico;
using Servicios.Notificaciones;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class CaminoCriticoTests
{
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    
    private CaminoCritico _caminoCritico;
    private Proyecto _proyecto;
    private Usuario _admin = new Usuario("Juan", "Pérez", new DateTime(1999, 2, 2), "email@email.com", "Contra5e{a");
    private Tarea _tarea1;
    private Tarea _tarea2;
    private Tarea _tarea3;
    private Tarea _tarea4;
    private DateTime _fechaHoy = DateTime.Today;

    [TestInitialize]
    public void SetUp()
    {
        RepositorioUsuarios repositorioUsuarios = new RepositorioUsuarios(_contexto);
        Notificador notificador = new Notificador(repositorioUsuarios);
        
        _caminoCritico = new CaminoCritico(notificador);

        _tarea1 = new Tarea("Tarea 1", "desc", 2, _fechaHoy);
        _tarea2 = new Tarea("Tarea 2", "desc", 3, _fechaHoy.AddDays(3));
        _tarea3 = new Tarea("Tarea 3", "desc", 4, _fechaHoy.AddDays(4));
        _tarea4 = new Tarea("Tarea 4", "desc", 1, _fechaHoy.AddDays(5));

        _tarea1.Id = 1; // por simplicidad de tests, harcodeamos ids en vez de usar el repositorio
        _tarea2.Id = 2;
        _tarea3.Id = 3;
        _tarea4.Id = 4;

        _tarea2.AgregarDependencia(new Dependencia("SS", _tarea1));
        _tarea2.AgregarDependencia(new Dependencia("FS", _tarea3));
        _tarea3.AgregarDependencia(new Dependencia("FS", _tarea1));

        _proyecto = new Proyecto("Nombre", "desc", _fechaHoy, _admin, new List<Usuario>());
    }

    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }
    
    [TestMethod]
    public void CalculaCaminoCriticoOk()
    {
        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);

        _caminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsTrue(_tarea1.EsCritica());
        Assert.IsTrue(_tarea2.EsCritica());
        Assert.IsTrue(_tarea3.EsCritica());
        Assert.IsFalse(_tarea4.EsCritica());
        Assert.AreEqual(3, _tarea4.Holgura);

        Assert.AreEqual(_fechaHoy, _tarea1.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy.AddDays(2), _tarea3.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy.AddDays(6), _tarea2.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy.AddDays(5), _tarea4.FechaInicioMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionCaminoCritico))]
    [TestMethod]
    public void NoSeCalculaCaminoCriticoSiHayCiclos()
    {
        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);

        _tarea3.AgregarDependencia(new Dependencia("SS", _tarea2));
        _caminoCritico.CalcularCaminoCritico(_proyecto);
    }

    [TestMethod]
    public void CalcularCaminoCriticoEnProyectoSinTareasNoLanzaExcepcion()
    {
        _caminoCritico.CalcularCaminoCritico(_proyecto);
        Assert.IsTrue(true); // No se lanzó excepción
    }

    [TestMethod]
    public void VariasTareasIndependientesCaminoCriticoOk()
    {
        Tarea tarea5 = new Tarea("Tarea 5", "desc", 2, _fechaHoy) { Id = 5 };
        Tarea tarea6 = new Tarea("Tarea 6", "desc", 1, _fechaHoy) { Id = 6 };

        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);
        _proyecto.AgregarTarea(tarea5);
        _proyecto.AgregarTarea(tarea6);

        _caminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsFalse(tarea5.EsCritica());
        Assert.IsFalse(tarea6.EsCritica());
    }

    [TestMethod]
    public void TareaSinDependenciasFueraDelCaminoCriticoTieneHolgura()
    {
        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);

        _caminoCritico.CalcularCaminoCritico(_proyecto);
        Assert.IsTrue(_tarea4.Holgura > 0);
    }

    [TestMethod]
    public void TareaUnicaEsCritica()
    {
        Tarea tarea = new Tarea("Única", "desc", 5, _fechaHoy) { Id = 1 };
        _proyecto.AgregarTarea(tarea);

        _caminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsTrue(tarea.EsCritica());
    }

    [TestMethod]
    public void DependenciasSSOk()
    {
        Tarea tarea1 = new Tarea("Tarea 1", "desc", 2, _fechaHoy) { Id = 1 };
        Tarea tarea2 = new Tarea("Tarea 2", "desc", 3, _fechaHoy.AddDays(3)) { Id = 2 };

        tarea2.AgregarDependencia(new Dependencia("SS", tarea1));

        _proyecto.AgregarTarea(tarea1);
        _proyecto.AgregarTarea(tarea2);

        _caminoCritico.CalcularCaminoCritico(_proyecto);

        Assert.IsTrue(tarea1.EsCritica());
        Assert.IsTrue(tarea2.EsCritica());
        Assert.AreEqual(tarea1.FechaInicioMasTemprana, tarea2.FechaInicioMasTemprana);
    }

    [ExpectedException(typeof(ExcepcionCaminoCritico))]
    [TestMethod]
    public void DetectaCicloConDependenciasSS()
    {
        Tarea tarea1 = new Tarea("Tarea 1", "desc", 2, _fechaHoy) { Id = 1 };
        Tarea tarea2 = new Tarea("Tarea 2", "desc", 3, _fechaHoy) { Id = 2 };

        tarea1.AgregarDependencia(new Dependencia("SS", tarea2));
        tarea2.AgregarDependencia(new Dependencia("SS", tarea1));

        _proyecto.AgregarTarea(tarea1);
        _proyecto.AgregarTarea(tarea2);

        _caminoCritico.CalcularCaminoCritico(_proyecto);
        Assert.IsTrue(_tarea1.FechaInicioMasTemprana == _tarea2.FechaInicioMasTemprana);
        Assert.IsFalse(_tarea1.EsCritica());
        Assert.IsFalse(_tarea2.EsCritica());
    }

    [TestMethod]
    public void ModificarFechaFinMasTempranaDelProyecto_NotificaAMiembros()
    {
        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea2);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea4);

        _caminoCritico.CalcularCaminoCritico(_proyecto);

        DateTime nuevaFechaFin = _proyecto.FechaFinMasTemprana;

        string esperado =
            $"Se cambió la fecha de fin más temprana del proyecto '{_proyecto.Nombre}' a '{nuevaFechaFin:dd/MM/yyyy}'.";

        Notificacion ultimaNoti = _admin.Notificaciones.Last();
        Assert.AreEqual(esperado, ultimaNoti.Mensaje);
    }
    
    [TestMethod]
    public void TareaConFechaFijadaNoEsModificada()
    {
        _tarea3.FijarFechaInicio(_fechaHoy.AddDays(100));
        _proyecto.AgregarTarea(_tarea1);
        _proyecto.AgregarTarea(_tarea3);
        _proyecto.AgregarTarea(_tarea2); 
        
        _caminoCritico.CalcularCaminoCritico(_proyecto);
        
        Assert.AreEqual(_fechaHoy.AddDays(100), _tarea3.FechaInicioMasTemprana);
        Assert.AreEqual(_fechaHoy, _tarea1.FechaInicioMasTemprana);
    }
}