using Dominio;
using Repositorios;
using Microsoft.EntityFrameworkCore;
using Tests.Contexto;
using Utilidades;

namespace Tests.RepositoriosTests;

[TestClass]
public class RepositorioUsuariosTests
{
    private RepositorioUsuarios _repositorioUsuarios;
    private SqlContext _contexto;
    private Usuario _usuario;

    [TestInitialize]
    public void Setup()
    {
        _contexto = SqlContextFactory.CrearContextoEnMemoria();

        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _usuario = new Usuario("Juan", "Pérez", new DateTime(1998, 7, 6), "unEmail@gmail.com", "uNaC@ntr4seña");
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void SeAgregaUnUsuarioOk()
    {
        _repositorioUsuarios.Agregar(_usuario);
        Usuario ultimoDelRepositorioUsuario = _repositorioUsuarios.ObtenerPorId(_usuario.Id);
        Assert.AreEqual(_usuario, ultimoDelRepositorioUsuario);
    }

    [TestMethod]
    public void SeAsignanIdsOk()
    {
        //primero se agrega al admin de sistema (seed) con Id 1
        _repositorioUsuarios.Agregar(_usuario);
        Usuario usuario2 = new Usuario("Mateo", "Pérez", new DateTime(1999, 2, 2), "email@gmail.com", "contr5Ase{a");
        _repositorioUsuarios.Agregar(usuario2);

        Assert.AreEqual(2, _usuario.Id);
        Assert.AreEqual(3, usuario2.Id);
    }

    [TestMethod]
    public void SeEliminaUnUsuarioOk()
    {
        _usuario.Id = 2;
        _repositorioUsuarios.Agregar(_usuario);
        _repositorioUsuarios.Eliminar(_usuario.Id);
        Assert.IsNull(_repositorioUsuarios.ObtenerPorId(2));
    }

    [TestMethod]
    public void SeObtieneListaDelRepoOk()
    {
        _repositorioUsuarios.Agregar(_usuario);
        List<Usuario> usuarios = _repositorioUsuarios.ObtenerTodos();
        Assert.IsNotNull(usuarios);
        Assert.AreEqual(2, usuarios.Count);
        Assert.AreEqual("Juan", usuarios.Last().Nombre);
    }

    [TestMethod]
    public void SePuedeObtenerUsuariosPorEmail()
    {
        Usuario admin = _repositorioUsuarios.ObtenerPorId(1);
        Usuario obtenidoPorEmail = _repositorioUsuarios.ObtenerUsuarioPorEmail("admin@sistema.com");
        Assert.AreEqual(admin, obtenidoPorEmail);
    }
    
    [TestMethod]
    public void SeActualizaEmailContrasenaYRolesDelUsuarioOK()
    {
        _repositorioUsuarios.Agregar(_usuario);
        
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("Admin123$");
        Usuario usuarioModificado = new Usuario("Pedro", "Rodriguez", new DateTime(1990, 1, 1), "nuevo@email.com", contrasenaEncriptada)
        {
            Id = _usuario.Id,
            EsAdministradorSistema = true,
            EsAdministradorProyecto = true,
            EstaAdministrandoUnProyecto = true,
            CantidadProyectosAsignados = 3
        };
        
        _repositorioUsuarios.Actualizar(usuarioModificado);

        Usuario usuarioActualizado = _repositorioUsuarios.ObtenerPorId(_usuario.Id);
        Assert.AreEqual("nuevo@email.com", usuarioActualizado.Email);
        Assert.IsTrue(usuarioActualizado.EsAdministradorSistema);
        Assert.IsTrue(usuarioActualizado.EsAdministradorProyecto);
        Assert.IsTrue(usuarioActualizado.EstaAdministrandoUnProyecto);
        Assert.AreEqual(3, usuarioActualizado.CantidadProyectosAsignados);
        Assert.IsTrue(usuarioActualizado.Autenticar("Admin123$"));
    }
    
    [TestMethod]
    public void SeActualizanNotificacionesDelUsuarioOK()
    {
        _usuario.RecibirNotificacion("Duplicada");
        _usuario.RecibirNotificacion("Original");
        _repositorioUsuarios.Agregar(_usuario);
        
        Usuario usuarioModificado = new Usuario("Juan", "Pérez", new DateTime(1985, 5, 5), "juan@email.com", "hash")
        {
            Id = _usuario.Id
        };
        usuarioModificado.RecibirNotificacion("Nueva");
        usuarioModificado.Notificaciones.Add(_usuario.Notificaciones.First());
        
        _repositorioUsuarios.Actualizar(usuarioModificado);

        Usuario usuarioActualizado = _repositorioUsuarios.ObtenerPorId(_usuario.Id);
        Assert.AreEqual(2, usuarioActualizado.Notificaciones.Count());
        Assert.AreEqual("Nueva", usuarioActualizado.Notificaciones.Last().Mensaje);
    }

}