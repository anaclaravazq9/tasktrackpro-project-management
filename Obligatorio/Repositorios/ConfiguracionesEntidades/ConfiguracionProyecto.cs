using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionProyecto
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Proyecto>()
            .Property(p => p.Nombre)
            .IsRequired();

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.Descripcion)
            .IsRequired()
            .HasMaxLength(400);

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.FechaInicio)
            .IsRequired();

        modelBuilder.Entity<Proyecto>()
            .Property(p => p.FechaFinMasTemprana)
            .IsRequired();
        
        modelBuilder.Entity<Proyecto>()
            .HasOne(p => p.Administrador)
            .WithOne()
            .HasForeignKey<Proyecto>("AdministradorId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Proyecto>()
            .HasOne(p => p.Lider)
            .WithMany()
            .HasForeignKey("LiderId")
            .OnDelete(DeleteBehavior.Restrict); 
        
        modelBuilder.Entity<Proyecto>()
            .HasMany(p => p.Miembros)
            .WithMany();
        
        modelBuilder.Entity<Proyecto>()
            .HasMany(p => p.Tareas)
            .WithOne()
            .HasForeignKey("ProyectoId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}