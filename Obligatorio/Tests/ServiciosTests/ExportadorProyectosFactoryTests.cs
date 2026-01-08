using Servicios.Exportacion;
using Excepciones;
using IServicios;
using Repositorios;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class ExportadorProyectosFactoryTests
{
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    private RepositorioProyectos _repositorioProyectos;
    private ExportadorProyectosFactory _factory;

    [TestInitialize]
    public void Setup()
    {
        _repositorioProyectos = new RepositorioProyectos(_contexto);
        _factory = new ExportadorProyectosFactory(_repositorioProyectos);
    }

    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }
    
    [TestMethod]
    public void Constructor_InicializaExportadorProyectosFactory()
    {
        Assert.IsNotNull(_factory);
    }
    
    [TestMethod]
    public void CrearExportador_ConFormatoCsv_RetornaExportadorCsv()
    {
        IExportadorProyectos exportador = _factory.CrearExportador("csv");

        Assert.IsNotNull(exportador);
        Assert.IsInstanceOfType(exportador, typeof(ExportadorCsv));
    }

    [TestMethod]
    public void CrearExportador_ConFormatoJson_RetornaExportadorJson()
    {
        IExportadorProyectos exportador = _factory.CrearExportador("json");

        Assert.IsNotNull(exportador);
        Assert.IsInstanceOfType(exportador, typeof(ExportadorJson));
    }
    
    [ExpectedException(typeof(ExcepcionExportador))]
    [TestMethod]
    public void CrearExportador_ConFormatoInvalido_LanzaExcepcionExportador()
    {
        _factory.CrearExportador("xls");
    }
}