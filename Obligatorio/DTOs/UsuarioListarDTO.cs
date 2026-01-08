using Dominio;

namespace DTOs;

public class UsuarioListarDTO
{
    public int Id { get; set; }

    public string Nombre { get; set; }

    public string Apellido { get; set; }

    public DateTime FechaNacimiento { get; set; }

    public string Email { get; set; }

    public bool EsAdministradorSistema { get; set; }
    public bool EsAdministradorProyecto { get; set; }
    
    public bool EsLider { get; set; }
    
    public int CantidadProyectosLiderando { get; set; }

    public List<NotificacionDTO> Notificaciones { get; private set; }

    public static UsuarioListarDTO DesdeEntidad(Usuario usuario)
    {
        return new UsuarioListarDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            FechaNacimiento = usuario.FechaNacimiento,
            Email = usuario.Email,
            EsAdministradorSistema = usuario.EsAdministradorSistema,
            EsAdministradorProyecto = usuario.EsAdministradorProyecto,
            EsLider = usuario.EsLider,
            CantidadProyectosLiderando = usuario.CantidadProyectosLiderando,
            Notificaciones = usuario.Notificaciones.Select(NotificacionDTO.DesdeEntidad).ToList(),
        };
    }

    public static UsuarioListarDTO DesdeDTO(UsuarioDTO dto)
    {
        return new UsuarioListarDTO
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email
        };
    }
}