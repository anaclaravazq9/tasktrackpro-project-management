namespace IServicios;

public interface IExportadorProyectosFactory
{
    public IExportadorProyectos CrearExportador(string formato);
}