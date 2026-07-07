using GestionUsuarios.Domain.Entities;

namespace GestionUsuarios.Infrastructure.Repositories;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> ObtenerPorNombreAsync(string nombreUsuario);
    Task<Usuario> CrearAsync(Usuario usuario);
    Task ActualizarAsync(Usuario usuario);
}
