using Excepciones;
using Excepciones.MensajesError;

namespace Dominio;

public class Notificacion
{
    public int Id { get; set; }
    public string Mensaje { get; private set; }
    public DateTime Fecha { get; private set; }

    public Notificacion()
    {
    }

    public Notificacion(string mensaje)
    {
        if (string.IsNullOrWhiteSpace(mensaje))
            throw new ExcepcionDominio(MensajesErrorDominio.MensajeNotificacionVacio);

        Mensaje = mensaje;
        Fecha = DateTime.Today;
    }
}