using Excepciones;
using Excepciones.MensajesError;
using IRepositorios;
using IServicios;

namespace Servicios.Exportacion;

public class ExportadorProyectosFactory : IExportadorProyectosFactory
{
    private readonly IRepositorioProyectos _repositorio;

    public ExportadorProyectosFactory(IRepositorioProyectos repositorio)
    {
        _repositorio = repositorio;
    }
    
    public IExportadorProyectos CrearExportador(string formato)
    {
        if (formato == "csv")
        {
            return new ExportadorCsv(_repositorio);
        }
        else if (formato == "json")
        {
            return new ExportadorJson(_repositorio);
        }
        else
        {
            throw new ExcepcionExportador(MensajesErrorServicios.FormatoNoSoportado);
        }
    }
}
