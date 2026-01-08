using Dominio;
using IRepositorios;
using Microsoft.EntityFrameworkCore;

namespace Repositorios;

public class RepositorioUsuarios : IRepositorioUsuarios
{
    private SqlContext _contexto;

    public RepositorioUsuarios(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Usuario objeto)
    {
        _contexto.Usuarios.Add(objeto);
        _contexto.SaveChanges();
    }

    public Usuario ObtenerPorId(int id)
    {
        return _contexto.Usuarios.
            Include(u=>u.Notificaciones)
            .FirstOrDefault(usuario => usuario.Id == id);
    }
    
    public Usuario ObtenerUsuarioPorEmail(string email)
    {
        return _contexto.Usuarios.
            Include(u=> u.Notificaciones)
            .FirstOrDefault(usuario => usuario.Email == email);
    }

    public void Eliminar(int id)
    {
        Usuario usuarioAEliminar = _contexto.Usuarios.FirstOrDefault(usuario => usuario.Id == id);
        _contexto.Usuarios.Remove(usuarioAEliminar);
        _contexto.SaveChanges();
    }
    
    public List<Usuario> ObtenerTodos()
    {
        return _contexto.Usuarios
            .Include(u=>u.Notificaciones)
            .ToList();
    }
    
    public void Actualizar(Usuario usuario)
    {
        Usuario usuarioContexto = ObtenerPorId(usuario.Id);
        
        if (usuarioContexto != null)
        {
            usuarioContexto.Actualizar(usuario);
            SincronizarNotificaciones(usuarioContexto, usuario);
            _contexto.SaveChanges();
        }
    }
    
    private void SincronizarNotificaciones(Usuario usuarioContexto, Usuario usuario)
    {
        EliminarNotificacionesNoIncluidas(usuarioContexto, usuario);
        AgregarNotificacionesNuevas(usuarioContexto, usuario);
    }
    
    private void EliminarNotificacionesNoIncluidas(Usuario usuarioContexto, Usuario usuario)
    {
        foreach (Notificacion notificacionExistente in usuarioContexto.Notificaciones.ToList())
        {
            if (!usuario.Notificaciones.Any(n => n.Id == notificacionExistente.Id))
            {
                usuarioContexto.Notificaciones.Remove(notificacionExistente);
            }
        }
    }
    
    private void AgregarNotificacionesNuevas(Usuario usuarioContexto, Usuario usuario)
    {
        foreach (Notificacion notificacionNueva in usuario.Notificaciones)
        {
            if (!usuarioContexto.Notificaciones.Any(n => n.Id == notificacionNueva.Id))
            {
                usuarioContexto.Notificaciones.Add(notificacionNueva);
            }
        }
    }
}