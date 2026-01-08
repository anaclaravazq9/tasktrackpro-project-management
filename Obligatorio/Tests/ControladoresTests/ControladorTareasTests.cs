using Controladores;
using DTOs;
using IServicios.IGestores;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorTareasTests
{
    private Mock<IGestorTareas> _mockGestorTareas;
    private ControladorTareas _controladorTareas;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorTareas = new Mock<IGestorTareas>();
        _controladorTareas = new ControladorTareas(_mockGestorTareas.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorTareas);
    }

    [TestMethod]
    public void AgregarTareaAlProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        TareaDTO nuevaTarea = new TareaDTO { Id = 2 };

        _mockGestorTareas.Setup(g => g.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea));

        _controladorTareas.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea);

        _mockGestorTareas.Verify(g => g.AgregarTareaAlProyecto(idProyecto, usuario, nuevaTarea), Times.Once);
    }

    [TestMethod]
    public void EliminarTareaDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTareaAEliminar = 1;

        _mockGestorTareas.Setup(g => g.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar));

        _controladorTareas.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar);

        _mockGestorTareas.Verify(g => g.EliminarTareaDelProyecto(idProyecto, usuario, idTareaAEliminar), Times.Once);
    }

    [TestMethod]
    public void ObtenerTareaPorId_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        int idTarea = 1;
        TareaDTO tareaEsperada = new TareaDTO { Id = idTarea };

        _mockGestorTareas.Setup(g => g.ObtenerTareaPorId(idProyecto, idTarea)).Returns(tareaEsperada);

        TareaDTO resultado = _controladorTareas.ObtenerTareaPorId(idProyecto, idTarea);

        Assert.AreEqual(tareaEsperada.Id, resultado.Id);
        _mockGestorTareas.Verify(g => g.ObtenerTareaPorId(idProyecto, idTarea), Times.Once);
    }

    [TestMethod]
    public void ModificarTituloTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        string nuevoTitulo = "Nuevo título";

        _mockGestorTareas.Setup(g => g.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo));

        _controladorTareas.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo);

        _mockGestorTareas.Verify(g => g.ModificarTituloTarea(usuario, idTarea, idProyecto, nuevoTitulo), Times.Once);
    }

    [TestMethod]
    public void ModificarDescripcionTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        string nuevaDescripcion = "Nueva descripción";

        _mockGestorTareas.Setup(g => g.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion));

        _controladorTareas.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion);

        _mockGestorTareas.Verify(g => g.ModificarDescripcionTarea(usuario, idTarea, idProyecto, nuevaDescripcion),
            Times.Once);
    }

    [TestMethod]
    public void ModificarDuracionTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        int nuevaDuracion = 5;

        _mockGestorTareas.Setup(g => g.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion));

        _controladorTareas.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion);

        _mockGestorTareas.Verify(g => g.ModificarDuracionTarea(usuario, idTarea, idProyecto, nuevaDuracion),
            Times.Once);
    }

    [TestMethod]
    public void ModificarFechaInicioTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        DateTime nuevaFecha = DateTime.Today.AddDays(4);

        _mockGestorTareas.Setup(g => g.ModificarFechaInicioTarea(usuario, idTarea, idProyecto, nuevaFecha));

        _controladorTareas.ModificarFechaInicioTarea(usuario, idTarea, idProyecto, nuevaFecha);

        _mockGestorTareas.Verify(g => g.ModificarFechaInicioTarea(usuario, idTarea, idProyecto, nuevaFecha),
            Times.Once);
    }

    [TestMethod]
    public void CambiarEstadoTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        EstadoTareaDTO nuevoEstado = EstadoTareaDTO.EnProceso;

        _mockGestorTareas.Setup(g => g.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado));

        _controladorTareas.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado);

        _mockGestorTareas.Verify(g => g.CambiarEstadoTarea(usuario, idTarea, idProyecto, nuevoEstado), Times.Once);
    }

    [TestMethod]
    public void EsMiembroDeTarea_LLamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;

        _mockGestorTareas.Setup(g => g.EsMiembroDeTarea(usuario, idTarea, idProyecto)).Returns(true);

        bool resultado = _controladorTareas.EsMiembroDeTarea(usuario, idTarea, idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorTareas.Verify(g => g.EsMiembroDeTarea(usuario, idTarea, idProyecto), Times.Once);
    }

    [TestMethod]
    public void AgregarDependenciaATarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idTareaDependencia = 2;
        int idProyecto = 1;
        string tipoDependencia = "FS";

        _mockGestorTareas.Setup(g =>
            g.AgregarDependenciaATarea(usuario, idTarea, idTareaDependencia, idProyecto, tipoDependencia));

        _controladorTareas.AgregarDependenciaATarea(usuario, idTarea, idTareaDependencia, idProyecto, tipoDependencia);

        _mockGestorTareas.Verify(
            g => g.AgregarDependenciaATarea(usuario, idTarea, idTareaDependencia, idProyecto, tipoDependencia),
            Times.Once);
    }

    [TestMethod]
    public void EliminarDependenciaDeTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idTareaDependencia = 2;
        int idProyecto = 1;

        _mockGestorTareas.Setup(g => g.EliminarDependenciaDeTarea(usuario, idTarea, idTareaDependencia, idProyecto));

        _controladorTareas.EliminarDependenciaDeTarea(usuario, idTarea, idTareaDependencia, idProyecto);

        _mockGestorTareas.Verify(g => g.EliminarDependenciaDeTarea(usuario, idTarea, idTareaDependencia, idProyecto),
            Times.Once);
    }

    [TestMethod]
    public void AgregarMiembroATarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        UsuarioDTO nuevoMiembro = new UsuarioDTO { Id = 2 };

        _mockGestorTareas.Setup(g => g.AgregarMiembroATarea(usuario, idTarea, idProyecto, nuevoMiembro));

        _controladorTareas.AgregarMiembroATarea(usuario, idTarea, idProyecto, nuevoMiembro);

        _mockGestorTareas.Verify(g => g.AgregarMiembroATarea(usuario, idTarea, idProyecto, nuevoMiembro), Times.Once);
    }

    [TestMethod]
    public void EliminarMiembroDeTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        UsuarioDTO miembroAEliminar = new UsuarioDTO { Id = 2 };

        _mockGestorTareas.Setup(g => g.EliminarMiembroDeTarea(usuario, idTarea, idProyecto, miembroAEliminar));

        _controladorTareas.EliminarMiembroDeTarea(usuario, idTarea, idProyecto, miembroAEliminar);

        _mockGestorTareas.Verify(g => g.EliminarMiembroDeTarea(usuario, idTarea, idProyecto, miembroAEliminar),
            Times.Once);
    }

    [TestMethod]
    public void ValidarYAsignarRecurso_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        RecursoDTO nuevoRecurso = new RecursoDTO { Id = 1 };
        int cantidad = 1;

        _mockGestorTareas.Setup(g => g.ValidarYAsignarRecurso(usuario, idTarea, idProyecto, nuevoRecurso, cantidad));

        _controladorTareas.ValidarYAsignarRecurso(usuario, idTarea, idProyecto, nuevoRecurso, cantidad);

        _mockGestorTareas.Verify(g => g.ValidarYAsignarRecurso(usuario, idTarea, idProyecto, nuevoRecurso, cantidad), Times.Once);
    }


    [TestMethod]
    public void EliminarRecursoDeTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        RecursoDTO recursoAEliminar = new RecursoDTO { Id = 1 };

        _mockGestorTareas.Setup(g => g.EliminarRecursoDeTarea(usuario, idTarea, idProyecto, recursoAEliminar));

        _controladorTareas.EliminarRecursoDeTarea(usuario, idTarea, idProyecto, recursoAEliminar);

        _mockGestorTareas.Verify(g => g.EliminarRecursoDeTarea(usuario, idTarea, idProyecto, recursoAEliminar),
            Times.Once);
    }
    
    [TestMethod]
    public void EncontrarRecursosAlternativosMismoTipo_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idProyecto = 1;
        RecursoDTO recursoOriginal = new RecursoDTO { Id = 10 };
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(5);
        int cantidad = 3;

        _mockGestorTareas.Setup(g => g.EncontrarRecursosAlternativosMismoTipo(usuario, idProyecto, recursoOriginal, fechaInicio, fechaFin, cantidad));

        _controladorTareas.EncontrarRecursosAlternativosMismoTipo(usuario, idProyecto, recursoOriginal, fechaInicio, fechaFin, cantidad);

        _mockGestorTareas.Verify(g => g.EncontrarRecursosAlternativosMismoTipo(usuario, idProyecto, recursoOriginal, fechaInicio, fechaFin, cantidad), Times.Once);
    }

    [TestMethod]
    public void ReprogramarTarea_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idProyecto = 1;
        int idTarea = 2;
        RecursoDTO recurso = new RecursoDTO { Id = 5 };
        int cantidad = 2;

        _mockGestorTareas.Setup(g => g.ReprogramarTarea(usuario, idProyecto, idTarea, recurso, cantidad));

        _controladorTareas.ReprogramarTarea(usuario, idProyecto, idTarea, recurso, cantidad);

        _mockGestorTareas.Verify(g => g.ReprogramarTarea(usuario, idProyecto, idTarea, recurso, cantidad), Times.Once);
    }

    
    [TestMethod]
    public void ForzarAsignacion_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idTarea = 1;
        int idProyecto = 1;
        RecursoDTO recurso = new RecursoDTO { Id = 4 };
        int cantidad = 1;

        _mockGestorTareas.Setup(g => g.ForzarAsignacion(usuario, idTarea, idProyecto, recurso, cantidad));

        _controladorTareas.ForzarAsignacion(usuario, idTarea, idProyecto, recurso, cantidad);

        _mockGestorTareas.Verify(g => g.ForzarAsignacion(usuario, idTarea, idProyecto, recurso, cantidad), Times.Once);
    }

}