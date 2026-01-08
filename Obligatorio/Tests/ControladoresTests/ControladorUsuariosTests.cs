using Controladores;
using DTOs;
using IServicios.IGestores;
using Moq;

namespace Tests.ControladoresTests;

[TestClass]
public class ControladorUsuariosTests
{
    private Mock<IGestorUsuarios> _mockGestorUsuarios;
    private ControladorUsuarios _controladorUsuarios;

    [TestInitialize]
    public void Setup()
    {
        _mockGestorUsuarios = new Mock<IGestorUsuarios>();
        _controladorUsuarios = new ControladorUsuarios(_mockGestorUsuarios.Object);
    }

    [TestMethod]
    public void Constructor_CreaControladorOk()
    {
        Assert.IsNotNull(_controladorUsuarios);
    }

    [TestMethod]
    public void CrearYAgregarUsuario_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        UsuarioDTO nuevoUsuario = new UsuarioDTO { Id = 2 };

        _mockGestorUsuarios.Setup(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario));

        _controladorUsuarios.CrearYAgregarUsuario(solicitante, nuevoUsuario);

        _mockGestorUsuarios.Verify(g => g.CrearYAgregarUsuario(solicitante, nuevoUsuario), Times.Once);
    }

    [TestMethod]
    public void EliminarUsuario_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int id = 1;

        _mockGestorUsuarios.Setup(g => g.EliminarUsuario(solicitante, id));

        _controladorUsuarios.EliminarUsuario(solicitante, id);

        _mockGestorUsuarios.Verify(g => g.EliminarUsuario(solicitante, id), Times.Once);
    }

    [TestMethod]
    public void ObtenerTodos_LlamaCorrectamenteAGestor()
    {
        var listaEsperada = new List<UsuarioListarDTO>
        {
            new UsuarioListarDTO { Id = 1, Nombre = "Usuario A" },
            new UsuarioListarDTO { Id = 2, Nombre = "Usuario B" }
        };

        _mockGestorUsuarios.Setup(g => g.ObtenerTodos()).Returns(listaEsperada);

        List<UsuarioListarDTO> resultado = _controladorUsuarios.ObtenerTodos();

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual("Usuario A", resultado[0].Nombre);
        Assert.AreEqual("Usuario B", resultado[1].Nombre);
        _mockGestorUsuarios.Verify(g => g.ObtenerTodos(), Times.Once);
    }

    [TestMethod]
    public void ObtenerUsuarioPorId_LlamaCorrectamenteAGestor()
    {
        int idUsuario = 1;
        UsuarioDTO usuarioEsperado = new UsuarioDTO { Id = idUsuario };

        _mockGestorUsuarios.Setup(g => g.ObtenerUsuarioPorId(idUsuario)).Returns(usuarioEsperado);

        UsuarioDTO resultado = _controladorUsuarios.ObtenerUsuarioPorId(idUsuario);

        Assert.AreEqual(usuarioEsperado.Id, resultado.Id);
        _mockGestorUsuarios.Verify(g => g.ObtenerUsuarioPorId(idUsuario), Times.Once);
    }

    [TestMethod]
    public void AgregarAdministradorSistema_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;

        _mockGestorUsuarios.Setup(g => g.AgregarAdministradorSistema(solicitante, nuevoAdmin));

        _controladorUsuarios.AgregarAdministradorSistema(solicitante, nuevoAdmin);

        _mockGestorUsuarios.Verify(g => g.AgregarAdministradorSistema(solicitante, nuevoAdmin), Times.Once);
    }

    [TestMethod]
    public void AsignarAdministradorProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;

        _mockGestorUsuarios.Setup(g => g.AsignarAdministradorProyecto(solicitante, nuevoAdmin));

        _controladorUsuarios.AsignarAdministradorProyecto(solicitante, nuevoAdmin);

        _mockGestorUsuarios.Verify(g => g.AsignarAdministradorProyecto(solicitante, nuevoAdmin), Times.Once);
    }

    [TestMethod]
    public void DesasignarAdministradorProyecto_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int nuevoAdmin = 2;

        _mockGestorUsuarios.Setup(g => g.DesasignarAdministradorProyecto(solicitante, nuevoAdmin));

        _controladorUsuarios.DesasignarAdministradorProyecto(solicitante, nuevoAdmin);

        _mockGestorUsuarios.Verify(g => g.DesasignarAdministradorProyecto(solicitante, nuevoAdmin), Times.Once);
    }

    [TestMethod]
    public void ReiniciarContrasena_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int idObjetivo = 2;

        _mockGestorUsuarios.Setup(g => g.ReiniciarContrasena(solicitante, idObjetivo));

        _controladorUsuarios.ReiniciarContrasena(solicitante, idObjetivo);

        _mockGestorUsuarios.Verify(g => g.ReiniciarContrasena(solicitante, idObjetivo), Times.Once);
    }

    [TestMethod]
    public void AutogenerarContrasenaValida_LlamaCorrectamenteAGestor()
    {
        _mockGestorUsuarios.Setup(g => g.AutogenerarContrasenaValida());

        _controladorUsuarios.AutogenerarContrasenaValida();

        _mockGestorUsuarios.Verify(g => g.AutogenerarContrasenaValida(), Times.Once);
    }


    [TestMethod]
    public void AutogenerarYAsignarContrasena_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int idObjetivo = 2;

        _mockGestorUsuarios.Setup(g => g.AutogenerarYAsignarContrasena(solicitante, idObjetivo));

        _controladorUsuarios.AutogenerarYAsignarContrasena(solicitante, idObjetivo);

        _mockGestorUsuarios.Verify(g => g.AutogenerarYAsignarContrasena(solicitante, idObjetivo), Times.Once);
    }

    [TestMethod]
    public void ModificarContrasena_LlamaCorrectamenteAGestor()
    {
        UsuarioDTO solicitante = new UsuarioDTO { Id = 1 };
        int idObjetivo = 2;
        string nuevaPass = "Contra123!";

        _mockGestorUsuarios.Setup(g => g.ModificarContrasena(solicitante, idObjetivo, nuevaPass));

        _controladorUsuarios.ModificarContrasena(solicitante, idObjetivo, nuevaPass);

        _mockGestorUsuarios.Verify(g => g.ModificarContrasena(solicitante, idObjetivo, nuevaPass), Times.Once);
    }

    [TestMethod]
    public void BorrarNotificacion_LlamaCorrectamenteAGestor()
    {
        int idUsuario = 1;
        int idNotificacion = 99;

        _mockGestorUsuarios.Setup(g => g.BorrarNotificacion(idUsuario, idNotificacion));

        _controladorUsuarios.BorrarNotificacion(idUsuario, idNotificacion);

        _mockGestorUsuarios.Verify(g => g.BorrarNotificacion(idUsuario, idNotificacion), Times.Once);
    }

    [TestMethod]
    public void LogIn_LlamaCorrectamenteAGestorYDevuelveUsuario()
    {
        string email = "usuario@correo.com";
        string contrasena = "Contra123!";
        UsuarioDTO usuarioEsperado = new UsuarioDTO { Id = 1, Email = email };

        _mockGestorUsuarios.Setup(g => g.LogIn(email, contrasena)).Returns(usuarioEsperado);

        var resultado = _controladorUsuarios.LogIn(email, contrasena);

        Assert.IsNotNull(resultado);
        Assert.AreEqual(usuarioEsperado.Id, resultado.Id);
        Assert.AreEqual(usuarioEsperado.Email, resultado.Email);
        _mockGestorUsuarios.Verify(g => g.LogIn(email, contrasena), Times.Once);
    }

    [TestMethod]
    public void ObtenerUsuariosDiferentes_LlamaCorrectamenteAGestorYDevuelveLista()
    {
        var entrada = new List<UsuarioListarDTO>
        {
            new UsuarioListarDTO { Id = 1 },
            new UsuarioListarDTO { Id = 2 }
        };

        var esperada = new List<UsuarioListarDTO>
        {
            new UsuarioListarDTO { Id = 3 },
            new UsuarioListarDTO { Id = 4 }
        };

        _mockGestorUsuarios.Setup(g => g.ObtenerUsuariosDiferentes(entrada)).Returns(esperada);

        var resultado = _controladorUsuarios.ObtenerUsuariosDiferentes(entrada);

        Assert.AreEqual(2, resultado.Count);
        Assert.AreEqual(3, resultado[0].Id);
        Assert.AreEqual(4, resultado[1].Id);
        _mockGestorUsuarios.Verify(g => g.ObtenerUsuariosDiferentes(entrada), Times.Once);
    }

    [TestMethod]
    public void ValidarUsuarioNoEsAdministradorInicial_LlamaCorrectamenteAGestor()
    {
        int idUsuario = 5;

        _mockGestorUsuarios.Setup(g => g.ValidarUsuarioNoEsAdministradorInicial(idUsuario));

        _controladorUsuarios.ValidarUsuarioNoEsAdministradorInicial(idUsuario);

        _mockGestorUsuarios.Verify(g => g.ValidarUsuarioNoEsAdministradorInicial(idUsuario), Times.Once);
    }
    
    [TestMethod]
    public void AutogenerarContrasenaValida_DevuelveContrasenaGenerada()
    {
        string contrasenaEsperada = "Clave123!";

        _mockGestorUsuarios.Setup(g => g.AutogenerarContrasenaValida()).Returns(contrasenaEsperada);

        string resultado = _controladorUsuarios.AutogenerarContrasenaValida();

        Assert.AreEqual(contrasenaEsperada, resultado);
        _mockGestorUsuarios.Verify(g => g.AutogenerarContrasenaValida(), Times.Once);
    }

}