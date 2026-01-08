using Dominio;

namespace DTOs;

public class RangoDeUsoDTO
{
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadDeUsos { get; set; }
    
    public static RangoDeUsoDTO DesdeEntidad(RangoDeUso rango)
    {
        return new RangoDeUsoDTO
        {
            FechaInicio = rango.FechaInicio,
            FechaFin = rango.FechaFin,
            CantidadDeUsos = rango.CantidadDeUsos
        };
    }
}