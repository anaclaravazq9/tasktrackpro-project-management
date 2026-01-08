using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Recurso
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Tipo { get; set; }
    public string Descripcion { get; set; }
    public Proyecto? ProyectoAsociado { get; set; } = null;
    public int Capacidad { get;  set; } 
    public int CantidadDeTareasUsandolo { get; set; } = 0;
    public virtual ICollection<RangoDeUso> RangosEnUso { get; set; }
    
    public Recurso()
    {
        RangosEnUso = new List<RangoDeUso>();
    }
    
    public Recurso(string nombre, string tipo, string descripcion, int capacidad)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        ValidarAtributoNoVacio(tipo, "tipo");
        ValidarAtributoNoVacio(descripcion, "descripcion");
        ValidarCapacidadMayorACero(capacidad);
        
        Nombre = nombre;
        Tipo = tipo;
        Descripcion = descripcion;
        RangosEnUso = new List<RangoDeUso>();
        Capacidad = capacidad;
    }

    public void ModificarNombre(string nombre)
    {
        ValidarAtributoNoVacio(nombre, "nombre");
        Nombre = nombre;
    }

    public void ModificarTipo(string tipo)
    {
        ValidarAtributoNoVacio(tipo, "tipo");
        Tipo = tipo;
    }

    public void ModificarDescripcion(string descripcion)
    {
        ValidarAtributoNoVacio(descripcion, "descripcion");
        Descripcion = descripcion;
    }

    public void AsociarAProyecto(Proyecto proyecto)
    {
        if (EsExclusivo())
        {
            throw new ExcepcionDominio(MensajesErrorDominio.RecursoYaEsExclusivo);
        }
        ProyectoAsociado = proyecto;
    }

    public void IncrementarCantidadDeTareasUsandolo()
    {
        CantidadDeTareasUsandolo++;
    }

    public void DecrementarCantidadDeTareasUsandolo()
    {
        if (CantidadDeTareasUsandolo <= 0)
        {
            throw new ExcepcionDominio(MensajesErrorDominio.CantidadTareasRecursoNegativa);
        }
        CantidadDeTareasUsandolo--;
    }

    public bool TieneCapacidadDisponible(DateTime fechaDesde, DateTime fechaHasta, int capacidadRequerida)
    {
        ValidarCapacidadMayorACero(capacidadRequerida);
        ValidarCapacidadMenorACapacidadRecurso(capacidadRequerida);
        ValidarInicioAntesQueFin(fechaDesde, fechaHasta);
        
        for (DateTime dia = fechaDesde; dia <= fechaHasta; dia = dia.AddDays(1))
        {
            int usosEnElDia = CantidadDeUsosPorDia(dia); 
            
            int totalUsosEnElDia = usosEnElDia + capacidadRequerida;

            if (totalUsosEnElDia > Capacidad)
            {
                return false;
            }
        }
        return true;
    }

    public void AgregarRangoDeUso(DateTime fechaInicio, DateTime fechaFin, int cantidadNuevo)
    { 
        RangoDeUso nuevoRango = new RangoDeUso(fechaInicio, fechaFin, cantidadNuevo);
        RangosEnUso.Add(nuevoRango);
    }
    
    public void AgregarRangoDeUsoForzado(DateTime fechaInicio, DateTime fechaFin, int cantidadNuevo)
    {
        RangoDeUso nuevoRango = new RangoDeUso(fechaInicio, fechaFin, cantidadNuevo);
        RangosEnUso.Add(nuevoRango);
    }
    
    public void EliminarRango(DateTime inicio, DateTime fin, int cantidad)
    {
        RangoDeUso rango = RangosEnUso.FirstOrDefault(r =>
            r.FechaInicio == inicio && r.FechaFin == fin && r.CantidadDeUsos == cantidad);
        ValidarRangoNoNulo(rango);
        RangosEnUso.Remove(rango);
    }

    public void ModificarCapacidad(int nuevaCapacidad)
    {
        ValidarCapacidadMayorACero(nuevaCapacidad);
        
        if (nuevaCapacidad < Capacidad)
        {
            ValidarSiElUsoSuperaLaNuevaCapacidad(nuevaCapacidad);
        }
        
        Capacidad = nuevaCapacidad;
    }
    
    public bool UsoSeAjustaANuevaCapacidad(int nuevaCapacidad)
    {
        if (!RangosEnUso.Any()) return true;
        DateTime primeraFechaDeUso = RangosEnUso.Min(r => r.FechaInicio.Date);
        DateTime ultimaFechaDeUso = RangosEnUso.Max(r => r.FechaFin.Date);

        for (DateTime dia = primeraFechaDeUso; dia <= ultimaFechaDeUso; dia = dia.AddDays(1))
        {
            int usoEnDia = CantidadDeUsosPorDia(dia);
            
            if (usoEnDia > nuevaCapacidad)
            {
                return false;
            }
        }
        return true;
    }

    private void ValidarSiElUsoSuperaLaNuevaCapacidad(int nuevaCapacidad)
    {
        if (!RangosEnUso.Any()) return; 
        DateTime primeraFechaDeUso = RangosEnUso.Min(r => r.FechaInicio.Date);
        DateTime ultimaFechaDeUso = RangosEnUso.Max(r => r.FechaFin.Date);
        for (DateTime dia = primeraFechaDeUso; dia <= ultimaFechaDeUso; dia = dia.AddDays(1))
        {
            int usoEnDia = CantidadDeUsosPorDia(dia);
            if (usoEnDia > nuevaCapacidad)
            {
                throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadNoReducible);
            }
        }
    }
    public DateTime BuscarProximaFechaDisponible(DateTime desde, int duracionEnDias, int cantidad)
    {
        DateTime inicio = CalcularFechaInicioDisponible(desde);

        while (true)
        {
            if (EsVentanaDisponible(inicio, duracionEnDias, cantidad))
                return inicio;

            inicio = inicio.AddDays(1);
        }
    }
    
    private DateTime CalcularFechaInicioDisponible(DateTime desde)
    {
        if (!RangosEnUso.Any())
            return desde;

        DateTime finUltimoUso = RangosEnUso.Max(r => r.FechaFin.Date).AddDays(1);
        return finUltimoUso > desde ? finUltimoUso : desde;
    }
    
    private bool EsVentanaDisponible(DateTime inicio, int duracion, int cantidad)
    {
        DateTime fin = inicio.AddDays(duracion - 1);

        for (DateTime dia = inicio; dia <= fin; dia = dia.AddDays(1))
        {
            if (CantidadDeUsosPorDia(dia) + cantidad > Capacidad)
                return false;
        }

        return true;
    }

    public bool EsExclusivo()
    {
        return ProyectoAsociado != null;
    }

    public bool SeEstaUsando()
    {
        return CantidadDeTareasUsandolo > 0;
    }

    public void Actualizar(Recurso recursoActualizado)
    {
        ValidarIdentidad(recursoActualizado);
        
        ModificarNombre(recursoActualizado.Nombre);
        ModificarTipo(recursoActualizado.Tipo);
        ModificarDescripcion(recursoActualizado.Descripcion);
        CantidadDeTareasUsandolo = recursoActualizado.CantidadDeTareasUsandolo;
        ModificarCapacidad(recursoActualizado.Capacidad);
    }
    
    private int CantidadDeUsosPorDia(DateTime dia)
    {
        return RangosEnUso
            .Where(r => r.FechaInicio <= dia && r.FechaFin >= dia)
            .Sum(r => r.CantidadDeUsos);
    }
    
    private void ValidarAtributoNoVacio(string texto, string nombreAtributo)
    {
        if (string.IsNullOrWhiteSpace(texto))
        {
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.AtributoVacio, nombreAtributo));
        }
    }
    
    private void ValidarCapacidadMayorACero(int capacidad)
    {
        if (capacidad <= 0)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadRecursoInvalida);
        }
    }
    
    private void ValidarCapacidadMenorACapacidadRecurso(int capacidadRequerida)
    {
        if (capacidadRequerida > Capacidad)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.CapacidadRecursoInvalida);
        }
    }
    
    private void ValidarInicioAntesQueFin(DateTime fechaDesde, DateTime fechaHasta)
    {
        if (fechaDesde > fechaHasta)
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.FechaInicioRangoMayorQueFin);
        }
    }
    
    private void ValidarIdentidad(Recurso otroRecurso)
    {
        if (!Equals(otroRecurso))
        {
            throw new ExcepcionRecurso(MensajesErrorDominio.ActualizarEntidadNoCoincidente);
        }
    }
    
    private void ValidarRangoNoNulo(RangoDeUso? rango)
    {
        if (rango == null)
            throw new ExcepcionRecurso(MensajesErrorDominio.RangoNoEncontrado);
    }

    public override bool Equals(object? otro)
    {
        Recurso otroRecurso = otro as Recurso;
        return otroRecurso != null && Id == otroRecurso.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

    public override string ToString()
    {
        return $"Nombre: '{Nombre}', tipo: '{Tipo}', descripción: '{Descripcion}'";
    }
}