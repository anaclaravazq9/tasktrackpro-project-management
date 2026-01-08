namespace DTOs;

public class ArchivoExportadoDTO
{
    public string TipoContenido { get; }
    public string NombreArchivo { get; }
    public byte[] Contenido { get; }

    public ArchivoExportadoDTO(string tipoContenido, string nombreArchivo, byte[] contenido)
    {
        TipoContenido = tipoContenido;
        NombreArchivo = nombreArchivo;
        Contenido = contenido;
    }
}