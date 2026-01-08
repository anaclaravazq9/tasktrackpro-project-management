using DTOs;
using IServicios;

namespace Controladores;

public class ControladorExportacion
{
    private readonly IExportadorProyectosFactory _factory;

    public ControladorExportacion(IExportadorProyectosFactory factory)
    {
        _factory = factory;
    }

    public async Task<ArchivoExportadoDTO> Exportar(string formato)
    {
        IExportadorProyectos exportador = _factory.CrearExportador(formato);
        byte[] contenido = await exportador.Exportar();
        return new ArchivoExportadoDTO(exportador.TipoContenido, exportador.NombreArchivo, contenido);
    }
}