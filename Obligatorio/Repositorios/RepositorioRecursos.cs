using Dominio;
using IRepositorios;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Repositorios;

public class RepositorioRecursos : IRepositorio<Recurso>
{
    private SqlContext _contexto;

    public RepositorioRecursos(SqlContext contexto)
    {
        _contexto = contexto;
    }

    public void Agregar(Recurso objeto)
    {
        _contexto.Recursos.Add(objeto);
        _contexto.SaveChanges();
    }

    public Recurso ObtenerPorId(int id)
    {
        return _contexto.Recursos.
            Include(r => r.ProyectoAsociado).
            Include(r => r.RangosEnUso)
            .FirstOrDefault(recurso => recurso.Id == id);
    }

    public void Eliminar(int id)
    {
        Recurso recursoAEliminar = _contexto.Recursos.FirstOrDefault(recurso => recurso.Id == id);
        _contexto.Remove(recursoAEliminar);
        _contexto.SaveChanges();
    }

    public List<Recurso> ObtenerTodos()
    {
        return _contexto.Recursos.
            Include(r=> r.ProyectoAsociado)
            .Include(r => r.RangosEnUso)
            .ToList();
    }
    
    public void Actualizar(Recurso recurso)
    {
        Recurso recursoContexto = ObtenerPorId(recurso.Id);
        if (recursoContexto != null)
        {
            recursoContexto.Actualizar(recurso);
            SincronizarProyectoAsociado(recurso, recursoContexto);
            SincronizarRangosEnUso(recurso, recursoContexto);
            _contexto.SaveChanges();
        }
    }

    private void SincronizarProyectoAsociado(Recurso recurso, Recurso recursoContexto)
    {
        if (recurso.ProyectoAsociado != null)
        {
            Proyecto proyectoAsociadoContexto = _contexto.Proyectos
                .FirstOrDefault(p => p.Id == recurso.ProyectoAsociado.Id);
            AsociarRecursoAProyectoSiNoEsExclusivo(recursoContexto, proyectoAsociadoContexto);
        }
    }
    
    private void AsociarRecursoAProyectoSiNoEsExclusivo(Recurso recursoContexto, Proyecto proyectoAsociadoContexto)
    {
        if (!recursoContexto.EsExclusivo())
        {
            recursoContexto.AsociarAProyecto(proyectoAsociadoContexto);
        }
    }
    
    private void SincronizarRangosEnUso(Recurso recurso, Recurso recursoContexto)
    {
        EliminarRangosEnUsoNoIncluidos(recursoContexto, recurso);
        AgregarRangosEnUsoNuevos(recursoContexto, recurso);
    }

    private void AgregarRangosEnUsoNuevos(Recurso recursoContexto, Recurso recurso)
    {
        List<RangoDeUso> rangosNuevos = recurso.RangosEnUso.ToList(); 
        foreach (RangoDeUso rangoNuevo in rangosNuevos)
        {
            if (rangoNuevo.Id == 0 || 
                !recursoContexto.RangosEnUso.Any(rangoExistente => rangoExistente.Id == rangoNuevo.Id))
            {
                recursoContexto.RangosEnUso.Add(rangoNuevo);
            }
        }
    }

    private void EliminarRangosEnUsoNoIncluidos(Recurso recursoContexto, Recurso recurso)
    {
        foreach (RangoDeUso rangoExistente in recursoContexto.RangosEnUso.ToList())
        {
            if (!recurso.RangosEnUso.Any(rango => rango.Id == rangoExistente.Id))
            {
                recursoContexto.RangosEnUso.Remove(rangoExistente);
            }
        }
    }
}