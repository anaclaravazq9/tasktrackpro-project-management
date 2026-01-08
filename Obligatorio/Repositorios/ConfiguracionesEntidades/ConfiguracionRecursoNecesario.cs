using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionRecursoNecesario
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RecursoNecesario>().Property(r => r.Cantidad)
            .IsRequired()
            .HasMaxLength(100);
        
        modelBuilder.Entity<RecursoNecesario>()
            .HasOne(rn => rn.Recurso)
            .WithMany() 
            .HasForeignKey("RecursoId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}