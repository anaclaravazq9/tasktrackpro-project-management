using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionNotificacion
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notificacion>().Property(n => n.Mensaje)
            .IsRequired();
        
        modelBuilder.Entity<Notificacion>().Property(n => n.Fecha)
            .IsRequired();
    }
    
}