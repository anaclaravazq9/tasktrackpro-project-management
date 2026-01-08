using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class RangoDeUso
{
    public int Id { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public int CantidadDeUsos { get; set; }
    
    public RangoDeUso()
    {
    }
    
    public RangoDeUso(DateTime fechaInicio, DateTime fechaFin, int cantidadDeUsos)
    {
        ValidarFechaInicioMenorAFin(fechaInicio, fechaFin);
        ValidarCantidadDeUsosMayorACero(cantidadDeUsos);

        FechaInicio = fechaInicio;
        FechaFin = fechaFin;
        CantidadDeUsos = cantidadDeUsos;
    }
    
    private void ValidarFechaInicioMenorAFin(DateTime fechaInicio, DateTime fechaFin)
    {
        if (fechaFin < fechaInicio)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.FechaInicioRangoMayorQueFin);
        }
    }
    
    private void ValidarCantidadDeUsosMayorACero(int cantidadDeUsos)
    {
        if(cantidadDeUsos <= 0)
        {
            throw new ExcepcionRangoDeUso(MensajesErrorDominio.RangoDeUsoNoPuedeSerCeroOMenos);
        }
    }
    
    public override bool Equals(object? otro)
    {
        RangoDeUso otroRango = otro as RangoDeUso;
        return otroRango != null && Id == otroRango.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}