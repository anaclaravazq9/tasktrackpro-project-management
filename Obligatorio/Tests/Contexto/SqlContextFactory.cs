using Microsoft.EntityFrameworkCore;
using Repositorios;

namespace Tests.Contexto;

public static class SqlContextFactory
{
    public static SqlContext CrearContextoEnMemoria() 
    {
        var optionsBuilder = new DbContextOptionsBuilder<SqlContext>();
        optionsBuilder.UseInMemoryDatabase("TestDB"); 
        SqlContext contexto = new SqlContext(optionsBuilder.Options);
        
        contexto.Database.EnsureCreated();
        return contexto;
    }
}