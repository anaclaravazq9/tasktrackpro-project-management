using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RangoDeUsoTests
{
    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RangoDeUso rango = new RangoDeUso();
        Assert.IsNotNull(rango);
    }
    
    [TestMethod]
    public void ConstructorConParametros_CreaRangoCorrectamente()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        int capacidadEnUso = 10;
        
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, capacidadEnUso);
        rango.Id = 1;
        
        Assert.AreEqual(1, rango.Id);
        Assert.AreEqual(fechaInicio, rango.FechaInicio);
        Assert.AreEqual(fechaFin, rango.FechaFin);
        Assert.AreEqual(capacidadEnUso, rango.CantidadDeUsos);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiFechaInicioEsMayorQueFechaFin()
    {
        DateTime fechaInicio = DateTime.Today.AddDays(10);
        DateTime fechaFin = DateTime.Today;
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 10);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiCantidadDeUsosEs0()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 0);
    }
    
    [ExpectedException(typeof(ExcepcionRangoDeUso))]
    [TestMethod]
    public void ConstructorConParametros_LanzaExcepcionSiCantidadDeUsosEsNegativa()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
            
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, -2);
    }
    
    [TestMethod]
    public void EqualsRetornaTrueSiLosIdsSonIguales()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        // por simplicidad se hardcodean los ids, 
        RangoDeUso rango1 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 1 };
        RangoDeUso rango2 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 1 };
        bool sonIguales = rango1.Equals(rango2);
        Assert.IsTrue(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiLosIdsNoSonIguales()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        // por simplicidad se hardcodean los ids, 
        RangoDeUso rango1 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 1 };
        RangoDeUso rango2 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 2 };
        bool sonIguales = rango1.Equals(rango2);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoEsNull()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 3);
        bool sonIguales = rango.Equals(null);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoNoEsRecurso()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        RangoDeUso rango = new RangoDeUso(fechaInicio, fechaFin, 3);
        int otro = 0;
        bool sonIguales = rango.Equals(otro);
        Assert.IsFalse(sonIguales);
    }

    [TestMethod]
    public void GetHashCodeFuncionaOk()
    {
        DateTime fechaInicio = DateTime.Today;
        DateTime fechaFin = DateTime.Today.AddDays(10);
        // por simplicidad se hardcodean los ids, 
        RangoDeUso rango1 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 1 };
        RangoDeUso rango2 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 1 };
        RangoDeUso rango3 = new RangoDeUso(fechaInicio, fechaFin, 3) { Id = 2 };
        Assert.AreEqual(rango1.GetHashCode(), rango2.GetHashCode());
        Assert.AreNotEqual(rango3.GetHashCode(), rango1.GetHashCode());
    }
}