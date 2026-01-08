using Dominio;
using Microsoft.EntityFrameworkCore;
using Repositorios.ConfiguracionesEntidades;

namespace Repositorios;

public class SqlContext : DbContext
{
    public DbSet<Recurso> Recursos { get; set; }
    public DbSet<Proyecto> Proyectos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    
    public SqlContext(DbContextOptions<SqlContext> opciones) : base(opciones)
    { 
        if (Database.IsRelational())
        {
            Database.Migrate();
        }
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfiguracionDependencia.Configurar(modelBuilder);
        ConfiguracionNotificacion.Configurar(modelBuilder);
        ConfiguracionProyecto.Configurar(modelBuilder);
        ConfiguracionRecurso.Configurar(modelBuilder);
        ConfiguracionTarea.Configurar(modelBuilder);
        ConfiguracionUsuario.Configurar(modelBuilder);
        ConfiguracionRangoDeUso.Configurar(modelBuilder);
        ConfiguracionRecursoNecesario.Configurar(modelBuilder);
    }
}