using GestionUsuarios.Domain.Entities;
using GestionUsuarios.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GestionUsuarios.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly UsuariosDbContext _context;

    public UsuarioRepository(UsuariosDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
        => await _context.Usuarios.AsNoTracking().ToListAsync();

    public async Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario)
        => await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

    public async Task<Usuario> CrearAsync(Usuario usuario)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();
        return usuario;
    }

    public async Task ActualizarAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}
