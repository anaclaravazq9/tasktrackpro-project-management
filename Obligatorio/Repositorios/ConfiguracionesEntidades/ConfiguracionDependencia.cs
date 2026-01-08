using Dominio;
using Microsoft.EntityFrameworkCore;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionDependencia
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dependencia>().Property(d => d.Tipo)
            .IsRequired()
            .HasMaxLength(2);
        modelBuilder.Entity<Dependencia>().HasCheckConstraint("CK_Dependencia_Tipo", "[Tipo] IN ('SS', 'FS')");
        
        modelBuilder.Entity<Dependencia>()
            .Property<int>("TareaDuenaId");  
        
        modelBuilder.Entity<Dependencia>()
            .HasKey("TareaDuenaId", "TareaId", "Tipo");

        modelBuilder.Entity<Dependencia>()
            .HasOne<Tarea>()              
            .WithMany(t => t.Dependencias)    
            .HasForeignKey("TareaDuenaId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Dependencia>()
            .HasOne(d => d.Tarea)             
            .WithMany()                       
            .HasForeignKey("TareaId")
            .OnDelete(DeleteBehavior.Restrict);
    }
}