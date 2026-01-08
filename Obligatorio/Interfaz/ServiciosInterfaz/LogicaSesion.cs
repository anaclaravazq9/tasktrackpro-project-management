using Blazored.LocalStorage;
using Controladores;
using DTOs;

namespace Interfaz.ServiciosInterfaz;

public class LogicaSesion
{
    public event Action?
        SesionModificada; // para modificar el main layout (estructura principal) según permisos si se modifica al usuario logueado

    public UsuarioDTO? UsuarioLogueado { get; private set; }
    private const string CURRENT_USER = "current_user";

    private readonly ILocalStorageService _localStorage;
    private readonly ControladorUsuarios _controladorUsuarios;

    public LogicaSesion(ILocalStorageService localStorage, ControladorUsuarios controladorUsuarios)
    {
        _localStorage = localStorage;
        _controladorUsuarios = controladorUsuarios;
    }

    public async Task<bool> Login(string email, string contraseña)
    {
        try
        {
            UsuarioDTO usuarioLogueado = _controladorUsuarios.LogIn(email, contraseña);
            UsuarioLogueado = usuarioLogueado;
            await _localStorage.SetItemAsync(CURRENT_USER, usuarioLogueado);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> HaySesionActiva()
    {
        UsuarioDTO? usuario = await _localStorage.GetItemAsync<UsuarioDTO>(CURRENT_USER);
        UsuarioLogueado = usuario;

        return usuario is not null;
    }

    public async Task LogOut()
    {
        UsuarioLogueado = null;
        await _localStorage.RemoveItemAsync(CURRENT_USER);
    }

    public bool EsAdminSistema()
    {
        return UsuarioLogueado.EsAdministradorSistema;
    }

    public bool EsAdminProyecto()
    {
        return UsuarioLogueado.EsAdministradorProyecto;
    }
    
    public bool EsLider()
    {
        return UsuarioLogueado.EsLider;
    }


    public bool EsLiderProyecto()
    {
        return UsuarioLogueado.EsLider;
    }

    public async Task ActualizarSesion()
    {
        if (UsuarioLogueado != null)
        {
            UsuarioLogueado = _controladorUsuarios.ObtenerUsuarioPorId(UsuarioLogueado.Id);
            await _localStorage.SetItemAsync(CURRENT_USER, UsuarioLogueado);
            SesionModificada?.Invoke();
        }
    }
}