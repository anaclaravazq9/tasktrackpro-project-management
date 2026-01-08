using Controladores;
using Excepciones;
using IServicios;
using Moq;
using Servicios.Exportacion;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorExportacionTests
{
    private ControladorExportacion _controladorExportacion;
    private Mock<IExportadorProyectosFactory> _mockExportadorProyectosFactory;
    
    [TestInitialize]
    public void Setup()
    {
        _mockExportadorProyectosFactory = new Mock<IExportadorProyectosFactory>();
        _controladorExportacion = new ControladorExportacion(_mockExportadorProyectosFactory.Object);
    }
    
    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorExportacion);
    }

    [TestMethod]
    public async Task Exportar_LlamaCorrectamenteAlExportador()
    {
        string formato = "csv";

        Mock<IExportadorProyectos> mockExportador = new Mock<IExportadorProyectos>();
        mockExportador.Setup(e => e.Exportar()).ReturnsAsync(new byte[0]);

        _mockExportadorProyectosFactory.Setup(f => f.CrearExportador(formato))
            .Returns(mockExportador.Object);
        
        await _controladorExportacion.Exportar(formato);
        
        _mockExportadorProyectosFactory.Verify(f => f.CrearExportador(formato), Times.Once);
        mockExportador.Verify(e => e.Exportar(), Times.Once);
    }
}