using Blazored.LocalStorage;
using Controladores;
using Dominio;
using Interfaz.Components;
using Interfaz.ServiciosInterfaz;
using Microsoft.EntityFrameworkCore;
using Repositorios;
using IRepositorios;
using IServicios;
using IServicios.IGestores;
using Servicios.CaminoCritico;
using Servicios.Exportacion;
using Servicios.Gestores;
using Servicios.Notificaciones;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContextFactory<SqlContext>(opciones =>
    opciones.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        providerOptions => providerOptions.EnableRetryOnFailure()));

builder.Services.AddScoped<IRepositorioUsuarios, RepositorioUsuarios>();
builder.Services.AddScoped<IRepositorioProyectos, RepositorioProyectos>();
builder.Services.AddScoped<IRepositorio<Recurso>, RepositorioRecursos>();

builder.Services.AddScoped<IGestorUsuarios, GestorUsuarios>();
builder.Services.AddScoped<IGestorProyectos, GestorProyectos>();
builder.Services.AddScoped<IGestorRecursos, GestorRecursos>();
builder.Services.AddScoped<IGestorTareas, GestorTareas>();
builder.Services.AddScoped<INotificador, Notificador>();
builder.Services.AddScoped<ICalculadorCaminoCritico, CaminoCritico>();
builder.Services.AddScoped<IExportadorProyectos, ExportadorCsv>();
builder.Services.AddScoped<IExportadorProyectos, ExportadorJson>();
builder.Services.AddScoped<IExportadorProyectosFactory, ExportadorProyectosFactory>();

builder.Services.AddScoped<ControladorTareas>();
builder.Services.AddScoped<ControladorProyectos>();
builder.Services.AddScoped<ControladorRecursos>();
builder.Services.AddScoped<ControladorUsuarios>();
builder.Services.AddScoped<ControladorExportacion>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddScoped<LogicaSesion>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapControllers();

app.Run();