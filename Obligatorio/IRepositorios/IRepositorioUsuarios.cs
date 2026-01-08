using Dominio;

namespace IRepositorios;

public interface IRepositorioUsuarios : IRepositorio<Usuario>
{
    Usuario ObtenerUsuarioPorEmail(string email);
    
}