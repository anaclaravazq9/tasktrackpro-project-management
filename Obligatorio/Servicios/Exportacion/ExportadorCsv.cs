using System.Text;
using IRepositorios;
using IServicios;

namespace Servicios.Exportacion;

public class ExportadorCsv : IExportadorProyectos
{
    private readonly IRepositorioProyectos _repositorio;
    public ExportadorCsv(IRepositorioProyectos repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "csv";
    public string TipoContenido => "text/csv";
    public string NombreArchivo => "proyectos.csv";

    public Task<byte[]> Exportar()
    {
        var proyectos = _repositorio.ObtenerTodos()
            .OrderBy(p => p.FechaInicio)
            .ToList();

        var sb = new StringBuilder();

        foreach (var proyecto in proyectos)
        {
            sb.AppendLine($"{proyecto.Nombre},{proyecto.FechaInicio:dd/MM/yyyy}");

            var tareasOrdenadas = proyecto.Tareas
                .OrderByDescending(t => t.Titulo)
                .ToList();

            foreach (var tarea in tareasOrdenadas)
            {
                string enCaminoCritico = tarea.EsCritica() ? "S" : "N";
                sb.AppendLine($"{tarea.Titulo},{tarea.FechaInicioMasTemprana:dd/MM/yyyy},{enCaminoCritico}");

                foreach (var recurso in tarea.RecursosNecesarios)
                {
                    sb.AppendLine(recurso.ToString());
                }
            }
        }

        return Task.FromResult(Encoding.UTF8.GetBytes(sb.ToString()));
    }

    
}
