using Dominio;
using IRepositorios;
using Microsoft.EntityFrameworkCore;

namespace Repositorios;

public class RepositorioProyectos : IRepositorioProyectos
{
    private SqlContext _contexto;

    public RepositorioProyectos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Proyecto objeto)
    {
        _contexto.Proyectos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Proyecto ObtenerPorId(int id)
    {
        return _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p => p.Lider)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
                .ThenInclude(t => t.Dependencias)
            .Include(p => p.Tareas)
                .ThenInclude(t => t.UsuariosAsignados)
            .Include(p=>p.Tareas)
                .ThenInclude(t => t.RecursosNecesarios)
                    .ThenInclude(r => r.Recurso)
            .FirstOrDefault(proyecto => proyecto.Id == id);
    }

    public void Eliminar(int id)
    {
        Proyecto proyecto = _contexto.Proyectos.FirstOrDefault(proyecto => proyecto.Id == id);
        _contexto.Proyectos.Remove(proyecto);
        _contexto.SaveChanges();
    }

    public List<Proyecto> ObtenerTodos()
    {
        return _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p=>p.Lider)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
                .ThenInclude(t => t.Dependencias)
            .Include(p => p.Tareas)
                .ThenInclude(t => t.UsuariosAsignados)
            .Include(p => p.Tareas)
                .ThenInclude(t => t.RecursosNecesarios)
                .ThenInclude(r => r.Recurso)
            .ToList();
    }

    public void Actualizar(Proyecto proyecto)
    {
        Proyecto proyectoContexto = _contexto.Proyectos
            .Include(p => p.Administrador)
            .Include(p =>p.Lider)
            .Include(p => p.Miembros)
            .Include(p => p.Tareas)
            .FirstOrDefault(p => p.Id == proyecto.Id);

        if (proyectoContexto != null)
        {
            proyectoContexto.Actualizar(proyecto);
            SincronizarAdministradorDelProyecto(proyecto, proyectoContexto);
            SincronizarLiderDelProyecto(proyecto, proyectoContexto);
            SincronizarMiembros(proyecto, proyectoContexto);
            SincronizarTareas(proyecto, proyectoContexto);
            _contexto.SaveChanges();
        }
    }
    
    public void ActualizarTarea(Tarea tarea)
    {
        Tarea tareaContexto = _contexto.Set<Tarea>()
            .Include(t => t.RecursosNecesarios)
                .ThenInclude(r => r.Recurso)
            .Include(t => t.UsuariosAsignados)
            .Include(t => t.Dependencias)
            .FirstOrDefault(t => t.Id == tarea.Id);
        
        if (tareaContexto != null)
        {
            tareaContexto.Actualizar(tarea);
            SincronizarUsuariosAsignados(tarea, tareaContexto);
            SincronizarRecursosNecesarios(tarea, tareaContexto);
            SincronizarDependencias(tarea, tareaContexto);
            _contexto.SaveChanges();
        }
    }
    
    private void SincronizarAdministradorDelProyecto(Proyecto proyecto, Proyecto proyectoContexto)
    {
        if (!proyecto.Administrador.Equals(proyectoContexto.Administrador))
        {
            Usuario nuevoAdmin = _contexto.Usuarios.FirstOrDefault(u => u.Id == proyecto.Administrador.Id);
            proyectoContexto.AsignarNuevoAdministrador(nuevoAdmin);
        }
    }
    
    private void SincronizarLiderDelProyecto(Proyecto proyecto, Proyecto proyectoContexto)
    {
        if (proyecto.Lider == null)
        {
            proyectoContexto.Lider = null;
        }
        else if (!proyecto.Lider.Equals(proyectoContexto.Lider))
        {
            Usuario nuevoLider = _contexto.Usuarios.FirstOrDefault(u => u.Id == proyecto.Lider.Id);
            proyectoContexto.AsignarLider(nuevoLider);
        }
    }
    
    private void SincronizarMiembros(Proyecto proyecto, Proyecto proyectoContexto)
    {
        EliminarMiembrosNoIncluidos(proyecto, proyectoContexto);
        AgregarMiembrosNuevos(proyecto, proyectoContexto);
    }

    private void EliminarMiembrosNoIncluidos(Proyecto proyecto, Proyecto proyectoContexto)
    {
        List<Usuario> miembrosAEliminar = proyectoContexto.Miembros
            .Where(m => !proyecto.Miembros.Contains(m))
            .ToList();
        
        foreach (Usuario miembro in miembrosAEliminar)
        {
            proyectoContexto.Miembros.Remove(miembro);
        }
    }
    
    private void AgregarMiembrosNuevos(Proyecto proyecto, Proyecto proyectoContexto)
    {
        foreach (Usuario miembro in proyecto.Miembros)
        {
            if (!proyectoContexto.Miembros.Contains(miembro))
            {
                Usuario miembroContexto = _contexto.Usuarios.FirstOrDefault(u => u.Id == miembro.Id);
                if (miembroContexto != null)
                {
                    proyectoContexto.Miembros.Add(miembroContexto);
                }
            }
        }
    }
    
    private void SincronizarTareas(Proyecto proyecto, Proyecto proyectoContexto) 
    {
        EliminarTareasNoIncluidas(proyecto, proyectoContexto);
        AgregarTareasNuevas(proyecto, proyectoContexto);
    }
    
    private void EliminarTareasNoIncluidas(Proyecto proyecto, Proyecto proyectoContexto)
    {
        List<Tarea> tareasAEliminar = proyectoContexto.Tareas
            .Where(t => !proyecto.Tareas.Contains(t))
            .ToList();

        foreach (Tarea tarea in tareasAEliminar)
        {
            proyectoContexto.Tareas.Remove(tarea);
            _contexto.Set<Tarea>().Remove(tarea);
        }
    }
    
    private void AgregarTareasNuevas(Proyecto proyecto, Proyecto proyectoContexto)
    {
        foreach (Tarea tarea in proyecto.Tareas)
        {
            if (!proyectoContexto.Tareas.Contains(tarea))
            {
                proyectoContexto.Tareas.Add(tarea);
            }
        }
    }
    private void SincronizarUsuariosAsignados(Tarea tarea, Tarea tareaContexto)
    {
        EliminarUsuariosAsignadosNoIncluidos(tarea, tareaContexto);
        AgregarUsuariosAsignadosNuevos(tarea, tareaContexto);
    }

    private void EliminarUsuariosAsignadosNoIncluidos(Tarea tarea, Tarea tareaContexto)
    {
        List<Usuario> usuariosAEliminar = tareaContexto.UsuariosAsignados
            .Where(u => !tarea.UsuariosAsignados.Contains(u))
            .ToList();
        
        foreach (Usuario usuario in usuariosAEliminar)
        {
            tareaContexto.UsuariosAsignados.Remove(usuario);
        }
    }
    
    private void AgregarUsuariosAsignadosNuevos(Tarea tarea, Tarea tareaContexto)
    {
        foreach (Usuario usuario in tarea.UsuariosAsignados)
        {
            if (!tareaContexto.UsuariosAsignados.Contains(usuario))
            {
                Usuario usuarioContexto = _contexto.Usuarios.FirstOrDefault(u => u.Id == usuario.Id);
                if (usuarioContexto != null)
                {
                    tareaContexto.UsuariosAsignados.Add(usuarioContexto);
                }
            }
        }
    }
    
    private void SincronizarRecursosNecesarios(Tarea tarea, Tarea tareaContexto)
    {
        EliminarRecursosNoIncluidos(tarea, tareaContexto);
        AgregarRecursosNuevos(tarea, tareaContexto);
    }
    
    private void EliminarRecursosNoIncluidos(Tarea tarea, Tarea tareaContexto)
    {
        List<RecursoNecesario> recursosAEliminar = tareaContexto.RecursosNecesarios
            .Where(r => !tarea.RecursosNecesarios.Contains(r))
            .ToList();
        
        foreach (RecursoNecesario recurso in recursosAEliminar)
        {
            tareaContexto.RecursosNecesarios.Remove(recurso);
        }
    }

    private void AgregarRecursosNuevos(Tarea tarea, Tarea tareaContexto)
    {
        foreach (RecursoNecesario recursoNecesario in tarea.RecursosNecesarios)
        {
            if (!tareaContexto.RecursosNecesarios.Contains(recursoNecesario))
            {
                Recurso recursoContexto = _contexto.Recursos.FirstOrDefault(r => r.Id == recursoNecesario.Recurso.Id);
                int cantidad = recursoNecesario.Cantidad;
                if (recursoContexto != null)
                {
                    tareaContexto.RecursosNecesarios.Add(new RecursoNecesario()
                    {
                        Recurso = recursoContexto,
                        Cantidad = cantidad
                    });
                }
            }
        }
    }
    
    private void SincronizarDependencias(Tarea tarea, Tarea tareaContexto)
    {
        EliminarDependenciasNoIncluidas(tarea, tareaContexto);
        AgregarDependenciasNuevas(tarea, tareaContexto);
    }
    
    private void EliminarDependenciasNoIncluidas(Tarea tarea, Tarea tareaContexto)
    {
        List<Dependencia> dependenciasAEliminar = tareaContexto.Dependencias
            .Where(d => !tarea.Dependencias.Contains(d))
            .ToList();
        
        foreach (Dependencia dependencia in dependenciasAEliminar)
        {
            tareaContexto.Dependencias.Remove(dependencia);
            _contexto.Set<Dependencia>().Remove(dependencia);
        }
    }

    private void AgregarDependenciasNuevas(Tarea tarea, Tarea tareaContexto)
    {
        foreach (var dependencia in tarea.Dependencias)
        {
            if (!tareaContexto.Dependencias.Any(d => d.Tarea.Id == dependencia.Tarea.Id && d.Tipo == dependencia.Tipo))
            {
                Tarea tareaDependiente = _contexto.Set<Tarea>().FirstOrDefault(t => t.Id == dependencia.Tarea.Id);
                if (tareaDependiente != null)
                {
                    tareaContexto.Dependencias.Add(new Dependencia
                    {
                        Tipo = dependencia.Tipo,
                        Tarea = tareaDependiente
                    });
                }
            }
        }
    }
}