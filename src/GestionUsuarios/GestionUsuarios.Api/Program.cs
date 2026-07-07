using GestionUsuarios.Application.Services;
using GestionUsuarios.Infrastructure.Data;
using GestionUsuarios.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Persistencia en memoria (ver nota en UsuariosDbContext sobre esta decisión).
var connectionString = builder.Configuration.GetConnectionString("GestionUsuariosDb")
    ?? "Data Source=gestionusuarios.db";

builder.Services.AddDbContext<UsuariosDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Gestión de Usuarios API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

// Endpoint de health check simple, útil para monitoreo básico entre microservicios.
app.MapGet("/health", () => Results.Ok(new { status = "ok", servicio = "GestionUsuarios" }));

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<UsuariosDbContext>();
    db.Database.EnsureCreated();
}

app.Run();

// Necesario para exponer la clase Program a los proyectos de test (WebApplicationFactory).
public partial class Program { }
