using Dominio;

namespace IRepositorios;

public interface IRepositorioProyectos : IRepositorio<Proyecto>
{
    void ActualizarTarea(Tarea tarea);
}