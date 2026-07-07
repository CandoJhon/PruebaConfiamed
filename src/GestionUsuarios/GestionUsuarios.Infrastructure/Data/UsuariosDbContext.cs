using GestionUsuarios.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace GestionUsuarios.Infrastructure.Data;

/// <summary>
/// Contexto de EF Core usando el proveedor InMemory.
/// Se eligió persistencia en memoria porque el enunciado no exige
/// explícitamente un motor de base de datos configurado; esto permite
/// centrar la evaluación en la lógica de negocio y la arquitectura.
/// Para pasar a un motor real (SQL Server / PostgreSQL) solo hace falta
/// cambiar el proveedor registrado en Program.cs.
/// </summary>
public class UsuariosDbContext : DbContext
{
    public UsuariosDbContext(DbContextOptions<UsuariosDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.NombreUsuario).IsRequired();
            entity.HasIndex(u => u.NombreUsuario).IsUnique();
        });
    }
}
