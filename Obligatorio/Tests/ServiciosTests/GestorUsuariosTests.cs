using Dominio;
using DTOs;
using Excepciones;
using Repositorios;
using Servicios.Gestores;
using Servicios.Notificaciones;
using Tests.Contexto;

namespace Tests.ServiciosTests;

[TestClass]
public class GestorUsuariosTests
{
    private Notificador _notificador;
    
    private SqlContext _contexto = SqlContextFactory.CrearContextoEnMemoria();
    private RepositorioUsuarios _repositorioUsuarios;
    private GestorUsuarios _gestorUsuarios;
    
    private UsuarioDTO _adminSistemaDTO;
    
    [TestInitialize]
    public void SetUp()
    {
        _repositorioUsuarios = new RepositorioUsuarios(_contexto);
        _notificador = new Notificador(_repositorioUsuarios);
        _gestorUsuarios = new GestorUsuarios(_repositorioUsuarios, _notificador);
        
        _adminSistemaDTO = UsuarioDTO.DesdeEntidad(_gestorUsuarios.AdministradorInicial);
    }

    private UsuarioDTO CrearUsuarioDTO(string nombre, string apellido, string email, string contrasena)
    {
        return new UsuarioDTO()
        {
            Nombre = nombre, Apellido = apellido, FechaNacimiento = new DateTime(2007, 4, 28), Email = email,
            Contrasena = contrasena
        };
    }

