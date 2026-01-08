using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class DependenciaTests
{
    private Tarea _tarea;

    [TestInitialize]
    public void Setup()
    {
        DateTime fechaInicioEstimada = new DateTime(2026, 9, 1);
        _tarea = new Tarea("titulo", "descripcion", 6, fechaInicioEstimada);
    }

    [TestMethod]
    public void ConstructorConParametrosAsignadosCorrectamente()
    {
        string tipo = "SS";

        Dependencia dependencia = new Dependencia(tipo, _tarea);

        Assert.AreEqual(tipo, dependencia.Tipo);
        Assert.AreEqual(_tarea, dependencia.Tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsVacio()
    {
        Dependencia dependencia = new Dependencia("", _tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsNull()
    {
        Dependencia dependencia = new Dependencia(null, _tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoEsEspacio()
    {
        Dependencia dependencia = new Dependencia("   ", _tarea);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTareaEsNull()
    {
        Dependencia dependencia = new Dependencia("SS", null);
    }

    [TestMethod]
    public void Constructor_AceptaTipoSS()
    {
        Dependencia dependencia = new Dependencia("SS", _tarea);
        Assert.AreEqual("SS", dependencia.Tipo);
    }

    [TestMethod]
    public void Constructor_AceptaTipoFS()
    {
        Dependencia dependencia = new Dependencia("FS", _tarea);
        Assert.AreEqual("FS", dependencia.Tipo);
    }

    [TestMethod]
    [ExpectedException(typeof(ExcepcionDominio))]
    public void Constructor_LanzaExcepcionSiTipoNoEsSSNiFS()
    {
        Dependencia dependencia = new Dependencia("FF", _tarea);
    }

    [TestMethod]
    public void EqualsRetornaTrueSiLasTareasYTiposSonIguales()
    {
        Dependencia dependencia1 = new Dependencia("SS", _tarea);
        Dependencia dependencia2 = new Dependencia("SS", _tarea);
        bool sonIguales = dependencia1.Equals(dependencia2);
        Assert.IsTrue(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiLosTiposNoSonIguales()
    {
        Dependencia dependencia1 = new Dependencia("FS", _tarea);
        Dependencia dependencia2 = new Dependencia("SS", _tarea);
        bool sonIguales = dependencia1.Equals(dependencia2);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoEsNull()
    {
        Dependencia dependencia = new Dependencia("SS", _tarea);
        bool sonIguales = dependencia.Equals(null);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoNoEsTarea()
    {
        Dependencia dependencia = new Dependencia("SS", _tarea);
        int otro = 0;
        bool sonIguales = dependencia.Equals(otro);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void GetHashCodeFuncionaOk()
    {
        Dependencia dependencia1 = new Dependencia("SS", _tarea);
        Dependencia dependencia2 = new Dependencia("SS", _tarea);

        Tarea nueva = new Tarea("titulo", "descripcion", 6, DateTime.Now);
        nueva.Id = 2;
        Dependencia dependencia3 = new Dependencia("SS", nueva);

        Assert.AreEqual(dependencia1.GetHashCode(), dependencia2.GetHashCode());
        Assert.AreNotEqual(dependencia3.GetHashCode(), dependencia1.GetHashCode());
    }

    [TestMethod]
    public void ToStringFuncionaOk()
    {
        Dependencia dependencia = new Dependencia("SS", _tarea);
        Assert.AreEqual("titulo (SS)", dependencia.ToString());
    }
    
    [TestMethod]
    public void ConstructorSinParametros_Publico_CreaInstanciaCorrectamente()
    {
        var dependencia = new Dependencia();
        Assert.IsNotNull(dependencia);
    }

}