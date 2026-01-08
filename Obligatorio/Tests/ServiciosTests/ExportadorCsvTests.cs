using System.Text;
using Dominio;
using IRepositorios;
using Moq;
using Servicios.Exportacion;

namespace Tests.ServiciosTests;

[TestClass]
public class ExportadorCsvTests
{
    [TestMethod]
    public async Task ExportarCsv_IncluyeProyectoYTareaYRecurso_ConFormatoEsperado()
    {
        Usuario admin = new Usuario { Id = 1 };
        Usuario miembro = new Usuario { Id = 2 };
        List<Usuario> miembros = new List<Usuario> { admin, miembro };

        Proyecto proyecto = new Proyecto("Proyecto Prueba", "Un proyecto de prueba", new DateTime(2027, 3, 1), admin, miembros);

        Tarea tarea = new Tarea("Tarea A", "Descripción", 5, new DateTime(2027, 3, 2));
        tarea.Holgura = 0;
        tarea.RecursosNecesarios.Add(new RecursoNecesario(new Recurso { Id = 1, Nombre = "Programadores" }, 2));

        proyecto.AgregarTarea(tarea);

        Mock<IRepositorioProyectos> mockRepo = new Mock<IRepositorioProyectos>();
        mockRepo.Setup(r => r.ObtenerTodos()).Returns(new List<Proyecto> { proyecto });

        ExportadorCsv exportador = new ExportadorCsv(mockRepo.Object);
        
        var resultado = await exportador.Exportar();
        var contenido = Encoding.UTF8.GetString(resultado);
        
        Assert.IsTrue(contenido.Contains("Proyecto Prueba,01/03/2027"));
        Assert.IsTrue(contenido.Contains("Tarea A,02/03/2027,S"));
        Assert.IsTrue(contenido.Contains("2 Programadores")); 
    }

    [TestMethod]
    public void ExportadorCsv_PropiedadesDevuelvenValoresEsperados()
    {
        Mock<IRepositorioProyectos> mockRepo = new Mock<IRepositorioProyectos>();
        ExportadorCsv exportador = new ExportadorCsv(mockRepo.Object);
        
        Assert.AreEqual("csv", exportador.NombreFormato);
        Assert.AreEqual("text/csv", exportador.TipoContenido);
        Assert.AreEqual("proyectos.csv", exportador.NombreArchivo);
    }


}