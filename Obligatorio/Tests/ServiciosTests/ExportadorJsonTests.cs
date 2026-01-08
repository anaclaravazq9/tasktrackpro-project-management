using Moq;
using System.Text;
using Dominio;
using IRepositorios;
using Servicios.Exportacion;

namespace Tests.ServiciosTests
{
    [TestClass]
    public class ExportadorJsonTests
    {
        [TestMethod]
        public async Task ExportarJson_IncluyeProyectoYTareaYRecurso_ConFormatoEsperado()
        {
            Usuario admin = new Usuario { Id = 1 };
            Usuario miembro = new Usuario { Id = 2 };
            List<Usuario> miembros = new List<Usuario> { admin, miembro };

            Proyecto proyecto = new Proyecto("Proyecto JSON", "Descripción", new DateTime(2027, 3, 1), admin, miembros);

            Tarea tarea = new Tarea("Tarea B", "Otra desc", 4, new DateTime(2027, 3, 2));
            tarea.Holgura = 0;

            Recurso recurso = new Recurso { Id = 10, Nombre = "Licencias" };
            RecursoNecesario recursoNecesario = new RecursoNecesario(recurso, 3);
            tarea.RecursosNecesarios.Add(recursoNecesario);

            proyecto.AgregarTarea(tarea);

            Mock<IRepositorioProyectos> mockRepo = new Mock<IRepositorioProyectos>();
            mockRepo.Setup(r => r.ObtenerTodos()).Returns(new List<Proyecto> { proyecto });

            ExportadorJson exportador = new ExportadorJson(mockRepo.Object);
            
            var resultado = await exportador.Exportar();
            string contenido = Encoding.UTF8.GetString(resultado);
            
            Assert.IsTrue(contenido.Contains("\"Nombre\": \"Proyecto JSON\""));
            Assert.IsTrue(contenido.Contains("\"FechaInicio\": \"01/03/2027\""));
            Assert.IsTrue(contenido.Contains("\"Titulo\": \"Tarea B\""));
            Assert.IsTrue(contenido.Contains("\"EnCaminoCritico\": \"S\""));
            Assert.IsTrue(contenido.Contains("Licencias"));
        }

        [TestMethod]
        public void ExportadorJson_PropiedadesDevuelvenValoresEsperados()
        {
            Mock<IRepositorioProyectos> mockRepo = new Mock<IRepositorioProyectos>();
            ExportadorJson exportador = new ExportadorJson(mockRepo.Object);
            
            string formato = exportador.NombreFormato;
            string tipoContenido = exportador.TipoContenido;
            string nombreArchivo = exportador.NombreArchivo;

            Assert.AreEqual("json", formato);
            Assert.AreEqual("application/json", tipoContenido);
            Assert.AreEqual("proyectos.json", nombreArchivo);
        }
    }
}
