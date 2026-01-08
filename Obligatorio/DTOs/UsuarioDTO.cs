using System.ComponentModel.DataAnnotations;
using Dominio;

namespace DTOs;

public class UsuarioDTO
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre no puede ser vacío.")]
    public string Nombre { get; set; }

    [Required(ErrorMessage = "El apellido no puede ser vacío.")]
    public string Apellido { get; set; }

    [Required(ErrorMessage = "La fecha de nacimiento es obligatoria.")]
    public DateTime FechaNacimiento { get; set; }

    [Required(ErrorMessage = "El email no puede ser vacío")]
    [EmailAddress(ErrorMessage = "Formato de email inválido.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "La contraseña no puede ser vacía.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*()_+\-=\[\]{};':""\\|,.<>\/?]).{8,}$",
        ErrorMessage =
            "La contraseña debe tener al menos 8 caracteres, una minúscula, una mayúscula, un número y un carácter especial.")]
    public string Contrasena { get; set; }

    public bool EsAdministradorSistema { get; set; }
    public bool EsAdministradorProyecto { get; set; }
    
    public bool EsLider { get; set; }

    public List<NotificacionDTO> Notificaciones { get; private set; }
    public bool EstaAdministrandoUnProyecto { get; set; } = false;

    public int CantidadProyectosAsignados { get; set; } = 0;
    
    public int CantidadProyectosLiderando { get; set; } = 0;

    public static UsuarioDTO DesdeEntidad(Usuario usuario)
    {
        return new UsuarioDTO
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Apellido = usuario.Apellido,
            FechaNacimiento = usuario.FechaNacimiento,
            Email = usuario.Email,
            EsAdministradorSistema = usuario.EsAdministradorSistema,
            EsAdministradorProyecto = usuario.EsAdministradorProyecto,
            EsLider = usuario.EsLider,
            Notificaciones = usuario.Notificaciones.Select(NotificacionDTO.DesdeEntidad).ToList(),
            EstaAdministrandoUnProyecto = usuario.EstaAdministrandoUnProyecto,
            CantidadProyectosAsignados = usuario.CantidadProyectosAsignados,
            CantidadProyectosLiderando = usuario.CantidadProyectosLiderando,
            // la contraseña se omite por seguridad.
        };
    }

    public static UsuarioDTO DesdeListarDTO(UsuarioListarDTO dto)
    {
        return new UsuarioDTO
        {
            Id = dto.Id,
            Nombre = dto.Nombre,
            Apellido = dto.Apellido,
            Email = dto.Email
        };
    }
}