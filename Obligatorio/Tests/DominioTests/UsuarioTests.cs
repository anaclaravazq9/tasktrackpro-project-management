using Dominio;
using Excepciones;
using Utilidades;

namespace Tests.DominioTests

{
    [TestClass]
    public class UsuarioTests
    {
        private DateTime _fechaNacimientoValida = new DateTime(2000, 9, 1);

        private Usuario CrearUsuarioValido()
        {
            string contrasena = "Contrase#a3";
            string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(contrasena);
            return new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@gmail.com", contrasenaEncriptada);
        }

        [TestMethod]
        public void ConstructorSinParametrosCreaUsuario()
        {
            Usuario usuario = new Usuario();
            Assert.IsNotNull(usuario);
        }

        [TestMethod]
        public void ConstructorConParametrosAsignaCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();

            Assert.AreEqual("Juan", usuario.Nombre);
            Assert.AreEqual("Perez", usuario.Apellido);
            Assert.AreEqual(_fechaNacimientoValida, usuario.FechaNacimiento);
            Assert.AreEqual("unemail@gmail.com", usuario.Email);
            Assert.IsTrue(usuario.Autenticar("Contrase#a3"));
            Assert.AreEqual(0, usuario.CantidadProyectosAsignados);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaNombreNoVacio()
        {
            Usuario usuario = new Usuario("", "Perez", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaNombreNoNull()
        {
            Usuario usuario = new Usuario(null, "Perez", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaApellidoNoVacio()
        {
            Usuario usuario = new Usuario("Juan", " ", _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaApellidoNoNull()
        {
            Usuario usuario = new Usuario("Juan", null, _fechaNacimientoValida, "unemail@gmail.com", "Contrase#a3");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void ConstructorValidaEdadMaxima()
        {
            DateTime fechaHace100Años = new DateTime(1925, 04, 30);
            Usuario usuario = new Usuario("Juan", "Perez", fechaHace100Años, "unemail@gmail.com", "Contrase#a3");
        }
        
        [TestMethod]
        public void UsuarioNoEsLiderAlCrearse()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.EsLider);
        }

        [TestMethod]
        public void UsuarioSeCreaConListaNotificacionesVacia()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsNotNull(usuario.Notificaciones);
            Assert.AreEqual(0, usuario.Notificaciones.Count);
        }

        [TestMethod]
        public void UnNuevoUsuarioNoEsAdministradorDeSistemaPorDefecto()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.EsAdministradorSistema);
        }

        [TestMethod]
        public void UnNuevoUsuarioNoAdministraProyectosPorDefecto()
        {
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.EsAdministradorProyecto);
        }

        [TestMethod]
        public void SePuedeMarcarNuevoUsuarioComoAdministradorDeSistema()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.EsAdministradorSistema = true;
            Assert.IsTrue(usuario.EsAdministradorSistema);
        }

        [TestMethod]
        public void SePuedeMarcarNuevoUsuarioComoAdministrandoUnProyecto()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.EstaAdministrandoUnProyecto = true;
            Assert.IsTrue(usuario.EstaAdministrandoUnProyecto);
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioOk()
        {
            string contrasenaIngresada = "Contrase#a3";
            Usuario usuario = CrearUsuarioValido();
            Assert.IsTrue(usuario.Autenticar(contrasenaIngresada));
        }

        [TestMethod]
        public void CompararContrasenaDadaConContrasenaDeUsuarioIncorrecto()
        {
            string contrasenaIngresada = "OtraContraseña";
            Usuario usuario = CrearUsuarioValido();
            Assert.IsFalse(usuario.Autenticar(contrasenaIngresada));
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void IngresoDeEmailInvalido()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "emailinvalido", "xxxxxx");
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void DaErrorSiElUsuarioEsMenorDeEdad()
        {
            DateTime fechaNacimiento = new DateTime(2020, 1, 6);
            Usuario usuario = new Usuario("Juan", "Perez", fechaNacimiento, "unemail@hotmail.com", "xxxxx");
        }
        
