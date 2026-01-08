using System.Text;
using System.Text.Json;
using IRepositorios;
using IServicios;

namespace Servicios.Exportacion;

public class ExportadorJson : IExportadorProyectos
{
    private readonly IRepositorioProyectos _repositorio;

    public ExportadorJson(IRepositorioProyectos repositorio)
    {
        _repositorio = repositorio;
    }

    public string NombreFormato => "json";
    public string TipoContenido => "application/json";
    public string NombreArchivo => "proyectos.json";

    public Task<byte[]> Exportar()
    {
        var proyectos = _repositorio.ObtenerTodos()
            .OrderBy(p => p.FechaInicio)
            .Select(p => new
            {
                Nombre = p.Nombre,
                FechaInicio = p.FechaInicio.ToString("dd/MM/yyyy"),
                Tareas = p.Tareas
                    .OrderByDescending(t => t.Titulo)
                    .Select(t => new
                    {
                        Titulo = t.Titulo,
                        FechaInicio = t.FechaInicioMasTemprana.ToString("dd/MM/yyyy"),
                        Duracion = t.DuracionEnDias,
                        EnCaminoCritico = t.EsCritica() ? "S" : "N",
                        Recursos = t.RecursosNecesarios.Select(r => r.ToString()).ToList()
                    }).ToList()
            }).ToList();

        var opciones = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        var json = JsonSerializer.Serialize(proyectos, opciones);
        var bytes = Encoding.UTF8.GetBytes(json);
        return Task.FromResult(bytes);
    }
}