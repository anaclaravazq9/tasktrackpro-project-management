using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Dependencia
{
    public string Tipo { get; set; }
    public Tarea Tarea { get; set; }
    
    public Dependencia() { }
    public Dependencia(string tipo, Tarea tarea)
    {
        ValidarNoVacio(tipo);
        ValidarTipoValido(tipo);
        ValidarTareaNoNula(tarea);
        Tipo = tipo;
        Tarea = tarea;
    }

    private void ValidarNoVacio(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcepcionDominio(string.Format(MensajesErrorDominio.AtributoVacio, valor));
    }

    private void ValidarTipoValido(string valor)
    {
        if (valor != "SS" && valor != "FS")
        {
            throw new ExcepcionDominio(MensajesErrorDominio.TipoDependenciaInvalido);
        }
    }

    private void ValidarTareaNoNula(Tarea tarea)
    {
        if (tarea == null)
            throw new ExcepcionDominio(MensajesErrorDominio.TareaNula);
    }

    public override bool Equals(object obj)
    {
        if (obj is Dependencia otra)
            return Tipo == otra.Tipo && Tarea.Equals(otra.Tarea);
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Tipo, Tarea.Id);
    }

    public override string ToString()
    {
        return $"{Tarea.Titulo} ({Tipo})";
    }
}