    private UsuarioDTO CrearYAsignarAdminSistema()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail77@adinet.com.uy", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.AgregarAdministradorSistema(_adminSistemaDTO, usuario.Id);
        return usuario;
    }
    
    [TestCleanup]
    public void Cleanup()
    {
        _contexto.Database.EnsureDeleted();
        _contexto.Dispose();
    }

    [TestMethod]
    public void ConstructorCreaGestorValido()
    {
        Assert.IsNotNull(_gestorUsuarios);
        Assert.AreEqual(1, _gestorUsuarios.ObtenerTodos().Count); // se crea solo con administrador
        Assert.AreEqual("Admin", _gestorUsuarios.ObtenerTodos().Last().Nombre);
    }

    [TestMethod]
    public void GestorCreaYAgregaUsuariosCorrectamente()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "gmail@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail2@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        Assert.AreEqual(3, _gestorUsuarios.ObtenerTodos().Count);
        Assert.AreEqual(usuario1.Id, _gestorUsuarios.ObtenerTodos().ElementAt(1).Id);
        Assert.AreEqual(usuario2.Id, _gestorUsuarios.ObtenerTodos().ElementAt(2).Id);
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void ErrorSiSeCreaUsuarioConEmailRepetido()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail3@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail3@gmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeAgregarUsuario()
    {
        UsuarioDTO solicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail5@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, solicitante);
        UsuarioDTO nuevoUsuario = CrearUsuarioDTO("Mateo", "Pérez", "unemail6@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(solicitante, nuevoUsuario);
    }

    [TestMethod]
    public void GestorEliminaUsuariosCorrectamente()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail7@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail8@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario2.Id);
        Assert.AreEqual(2, _gestorUsuarios.ObtenerTodos().Count); //se cuenta al admin inicial
        Assert.AreEqual(usuario1.Id, _gestorUsuarios.ObtenerTodos().ElementAt(1).Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void GestorNoEliminaPrimerAdministradorSistema()
    {
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, 1);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminDeSistemaNoPuedeEliminarUsuario()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail9@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail10@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
        _gestorUsuarios.EliminarUsuario(usuario1, usuario2.Id);
    }

    [TestMethod]
    public void UsuarioSePuedeEliminarASiMismo()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail11@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(usuario, usuario.Id);
    }

    [TestMethod]
    public void GestorDevuelveUnUsuarioPorId()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail12@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail13@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        UsuarioDTO busqueda = _gestorUsuarios.ObtenerUsuarioPorId(usuario1.Id);
        Assert.AreEqual(usuario1.Id, busqueda.Id);
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void ErrorSiSeBuscaUnUsuarioNoRegistrado()
    {
        UsuarioDTO busqueda = _gestorUsuarios.ObtenerUsuarioPorId(4);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeSistema()
    {
        UsuarioDTO usuario = CrearYAsignarAdminSistema();
        usuario = _gestorUsuarios.ObtenerUsuarioPorId(usuario.Id); // actualización
        Assert.IsTrue(usuario.EsAdministradorSistema);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void SoloUnAdminDeSistemaPuedeAsignarOtro()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "unemail14@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "unemail15@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);

        _gestorUsuarios.AgregarAdministradorSistema(usuario1, usuario2.Id);
    }

    [TestMethod]
    public void GestorAsignaAdministradorDeProyectoCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail16@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);

        nuevoAdminProyecto = _gestorUsuarios.ObtenerUsuarioPorId(nuevoAdminProyecto.Id); // actualización
        Assert.IsTrue(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeAsignarAdministradorProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail17@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail18@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [TestMethod]
    public void GestorEliminaAdministradorDeProyectoCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail19@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);

        Assert.IsFalse(nuevoAdminProyecto.EsAdministradorProyecto);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNoPuedeEliminarAdministradorProyecto()
    {
        UsuarioDTO adminSistema = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail20@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(adminSistema, nuevoAdminProyecto.Id);

        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("José", "Pérez", "unemail21@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioNoEsAdministradorProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail22@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ErrorEliminarAdministradorProyectoSiUsuarioEstaAdministrandoUnProyecto()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO nuevoAdminProyecto = CrearUsuarioDTO("Mateo", "Pérez", "unemail23@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, nuevoAdminProyecto);
        _gestorUsuarios.AsignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);

        Usuario nuevoAdmin =
            _repositorioUsuarios.ObtenerPorId(nuevoAdminProyecto.Id); // esto lo gestiona el gestor de proyectos
        nuevoAdmin.EstaAdministrandoUnProyecto = true;

        _gestorUsuarios.DesasignarAdministradorProyecto(usuarioSolicitante, nuevoAdminProyecto.Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ErrorEliminarUsuarioMiembroDeProyecto()
    {
        UsuarioDTO adminDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail23@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, adminDTO);
        adminDTO.EsAdministradorProyecto = true;

        Usuario admin = _repositorioUsuarios.ObtenerPorId(adminDTO.Id);
        Proyecto proyecto =
            new Proyecto("Proyecto", "descripción", DateTime.Today.AddDays(1), admin, new List<Usuario>());

        _gestorUsuarios.EliminarUsuario(adminDTO, adminDTO.Id);
    }

    [TestMethod]
    public void AdminSistemaReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail24@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void AdminProyectoReiniciaLaContraseñaDeUnUsuarioCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail25@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail26@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar("TaskTrackPro@2025"));
    }

    [TestMethod]
    public void UnUsuarioPuedeReiniciarSuContraseñaCorrectamente()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail27@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        _gestorUsuarios.ReiniciarContrasena(usuarioDTO, usuarioDTO.Id);
        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsTrue(usuario.Autenticar("TaskTrackPro@2025"));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeReiniciarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail28@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail29@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);
        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaAutogeneraUnaContraseñaCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = new UsuarioDTO()
        {
            Nombre = "José", Apellido = "Perez", FechaNacimiento = new DateTime(1999, 9, 1),
            Email = "unemail@gmail.com", Contrasena = "Contrase#a9"
        };
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.AutogenerarYAsignarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena =
            ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void AdminProyectoAutogeneraUnaContraseñaCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail30@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail31@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.AutogenerarYAsignarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena =
            ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");
        Assert.IsFalse(usuarioObjetivo.Autenticar("Contrase#a9"));
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminDeSistemaNiDeProyectoPuedeAutogenerarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail32@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivo = CrearUsuarioDTO("Mateo", "Pérez", "unemail33@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivo);

        _gestorUsuarios.AutogenerarYAsignarContrasena(usuarioSolicitante, usuarioObjetivo.Id);
    }

    [TestMethod]
    public void AdminSistemaPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail34@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id, nuevaContrasena);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void AdminProyectoPuedeModificarContrasenaDeUsuarioCorrectamente()
    {
        UsuarioDTO administrador = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Mateo", "Pérez", "unemail35@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        _gestorUsuarios.AsignarAdministradorProyecto(administrador, usuarioSolicitante.Id);

        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("José", "Pérez", "unemail36@gmail.com", "Contrase#a9");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id, nuevaContrasena);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
    }

    [TestMethod]
    public void UsuarioPuedeModificarSuContrasena()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail37@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);

        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void NoAdminSistemaNiAdminProyectoNoPuedeModificarContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearUsuarioDTO("Juan", "Pérez", "unemail38@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioSolicitante);
        UsuarioDTO usuarioObjetivo = CrearUsuarioDTO("Mateo", "Pérez", "unemail39@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivo);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivo.Id, nuevaContrasena);
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void DaErrorSiSeCambiaContrasenaInvalida()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail40@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        string nuevaContrasena = "c1.A";
        _gestorUsuarios.ModificarContrasena(usuario, usuario.Id, nuevaContrasena);
    }

    [TestMethod]
    public void NoSeCambiaContrasenaInvalida()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail41@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena = "c1.A";
        try
        {
            _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);
        }
        catch
        {
        } // Ignorar la excepción

        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.IsFalse(usuario.Autenticar(nuevaContrasena));
        Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
    }

    [TestMethod]
    public void SeNotificaReinicioDeContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail42@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.ReiniciarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.ContrasenaReiniciada("TaskTrackPro@2025");

        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeNotificaContrasenaAutogenerada()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail43@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        _gestorUsuarios.AutogenerarYAsignarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string nuevaContrasena =
            ultimaNotificacion.Mensaje.Replace("Se modificó su contraseña. La nueva contraseña es ", "");

        Assert.IsTrue(ultimaNotificacion.Mensaje.StartsWith("Se modificó su contraseña. La nueva contraseña es "));
        Assert.IsFalse(string.IsNullOrEmpty(nuevaContrasena)); // asegurar que hay una nueva contraseña en el mensaje
        Assert.IsTrue(usuarioObjetivo.Autenticar(nuevaContrasena));
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void SeNotificaModificacionDeContrasena()
    {
        UsuarioDTO usuarioSolicitante = CrearYAsignarAdminSistema();
        UsuarioDTO usuarioObjetivoDTO = CrearUsuarioDTO("Mateo", "Pérez", "unemail44@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioObjetivoDTO);

        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioSolicitante, usuarioObjetivoDTO.Id, nuevaContrasena);

        Usuario usuarioObjetivo = _repositorioUsuarios.ObtenerPorId(usuarioObjetivoDTO.Id);
        Notificacion ultimaNotificacion = usuarioObjetivo.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.ContrasenaModificada(nuevaContrasena);

        Assert.AreEqual(mensajeEsperado,
            ultimaNotificacion.Mensaje);
        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
    }

    [TestMethod]
    public void NoSeNotificaSiElPropioUsuarioModificaSuContrasena()
    {
        UsuarioDTO usuarioDTO = CrearUsuarioDTO("Juan", "Pérez", "unemail45@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuarioDTO);
        string nuevaContrasena = "NuevaContraseña/1";
        _gestorUsuarios.ModificarContrasena(usuarioDTO, usuarioDTO.Id, nuevaContrasena);

        Usuario usuario = _repositorioUsuarios.ObtenerPorId(usuarioDTO.Id);
        Assert.AreEqual(0, usuario.Notificaciones.Count());
    }

    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeCreaUnUsuario()
    {
        UsuarioDTO admin2DTO = CrearYAsignarAdminSistema();
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail46@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);

        Usuario admin2 = _repositorioUsuarios.ObtenerPorId(admin2DTO.Id);
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.UsuarioCreado(usuario.Nombre, usuario.Apellido);

        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueCreaUnUsuario()
    {
        Usuario adminSistema = _gestorUsuarios.AdministradorInicial;

        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail47@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        Assert.AreEqual(0, adminSistema.Notificaciones.Count());
    }

    [TestMethod]
    public void SeNotificaAAdministradoresSistemaCuandoSeEliminaUnUsuario()
    {
        UsuarioDTO admin2DTO = CrearYAsignarAdminSistema();
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail48@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario.Id);

        Usuario admin2 = _repositorioUsuarios.ObtenerPorId(admin2DTO.Id);
        Notificacion ultimaNotificacion = admin2.Notificaciones.Last();
        string mensajeEsperado = MensajesNotificacion.UsuarioEliminado(usuario.Nombre, usuario.Apellido);

        Assert.AreEqual(DateTime.Today, ultimaNotificacion.Fecha);
        Assert.AreEqual(mensajeEsperado, ultimaNotificacion.Mensaje);
    }

    [TestMethod]
    public void NoSeNotificaAlUsuarioQueEliminaUnUsuario()
    {
        Usuario adminSistema = _gestorUsuarios.AdministradorInicial;
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail49@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        _gestorUsuarios.EliminarUsuario(_adminSistemaDTO, usuario.Id);
        Assert.AreEqual(0, adminSistema.Notificaciones.Count());
    }

    [TestMethod]
    public void LoginCorrecto()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail50@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        UsuarioDTO otro = CrearUsuarioDTO("Mateo", "Pérez", "unemail51@hotmail.com", "Contrase#a9)");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, otro);
        UsuarioDTO obtenido = _gestorUsuarios.LogIn(usuario.Email, "Contrase#a3");
        Assert.AreEqual(usuario.Id, obtenido.Id);
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void LoginIncorrectoConContraseñaIncorrecta()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "unemail52@gmail.com", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);
        UsuarioDTO obtenido = _gestorUsuarios.LogIn(usuario.Email, "ContraseñaIncorrecta");
    }

    [ExpectedException(typeof(ExcepcionUsuario))]
    [TestMethod]
    public void LoginIncorrectoConEmailNoRegistrado()
    {
        UsuarioDTO obtenido = _gestorUsuarios.LogIn("unemail53@noregistrado.com", "unaContraseña");
    }

    [TestMethod]
    public void SeObtienenLosUsuariosQueNoEstanEnUnaLista()
    {
        UsuarioDTO usuario1 = CrearUsuarioDTO("Juan", "Pérez", "jp@gmail.com", "Contrase#a3");
        UsuarioDTO usuario2 = CrearUsuarioDTO("Mateo", "Pérez", "mp@gmail.com", "Contrase#a3");
        UsuarioDTO usuario3 = CrearUsuarioDTO("José", "Pérez", "jp@adinet.com.uy", "Contrase#a3");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario1);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario2);
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario3);

        List<UsuarioListarDTO> usuarios = new List<UsuarioListarDTO>
        {
            UsuarioListarDTO.DesdeDTO(usuario1),
            UsuarioListarDTO.DesdeDTO(usuario2),
            UsuarioListarDTO.DesdeDTO(_adminSistemaDTO),
        };
        List<UsuarioListarDTO> usuariosNoEnLista = _gestorUsuarios.ObtenerUsuariosDiferentes(usuarios);

        Assert.AreEqual(1, usuariosNoEnLista.Count);
        Assert.AreEqual(usuario3.Id, usuariosNoEnLista.ElementAt(0).Id);
    }

    [ExpectedException(typeof(ExcepcionPermisos))]
    [TestMethod]
    public void ValidarQueUnUsuarioNoEsPrimerAdminLanzaExcepcionConElPrimerAdmin()
    {
        Usuario admin = _gestorUsuarios.AdministradorInicial;
        _gestorUsuarios.ValidarUsuarioNoEsAdministradorInicial(admin.Id);
    }

    [TestMethod]
    public void SeBorraUnaNotificacionOk()
    {
        UsuarioDTO usuario = CrearUsuarioDTO("Juan", "Pérez", "admin@gmail.com", "Admin123$");
        _gestorUsuarios.CrearYAgregarUsuario(_adminSistemaDTO, usuario);

        Usuario usuarioDominio = _repositorioUsuarios.ObtenerPorId(usuario.Id);
        usuarioDominio.RecibirNotificacion("notificación"); //se hardcodea por simplicidad de test

        _gestorUsuarios.BorrarNotificacion(usuario.Id, usuarioDominio.Notificaciones.First().Id);

        Assert.AreEqual(0, usuarioDominio.Notificaciones.Count());
    }
    
}