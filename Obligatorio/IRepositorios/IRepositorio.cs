namespace IRepositorios;

public interface IRepositorio<T>
{
    void Agregar(T objeto);
    T ObtenerPorId(int id);
    void Eliminar(int id);
    List<T> ObtenerTodos();
    void Actualizar(T objeto);
}