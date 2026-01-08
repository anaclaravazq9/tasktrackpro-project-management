using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionTarea
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tarea>().Property(t => t.Titulo)
            .IsRequired();

        modelBuilder.Entity<Tarea>().Property(t => t.Descripcion)
            .IsRequired();

        modelBuilder.Entity<Tarea>().Property(t => t.DuracionEnDias)
            .IsRequired();
        
        modelBuilder.Entity<Tarea>()
            .HasCheckConstraint("CK_Tarea_DuracionMayorACero", "[DuracionEnDias] > 0");

        modelBuilder.Entity<Tarea>().Property(t => t.FechaInicioMasTemprana)
            .IsRequired();

        modelBuilder.Entity<Tarea>().Property(t => t.FechaFinMasTemprana)
            .IsRequired();
        
        modelBuilder.Entity<Tarea>()
            .HasMany(t => t.UsuariosAsignados)
            .WithMany();

        modelBuilder.Entity<Tarea>()
            .HasMany(t => t.RecursosNecesarios)
            .WithOne()
            .HasForeignKey("TareaId")
            .OnDelete(DeleteBehavior.Cascade);
        
        modelBuilder.Entity<Tarea>()
            .HasMany(t => t.Dependencias)  
            .WithOne()                           
            .HasForeignKey("TareaDuenaId")       
            .OnDelete(DeleteBehavior.Cascade); 
    }
}