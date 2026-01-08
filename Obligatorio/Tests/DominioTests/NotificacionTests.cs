using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class NotificacionTests
{
    private Notificacion _nuevaNotificacion;

    [TestInitialize]
    public void SetUp()
    {
        _nuevaNotificacion = new Notificacion("Mensaje de notificación");
    }

    [TestMethod]
    public void ConstructorSinParametrosCreaNotificacionOk()
    {
        Notificacion notificacion = new Notificacion();
        Assert.IsNotNull(notificacion);
    }

    [TestMethod]
    public void ConstructorAsignaMensajeOk()
    {
        Assert.AreEqual("Mensaje de notificación", _nuevaNotificacion.Mensaje);
    }

    [TestMethod]
    public void ConstructorAsignaFechaOk()
    {
        Assert.AreEqual(DateTime.Today, _nuevaNotificacion.Fecha);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiMensajeEsNull()
    {
        Notificacion notificacionNull = new Notificacion(null);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiMensajeEsVacio()
    {
        Notificacion notificacionVacia = new Notificacion("");
    }
    
    [TestMethod]
    public void SetterId_PermiteSetearIdCorrectamente()
    {
        _nuevaNotificacion.Id = 5;
        Assert.AreEqual(5, _nuevaNotificacion.Id);
    }
}