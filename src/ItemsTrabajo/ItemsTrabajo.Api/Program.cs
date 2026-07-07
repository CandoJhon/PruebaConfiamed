using ItemsTrabajo.Application.Clients;
using ItemsTrabajo.Application.Services;
using ItemsTrabajo.Infrastructure.Data;
using ItemsTrabajo.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Persistencia en memoria (misma decisión que en GestionUsuarios.Api).
var connectionString = builder.Configuration.GetConnectionString("ItemsTrabajoDb")
    ?? "Data Source=itemstrabajo.db";

builder.Services.AddDbContext<ItemsTrabajoDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddScoped<IItemTrabajoRepository, ItemTrabajoRepository>();
builder.Services.AddScoped<IItemTrabajoService, ItemTrabajoService>();
builder.Services.AddScoped<IDistribucionService, DistribucionService>();

// Cliente HTTP tipado hacia el microservicio de Gestión de Usuarios.
// La URL base se lee de appsettings.json -> Servicios:GestionUsuariosBaseUrl.
var gestionUsuariosBaseUrl = builder.Configuration["Servicios:GestionUsuariosBaseUrl"]
    ?? "http://localhost:5100";

builder.Services.AddHttpClient<IGestionUsuariosClient, GestionUsuariosClient>(client =>
{
    client.BaseAddress = new Uri(gestionUsuariosBaseUrl);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ítems de Trabajo API", Version = "v1" });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "ok", servicio = "ItemsTrabajo" }));

// creación de eesquema de base de datos 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ItemsTrabajoDbContext> (); 
    db.Database.EnsureCreated();
}

app.Run();

public partial class Program { }
