using Dominio;

namespace IServicios;

public interface INotificador
{
    void NotificarUno(Usuario usuario, string mensaje);
    void NotificarMuchos(List<Usuario> usuarios, string mensaje);
}