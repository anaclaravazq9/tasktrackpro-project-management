using Dominio;
using Excepciones;

namespace Tests.DominioTests;

[TestClass]
public class RecursoNecesarioTests
{

    [TestMethod]
    public void Constructor_InicializaUnRango()
    {
        RecursoNecesario recursoNecesario = new RecursoNecesario();
        Assert.IsNotNull(recursoNecesario);
    }
    

    [TestMethod]
    public void Constructor_ConParametrosAsignaCorrectamente()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        int cantidad = 2;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);

        Assert.AreEqual(recurso, rn.Recurso);
        Assert.AreEqual(cantidad, rn.Cantidad);
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void Constructor_LanzaExcepcionSiRecursoEsNull()
    {
        Recurso recurso = null;
        int cantidad = 2;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);
    }
    
    [ExpectedException(typeof(ExcepcionRecurso))]
    [TestMethod]
    public void Constructor_LanzaExcepcionSiCantidadEsMenorOIgualACero()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        int cantidad = 0;

        RecursoNecesario rn = new RecursoNecesario(recurso, cantidad);
    }
    
    [TestMethod]
    public void EqualsRetornaTrueSiLosIdsSonIguales()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        RecursoNecesario rn1 = new RecursoNecesario(recurso, 2) { Id = 1 };
        RecursoNecesario rn2 = new RecursoNecesario(recurso, 2) { Id = 1 };

        bool sonIguales = rn1.Equals(rn2);
        Assert.IsTrue(sonIguales);
    }
    
    
    [TestMethod]
    public void EqualsRetornaFalseSiLosIdsNoSonIguales()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        RecursoNecesario rn1 = new RecursoNecesario(recurso, 2) { Id = 1 };
        RecursoNecesario rn2 = new RecursoNecesario(recurso, 2) { Id = 2 };

        bool sonIguales = rn1.Equals(rn2);
        Assert.IsFalse(sonIguales);
    }
    
    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoEsNull()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        RecursoNecesario rn = new RecursoNecesario(recurso, 2);
        bool sonIguales = rn.Equals(null);
        Assert.IsFalse(sonIguales);
    }
    
    [TestMethod]
    public void EqualsRetornaFalseSiUnObjetoNoEsRecursoNecesario()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        RecursoNecesario rn = new RecursoNecesario(recurso, 2);
        int otro = 0;
        bool sonIguales = rn.Equals(otro);
        Assert.IsFalse(sonIguales);
    }
    
    [TestMethod]
    public void GetHashCodeFuncionaOk()
    {
        Recurso recurso = new Recurso("Nombre", "Tipo", "Descripcion", 5);
        RecursoNecesario rn1 = new RecursoNecesario(recurso, 2) { Id = 1 };
        RecursoNecesario rn2 = new RecursoNecesario(recurso, 2) { Id = 1 };
        RecursoNecesario rn3 = new RecursoNecesario(recurso, 2) { Id = 2 };

        Assert.AreEqual(rn1.GetHashCode(), rn2.GetHashCode());
        Assert.AreNotEqual(rn3.GetHashCode(), rn1.GetHashCode());
    }
}