        [TestMethod]
        public void AsignarRolLiderMarcaAlUsuarioComoLider()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.AsignarRolLider();
            Assert.IsTrue(usuario.EsLider);
        }
        
        
        [TestMethod]
        public void RemoverComoLiderMarcaUsuarioComoNoLider()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.AsignarRolLider();
            usuario.RemoverRolLider();
            Assert.IsFalse(usuario.EsLider);
        }

        [TestMethod]
        public void SeExponeElSeteoDeContrasenaEncriptada()
        {
            Usuario usuario = CrearUsuarioValido();
            string contrasena = "NuevaContraseña123";
            string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(contrasena);
            usuario.ContrasenaEncriptada =contrasenaEncriptada;
            Assert.IsTrue(usuario.Autenticar(contrasena));
        }
        
        [TestMethod]
        public void SeCambiaContrasenaCorrectamente()
        {
            Usuario usuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "unemail@adinet.com", "xxxxxxx");
            string nuevaContrasena = "CoNtRaSeN1@";
            string nuevaContrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(nuevaContrasena);
            usuario.EstablecerContrasenaEncriptada(nuevaContrasenaEncriptada);
            Assert.IsTrue(usuario.Autenticar(nuevaContrasena));
        }

        [TestMethod]
        public void SeCambiaEmailCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.CambiarEmail("otroEmail@fi365.ort.edu.uy");
            Assert.AreEqual("otroEmail@fi365.ort.edu.uy", usuario.Email);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void DaErrorSiSeCambiaEmailInvalido()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.CambiarEmail("email");
        }

        [TestMethod]
        public void NoSeCambiaEmailInvalido()
        {
            Usuario usuario = CrearUsuarioValido();
            string nuevoEmail = "email";
            try
            {
                usuario.CambiarEmail(nuevoEmail);
            }
            catch (ExcepcionDominio)
            {
            } // Ignorar la excepción

            Assert.AreNotEqual(nuevoEmail, usuario.Email);
            Assert.AreEqual("unemail@gmail.com", usuario.Email);
        }

        [TestMethod]
        public void SeRecibeUnaNotificacionCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensaje de notificación";
            usuario.RecibirNotificacion(mensaje);

            Assert.AreEqual(1, usuario.Notificaciones.Count);
            Assert.AreEqual("un mensaje de notificación", usuario.Notificaciones.ElementAt(0).Mensaje);
            Assert.AreEqual(DateTime.Today, usuario.Notificaciones.ElementAt(0).Fecha);
        }

        [TestMethod]
        public void SeBorranNotificacionesCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensaje de notificación";
            usuario.RecibirNotificacion(mensaje);
            usuario.RecibirNotificacion(mensaje);
            int id1 = usuario.Notificaciones.ElementAt(0).Id;

            usuario.BorrarNotificacion(id1);
            Assert.AreEqual(1, usuario.Notificaciones.Count);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void BorrarNotificacionInexistenteDaError()
        {
            Usuario usuario = CrearUsuarioValido();
            string mensaje = "un mensaje de notificación";
            usuario.RecibirNotificacion(mensaje);
            usuario.RecibirNotificacion(mensaje);
            usuario.BorrarNotificacion(1000);
        }

        [ExpectedException(typeof(ExcepcionDominio))]
        [TestMethod]
        public void BorrarNotificacionEnListaVaciaDaError()
        {
            Usuario usuario = CrearUsuarioValido();
            usuario.BorrarNotificacion(2);
        }

        [TestMethod]
        public void SeActualizaElEmailDeUnUsuarioCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario usuarioConOtroEmail = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", "xxxxxx");
            usuarioConOtroEmail.Id = usuario.Id;
            
            usuario.Actualizar(usuarioConOtroEmail);
            Assert.AreEqual("otroemail@gmail.com", usuario.Email);
        }
        
        [TestMethod]
        public void SeActualizaLaContrasenaDeUnUsuarioCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string nuevaContrasena = UtilidadesContrasena.ValidarYEncriptarContrasena("Admin123$");
            Usuario usuarioConOtraContrasena = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", nuevaContrasena);
            usuarioConOtraContrasena.Id = usuario.Id;
            
            usuario.Actualizar(usuarioConOtraContrasena);
            Assert.IsTrue(usuario.Autenticar("Admin123$"));
        }
        
        [TestMethod]
        public void SeActualizaEsAdministradorProyectoDeUnUsuarioCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario nuevoUsuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", "xxxx");
            nuevoUsuario.Id = usuario.Id;
            
            nuevoUsuario.EsAdministradorProyecto = true;
            
            usuario.Actualizar(nuevoUsuario);
            Assert.IsTrue(nuevoUsuario.EsAdministradorProyecto);
        }
        
        [TestMethod]
        public void SeActualizaEsAdministradorSistemaDeUnUsuarioCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario nuevoUsuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", "xxxx");
            nuevoUsuario.Id = usuario.Id;
            
            nuevoUsuario.EsAdministradorSistema = true;
            
            usuario.Actualizar(nuevoUsuario);
            Assert.IsTrue(nuevoUsuario.EsAdministradorSistema);
        }
        
        [TestMethod]
        public void SeActualizaEstaAdministrandoUnProyectoCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario nuevoUsuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", "xxxx");
            nuevoUsuario.Id = usuario.Id;
            
            nuevoUsuario.EstaAdministrandoUnProyecto = true;
            
            usuario.Actualizar(nuevoUsuario);
            Assert.IsTrue(nuevoUsuario.EstaAdministrandoUnProyecto);
        }
        
        [TestMethod]
        public void SeActualizaCantidadDeProyectosAsignadosCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario nuevoUsuario = new Usuario("Juan", "Perez", _fechaNacimientoValida, "otroemail@gmail.com", "xxxx");
            nuevoUsuario.Id = usuario.Id;
            
            nuevoUsuario.CantidadProyectosAsignados = 10;
            
            usuario.Actualizar(nuevoUsuario);
            Assert.AreEqual(10, usuario.CantidadProyectosAsignados);
        }
        
        [ExpectedException(typeof(ExcepcionUsuario))]
        [TestMethod]
        public void NoSePuedeActualizarUnUsuarioConIdDiferente()
        {
            Usuario usuario = CrearUsuarioValido();
            Usuario usuarioConIdDiferente = CrearUsuarioValido();
            usuarioConIdDiferente.Id = 2;
            
            usuario.Actualizar(usuarioConIdDiferente);
        }
        
        [TestMethod]
        public void EqualsRetornaTrueSiLosIdsSonIguales()
        {
            Usuario usuario1 = CrearUsuarioValido();
            Usuario usuario2 = CrearUsuarioValido();

            usuario1.Id = 1; // se hardcodean ids para que ambos usuarios tengan el mismo id
            usuario2.Id = 1;

            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsTrue(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiLosIdsNoSonIguales()
        {
            Usuario usuario1 = CrearUsuarioValido();
            Usuario usuario2 = CrearUsuarioValido();

            usuario1.Id = 1; // se hardcodean ids para que ambos usuarios tengan distinto id
            usuario2.Id = 2;

            bool sonIguales = usuario1.Equals(usuario2);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiUnObjetoEsNull()
        {
            Usuario usuario1 = CrearUsuarioValido();
            bool sonIguales = usuario1.Equals(null);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void EqualsRetornaFalseSiUnObjetoNoEsUsuario()
        {
            Usuario usuario1 = CrearUsuarioValido();
            int otro = 0;
            bool sonIguales = usuario1.Equals(otro);
            Assert.IsFalse(sonIguales);
        }

        [TestMethod]
        public void GetHashCodeFuncionaOk()
        {
            Usuario usuario1 = CrearUsuarioValido();
            Usuario usuario2 = CrearUsuarioValido();
            //ambos tienen mismo id ya que no hay un gestor que maneje ids
            Usuario usuario3 = CrearUsuarioValido();
            usuario3.Id =
                3; // usuario con id distinto a los otros 2 (se hardcodea en vez de llamar al gestor por simplicidad)
            Assert.AreEqual(usuario1.GetHashCode(), usuario2.GetHashCode());
            Assert.AreNotEqual(usuario3.GetHashCode(), usuario1.GetHashCode());
        }

        [TestMethod]
        public void ToStringFuncionaCorrectamente()
        {
            Usuario usuario = CrearUsuarioValido();
            string resultadoEsperado = $"{usuario.Nombre} {usuario.Apellido} ({usuario.Email})";
            Assert.AreEqual(resultadoEsperado, usuario.ToString());
        }
        
        [TestMethod]
        public void ConstructorSinParametros_CreaInstanciaCorrectamente()
        {
            var usuario = new Usuario();
            Assert.IsNotNull(usuario);
        }
    }
}