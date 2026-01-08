using Controladores;
using Dominio;
using DTOs;
using IServicios.IGestores;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorProyectosTests
{
    private Mock<IGestorProyectos> _mockGestorProyectos;
    private ControladorProyectos _controladorProyectos;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorProyectos = new Mock<IGestorProyectos>();
        _controladorProyectos = new ControladorProyectos(_mockGestorProyectos.Object);
    }

    private UsuarioDTO CrearSolicitante(int id = 1) => new UsuarioDTO { Id = id };

    private Usuario CrearUsuario(int id = 1, string email = "admin@test.com")
        => new Usuario("Nombre", "Apellido", DateTime.Today.AddYears(-20), email, "Pass123$") { Id = id };

    private Proyecto CrearProyectoDominio(string nombre = "P1", Usuario admin = null, List<Usuario> miembros = null)
        => new Proyecto(
            nombre,
            "desc",
            DateTime.Today.AddYears(1),
            admin ?? new Usuario(),
            miembros ?? new List<Usuario>());

    private ProyectoDTO CrearProyectoDTO(int id = 1, string nombre = "Proyecto Test")
        => new ProyectoDTO { Id = id, Nombre = nombre };

    private List<ProyectoDTO> CrearListaProyectoDTO()
    {
        return new List<ProyectoDTO>
        {
            new ProyectoDTO { Id = 1, Nombre = "Proyecto A" },
            new ProyectoDTO { Id = 2, Nombre = "Proyecto B" }
        };
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorProyectos);
    }


    [TestMethod]
    public void CrearProyecto_LlamaCorrectamenteAlGestor()
    {
        UsuarioDTO solicitante = CrearSolicitante();
        ProyectoDTO nuevoProyecto = CrearProyectoDTO();

        _mockGestorProyectos.Setup(g => g.CrearProyecto(nuevoProyecto, solicitante));

        _controladorProyectos.CrearProyecto(nuevoProyecto, solicitante);

        _mockGestorProyectos.Verify(g => g.CrearProyecto(nuevoProyecto, solicitante), Times.Once);
    }

    [TestMethod]
    public void EliminarProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO solicitante = CrearSolicitante();

        _mockGestorProyectos.Setup(g => g.EliminarProyecto(idProyecto, solicitante));

        _controladorProyectos.EliminarProyecto(idProyecto, solicitante);

        _mockGestorProyectos.Verify(g => g.EliminarProyecto(idProyecto, solicitante), Times.Once);
    }

    [TestMethod]
    public void ObtenerTodos_LlamaCorrectamenteAGestorYDevuelveLista()
    {
        var listaEsperada = CrearListaProyectoDTO();

        _mockGestorProyectos.Setup(g => g.ObtenerTodos()).Returns(listaEsperada);

        List<ProyectoDTO> resultado = _controladorProyectos.ObtenerTodos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Proyecto A", resultado[0].Nombre);
        Assert.AreEqual("Proyecto B", resultado[1].Nombre);
        _mockGestorProyectos.Verify(g => g.ObtenerTodos(), Times.Once);
    }

    [TestMethod]
    public void ObtenerProyectoPorId_LlamaCorrectamenteAGestorYDevuelveProyecto()
    {
        int idProyecto = 1;
        ProyectoDTO proyectoEsperado = CrearProyectoDTO(idProyecto, "Proyecto Test");

        _mockGestorProyectos.Setup(g => g.ObtenerProyectoPorId(idProyecto)).Returns(proyectoEsperado);

        ProyectoDTO resultado = _controladorProyectos.ObtenerProyectoPorId(idProyecto);

        Assert.AreEqual(idProyecto, resultado.Id);
        Assert.AreEqual("Proyecto Test", resultado.Nombre);
        _mockGestorProyectos.Verify(g => g.ObtenerProyectoPorId(idProyecto), Times.Once);
    }

    [TestMethod]
    public void ModificarNombreDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO solicitante = CrearSolicitante();
        string nuevoNombre = "Nuevo Proyecto";

        _mockGestorProyectos.Setup(g => g.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante));

        _controladorProyectos.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarNombreDelProyecto(idProyecto, nuevoNombre, solicitante),
            Times.Once);
    }

    [TestMethod]
    public void ModificarDescripcionDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO solicitante = CrearSolicitante();
        string nuevaDescripcion = "Descripción actualizada";

        _mockGestorProyectos.Setup(g => g.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante));

        _controladorProyectos.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarDescripcionDelProyecto(idProyecto, nuevaDescripcion, solicitante),
            Times.Once);
    }

    [TestMethod]
    public void ModificarFechaDeInicioDelProyecto_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        UsuarioDTO solicitante = CrearSolicitante();
        DateTime nuevaFecha = DateTime.Today.AddDays(5);

        _mockGestorProyectos.Setup(g => g.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante));

        _controladorProyectos.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante);

        _mockGestorProyectos.Verify(g => g.ModificarFechaDeInicioDelProyecto(idProyecto, nuevaFecha, solicitante),
            Times.Once);
    }

    [TestMethod]
    public void CambiarAdministradorDeProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = CrearSolicitante();
        int idProyecto = 10;
        int idNuevoAdmin = 99;

        _mockGestorProyectos.Setup(g => g.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin));

        _controladorProyectos.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin);

        _mockGestorProyectos.Verify(g => g.CambiarAdministradorDeProyecto(solicitante, idProyecto, idNuevoAdmin),
            Times.Once);
    }

    [TestMethod]
    public void AgregarMiembroAProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = CrearSolicitante();
        UsuarioDTO nuevoMiembro = new UsuarioDTO { Id = 2 };
        int idProyecto = 5;

        _mockGestorProyectos.Setup(g => g.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro));

        _controladorProyectos.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro);

        _mockGestorProyectos.Verify(g => g.AgregarMiembroAProyecto(idProyecto, solicitante, nuevoMiembro), Times.Once);
    }

    [TestMethod]
    public void EliminarMiembroDelProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = CrearSolicitante();
        int idProyecto = 3;
        int idMiembro = 4;

        _mockGestorProyectos.Setup(g => g.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro));

        _controladorProyectos.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro);

        _mockGestorProyectos.Verify(g => g.EliminarMiembroDelProyecto(idProyecto, solicitante, idMiembro), Times.Once);
    }

    [TestMethod]
    public void ObtenerProyectosPorUsuario_LlamaCorrectamenteAGestorYDevuelveLista()
    {
        int idUsuario = 1;
        List<ProyectoDTO> proyectosEsperados = CrearListaProyectoDTO();

        _mockGestorProyectos.Setup(g => g.ObtenerProyectosPorUsuario(idUsuario)).Returns(proyectosEsperados);

        List<ProyectoDTO> resultado = _controladorProyectos.ObtenerProyectosPorUsuario(idUsuario);

        Assert.AreEqual(2, resultado.Count);
        _mockGestorProyectos.Verify(g => g.ObtenerProyectosPorUsuario(idUsuario), Times.Once);
    }

    [TestMethod]
    public void VerificarUsuarioNoTieneTareasAsignadas_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 1;
        int idUsuario = 2;

        _mockGestorProyectos.Setup(g => g.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario));

        _controladorProyectos.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario);

        _mockGestorProyectos.Verify(g => g.VerificarUsuarioNoTieneTareasAsignadas(idProyecto, idUsuario), Times.Once);
    }

    [TestMethod]
    public void CalcularCaminoCritico_LlamaCorrectamenteAGestor()
    {
        ProyectoDTO proyecto = new ProyectoDTO { Id = 1 };

        _mockGestorProyectos.Setup(g => g.CalcularCaminoCritico(proyecto));

        _controladorProyectos.CalcularCaminoCritico(proyecto);

        _mockGestorProyectos.Verify(g => g.CalcularCaminoCritico(proyecto), Times.Once);
    }

    [TestMethod]
    public void EsAdministradorDeProyecto_LlamaCorrectamenteAGestorYDevuelveTrue()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idProyecto = 3;

        _mockGestorProyectos.Setup(g => g.EsAdministradorDeProyecto(usuario, idProyecto)).Returns(true);

        bool resultado = _controladorProyectos.EsAdministradorDeProyecto(usuario, idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorProyectos.Verify(g => g.EsAdministradorDeProyecto(usuario, idProyecto), Times.Once);
    }

    [TestMethod]
    public void EsMiembroDeProyecto_LlamaCorrectamenteAGestorYDevuelveTrue()
    {
        int idUsuario = 1;
        int idProyecto = 2;

        _mockGestorProyectos.Setup(g => g.EsMiembroDeProyecto(idUsuario, idProyecto)).Returns(true);

        bool resultado = _controladorProyectos.EsMiembroDeProyecto(idUsuario, idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorProyectos.Verify(g => g.EsMiembroDeProyecto(idUsuario, idProyecto), Times.Once);
    }
    
    [TestMethod]
    public void AsignarLider_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 42;
        UsuarioDTO solicitante = CrearSolicitante();
        int idNuevoLider = 99;

        _mockGestorProyectos.Setup(g => g.AsignarLider(idProyecto, solicitante, idNuevoLider));

        _controladorProyectos.AsignarLider(idProyecto, solicitante, idNuevoLider);

        _mockGestorProyectos.Verify(g => g.AsignarLider(idProyecto, solicitante, idNuevoLider), Times.Once);
    }
    
    [TestMethod]
    public void EsLiderDeProyecto_LlamaCorrectamenteAGestorYDevuelveTrue()
    {
        UsuarioDTO usuario = new UsuarioDTO { Id = 1 };
        int idProyecto = 3;

        _mockGestorProyectos.Setup(g => g.EsLiderDeProyecto(usuario, idProyecto)).Returns(true);

        bool resultado = _controladorProyectos.EsLiderDeProyecto(usuario, idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorProyectos.Verify(g => g.EsLiderDeProyecto(usuario, idProyecto), Times.Once);
    }
    
    [TestMethod]
    public void DesasignarLider_LlamaCorrectamenteAGestor()
    {
        int idProyecto = 10;
        UsuarioDTO solicitante = CrearSolicitante();

        _mockGestorProyectos.Setup(g => g.DesasignarLider(idProyecto, solicitante));

        _controladorProyectos.DesasignarLider(idProyecto, solicitante);

        _mockGestorProyectos.Verify(g => g.DesasignarLider(idProyecto, solicitante), Times.Once);
    }

    [TestMethod]
    public void ExisteLiderEnProyecto_LlamaCorrectamenteAGestorYDevuelveTrue()
    {
        int idProyecto = 5;

        _mockGestorProyectos.Setup(g => g.ExisteLiderEnProyecto(idProyecto)).Returns(true);

        bool resultado = _controladorProyectos.ExisteLiderEnProyecto(idProyecto);

        Assert.IsTrue(resultado);
        _mockGestorProyectos.Verify(g => g.ExisteLiderEnProyecto(idProyecto), Times.Once);
    }

}