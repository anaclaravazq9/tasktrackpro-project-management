using Dominio;

namespace DTOs;

public class NotificacionDTO
{
    public int Id { get; private set; }

    public string Mensaje { get; private set; }
    public DateTime Fecha { get; private set; }

    public static NotificacionDTO DesdeEntidad(Notificacion notificacion)
    {
        return new NotificacionDTO
        {
            Id = notificacion.Id,
            Mensaje = notificacion.Mensaje,
            Fecha = notificacion.Fecha
        };
    }
}