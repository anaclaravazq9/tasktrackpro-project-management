using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionRangoDeUso
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RangoDeUso>().Property(r => r.FechaInicio)
            .IsRequired();
        
        modelBuilder.Entity<RangoDeUso>().Property(r => r.FechaFin)
            .IsRequired();
        
        modelBuilder.Entity<RangoDeUso>().Property(r => r.CantidadDeUsos)
            .IsRequired();
    }
}