using Dominio;
using Microsoft.EntityFrameworkCore;
using Utilidades;

namespace Repositorios.ConfiguracionesEntidades;

public static class ConfiguracionUsuario
{
    public static void Configurar(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Nombre)
            .IsRequired();
        
        modelBuilder.Entity<Usuario>()
            .Property(u => u.Apellido)
            .IsRequired();
        
        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();
        
        modelBuilder.Entity<Usuario>()
            .Property<string>("_contrasenaEncriptada") 
            .HasColumnName("ContrasenaEncriptada")   
            .IsRequired();
        
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Notificaciones)
            .WithOne()
            .HasForeignKey("UsuarioId") 
            .IsRequired();
        
        string contrasenaEncriptada = UtilidadesContrasena.ValidarYEncriptarContrasena("TaskTrackPro@2025");
        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nombre = "Admin",
                Apellido = "Sistema",
                Email = "admin@sistema.com",
                FechaNacimiento = new DateTime(1999, 01, 01),
                EsAdministradorSistema = true,
                ContrasenaEncriptada = contrasenaEncriptada
            }); 
    }
}