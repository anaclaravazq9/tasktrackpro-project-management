using Controladores;
using DTOs;
using IServicios.IGestores;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorRecursosTests
{
    private Mock<IGestorRecursos> _mockGestorRecursos;
    private ControladorRecursos _controladorRecursos;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorRecursos = new Mock<IGestorRecursos>();
        _controladorRecursos = new ControladorRecursos(_mockGestorRecursos.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorRecursos);
    }

    [TestMethod]
    public void AgregarRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        RecursoDTO nuevoRecurso = new RecursoDTO { Id = 2 };

        _mockGestorRecursos.Setup(g => g.AgregarRecurso(usuario, nuevoRecurso, false));

        _controladorRecursos.AgregarRecurso(usuario, nuevoRecurso, false);

        _mockGestorRecursos.Verify(g => g.AgregarRecurso(usuario, nuevoRecurso, false), Times.Once);
    }

    [TestMethod]
    public void EliminarRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecursoAEliminar = 1;

        _mockGestorRecursos.Setup(g => g.EliminarRecurso(usuario, idRecursoAEliminar));

        _controladorRecursos.EliminarRecurso(usuario, idRecursoAEliminar);

        _mockGestorRecursos.Verify(g => g.EliminarRecurso(usuario, idRecursoAEliminar), Times.Once);
    }

    [TestMethod]
    public void ObtenerRecursoPorId_LlamaCorrectamenteAGestor()
    {
        int idRecurso = 1;
        RecursoDTO recursoEsperado = new RecursoDTO { Id = idRecurso };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursoPorId(idRecurso)).Returns(recursoEsperado);

        RecursoDTO resultado = _controladorRecursos.ObtenerRecursoPorId(idRecurso);

        Assert.AreEqual(recursoEsperado.Id, resultado.Id);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursoPorId(idRecurso), Times.Once);
    }

    [TestMethod]
    public void ObtenerRecursosGenerales_LlamaCorrectamenteAGestor()
    {
        var listaEsperada = new List<RecursoDTO>
        {
            new RecursoDTO { Id = 1, Nombre = "Recurso A" },
            new RecursoDTO { Id = 2, Nombre = "Recurso B" }
        };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursosGenerales()).Returns(listaEsperada);

        List<RecursoDTO> resultado = _controladorRecursos.ObtenerRecursosGenerales();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recurso A", resultado[0].Nombre);
        Assert.AreEqual("Recurso B", resultado[1].Nombre);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursosGenerales(), Times.Once);
    }

    [TestMethod]
    public void ObtenerRecursosExclusivos_LlamaCorrectamenteAGestor()
    {
        ProyectoDTO proyecto = new ProyectoDTO { Id = 3 };
        var listaEsperada = new List<RecursoDTO>
        {
            new RecursoDTO { Id = 1, Nombre = "Recurso A", IdProyectoAsociado = proyecto.Id },
            new RecursoDTO { Id = 2, Nombre = "Recurso B", IdProyectoAsociado = proyecto.Id }
        };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursosExclusivos(3)).Returns(listaEsperada);

        List<RecursoDTO> resultado = _controladorRecursos.ObtenerRecursosExclusivos(3);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recurso A", resultado[0].Nombre);
        Assert.AreEqual("Recurso B", resultado[1].Nombre);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursosExclusivos(3), Times.Once);
    }

    [TestMethod]
    public void ModificarNombreRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecurso = 1;
        string nuevoNombre = "Nuevo nombre";

        _mockGestorRecursos.Setup(g => g.ModificarNombreRecurso(usuario, idRecurso, nuevoNombre));

        _controladorRecursos.ModificarNombreRecurso(usuario, idRecurso, nuevoNombre);

        _mockGestorRecursos.Verify(g => g.ModificarNombreRecurso(usuario, idRecurso, nuevoNombre), Times.Once);
    }

    [TestMethod]
    public void ModificarTipoRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecurso = 1;
        string nuevoTipo = "Nuevo tipo";

        _mockGestorRecursos.Setup(g => g.ModificarTipoRecurso(usuario, idRecurso, nuevoTipo));

        _controladorRecursos.ModificarTipoRecurso(usuario, idRecurso, nuevoTipo);

        _mockGestorRecursos.Verify(g => g.ModificarTipoRecurso(usuario, idRecurso, nuevoTipo), Times.Once);
    }

    [TestMethod]
    public void ModificarDecripcionRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecurso = 1;
        string nuevaDescripcion = "Nuevo descripcion";

        _mockGestorRecursos.Setup(g => g.ModificarDescripcionRecurso(usuario, idRecurso, nuevaDescripcion));

        _controladorRecursos.ModificarDescripcionRecurso(usuario, idRecurso, nuevaDescripcion);

        _mockGestorRecursos.Verify(g => g.ModificarDescripcionRecurso(usuario, idRecurso, nuevaDescripcion),
            Times.Once);
    }

    [TestMethod]
    public void ObtenerRecursoExclusivoPorId_LlamaCorrectamenteAGestor()
    {
        int idRecurso = 1;
        ProyectoDTO proyecto = new ProyectoDTO { Id = 2 };
        RecursoDTO recursoEsperado = new RecursoDTO { Id = idRecurso, IdProyectoAsociado = proyecto.Id };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursoExclusivoPorId(proyecto.Id, idRecurso)).Returns(recursoEsperado);

        RecursoDTO resultado = _controladorRecursos.ObtenerRecursoExclusivoPorId(proyecto.Id, idRecurso);

        Assert.AreEqual(recursoEsperado.Id, resultado.Id);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursoExclusivoPorId(proyecto.Id, idRecurso), Times.Once);
    }
    
    [TestMethod]
    public void ModificarCapacidadRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idRecurso = 1;
        int nuevaCapacidad = 10;

        _mockGestorRecursos.Setup(g => g.ModificarCapacidadRecurso(usuario, idRecurso, nuevaCapacidad));

        _controladorRecursos.ModificarCapacidadRecurso(usuario, idRecurso, nuevaCapacidad);

        _mockGestorRecursos.Verify(g => g.ModificarCapacidadRecurso(usuario, idRecurso, nuevaCapacidad), Times.Once);
    }

    [TestMethod]
    public void ObtenerPanelRecursos_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        List<RecursoPanelDTO> recursosEsperados = new List<RecursoPanelDTO>
        {
            new RecursoPanelDTO { Id = 1, Nombre = "Recurso 1" },
            new RecursoPanelDTO { Id = 2, Nombre = "Recurso 2" }
        };

        _mockGestorRecursos.Setup(g => g.ObtenerRecursosParaPanel(idProyecto)).Returns(recursosEsperados);

        List<RecursoPanelDTO> resultado = _controladorRecursos.ObtenerPanelRecursos(idProyecto);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Recurso 1", resultado[0].Nombre);
        Assert.AreEqual("Recurso 2", resultado[1].Nombre);
        _mockGestorRecursos.Verify(g => g.ObtenerRecursosParaPanel(idProyecto), Times.Once);
    }

    
}
