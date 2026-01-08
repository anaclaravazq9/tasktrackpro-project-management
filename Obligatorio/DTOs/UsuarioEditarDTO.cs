namespace DTOs;

public class UsuarioEditarDTO
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string Email { get; set; }
    public bool EsAdministradorSistema { get; set; }
    public bool EsAdministradorProyecto { get; set; }
    public string? ContrasenaAutogenerada { get; set; }
}