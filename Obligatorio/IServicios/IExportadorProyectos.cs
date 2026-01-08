namespace IServicios;

public interface IExportadorProyectos
{
    string NombreFormato { get; }
    string TipoContenido { get; }
    string NombreArchivo { get; } 
    
    Task<byte[]> Exportar(); 
}