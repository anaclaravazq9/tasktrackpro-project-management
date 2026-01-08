using Dominio;
using Repositorios;
using IRepositorios;
using Servicios.Notificaciones;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class NotificadorTests
{
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    private IRepositorioUsuarios _repositorioUsuarios;
    
    private Notificador _notificador;

    [TestInitialize]
    public void Setup()
    {
        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _notificador = new Notificador(_repositorioUsuarios);
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void Constructor_CreaNotificadorValido()
    {
        Assert.IsNotNull(_notificador);
    }
    
    [TestMethod]
    public void NotificarUno_NotificaCorrectamente()
    {
        Usuario usuario = new Usuario("Juan", "Perez", new DateTime(1999, 10, 19), "juanperez@adinet.com.uy",
            "hashxxx12389");
        _repositorioUsuarios.Agregar(usuario);
        
        _notificador.NotificarUno(usuario, "Mensaje de prueba");
        
        Notificacion notificacion = usuario.Notificaciones.Last();
        Assert.AreEqual("Mensaje de prueba", notificacion.Mensaje);
    }
    
    [TestMethod]
    public void NotificarMuchos_NotificaCorrectamente()
    {
        Usuario usuario1 = new Usuario("Juan", "Perez", new DateTime(1999, 10, 19), "juanperez@adinet.com.uy",
            "hashxxx12389");
        Usuario usuario2 = new Usuario("José", "Perez", new DateTime(1999, 10, 19), "joseperez@adinet.com.uy",
            "hashxxx12389");
        _repositorioUsuarios.Agregar(usuario1);
        _repositorioUsuarios.Agregar(usuario2);
        
        _notificador.NotificarMuchos(new List<Usuario>{ usuario1, usuario2 }, "Mensaje de prueba");
        
        Notificacion notificacion1 = usuario1.Notificaciones.Last();
        Notificacion notificacion2 = usuario2.Notificaciones.Last();
        Assert.AreEqual("Mensaje de prueba", notificacion1.Mensaje);
        Assert.AreEqual("Mensaje de prueba", notificacion2.Mensaje);
    }
}