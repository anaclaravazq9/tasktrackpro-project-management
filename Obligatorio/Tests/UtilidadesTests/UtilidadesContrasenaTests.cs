using Excepciones;
using Utilidades;

namespace Tests.UtilidadesTests;

[TestClass]
public class UtilidadesContrasenaTests
{
    [TestMethod]
    public void ContrasenaEncriptadaEsDistintaALaOriginal()
    {
        string unaContrasena = "Contrase#a3";
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(unaContrasena);
        Assert.AreNotEqual(unaContrasena, contrasenaEncriptada);
    }

    [TestMethod]
    public void SePuedeVerificarContrasenaEncriptada()
    {
        string unaContrasena = "Contrase#a3";
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena(unaContrasena);
        Assert.IsTrue(BCrypt.Net.BCrypt.Verify(unaContrasena, contrasenaEncriptada));
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void IngresoDeContrasenaMuyCorta()
    {
        UtilidadesContrasena.ValidarYEncriptarContrasena("P3e.");
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void IngresoDeContrasenaSinMayusculas()
    {
        UtilidadesContrasena.ValidarYEncriptarContrasena("minuscula1@");
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void IngresoDeContrasenaSinMinusculas()
    {
        UtilidadesContrasena.ValidarYEncriptarContrasena("MAYUSCULA1@");
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void IngresoDeContrasenaSinNumeros()
    {
        UtilidadesContrasena.ValidarYEncriptarContrasena("CoNtRaSeN@");
    }

    [ExpectedException(typeof(ExcepcionContrasena))]
    [TestMethod]
    public void IngresoDeContrasenaSinCaracterEspecial()
    {
        UtilidadesContrasena.ValidarYEncriptarContrasena("CoNtRaSeN14");
    }

    [TestMethod]
    public void SeAutogeneraUnaContrasenaValida()
    {
        string contrasenaAutogenerada = UtilidadesContrasena.AutogenerarContrasenaValida();
        UtilidadesContrasena
            .ValidarYEncriptarContrasena(
                contrasenaAutogenerada); // si la contraseña no es válida, se lanza una excepción antes de encriptar
    }
}