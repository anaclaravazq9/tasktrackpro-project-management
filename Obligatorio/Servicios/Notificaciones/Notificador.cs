using Dominio;
using IRepositorios;
using IServicios;

namespace Servicios.Notificaciones;

public class Notificador : INotificador
{
    private readonly IRepositorioUsuarios _repositorioUsuarios;
    
    public Notificador(IRepositorioUsuarios repositorioUsuarios)
    {
        _repositorioUsuarios = repositorioUsuarios;
    }
    public void NotificarUno(Usuario usuario, string mensaje)
    {
        NotificarMuchos(new List<Usuario> { usuario }, mensaje);
    }

    public void NotificarMuchos(List<Usuario> usuarios, string mensaje)
    {
        usuarios.ForEach(u => u.RecibirNotificacion(mensaje));
        usuarios.ForEach(u => _repositorioUsuarios.Actualizar(u));
    }
}