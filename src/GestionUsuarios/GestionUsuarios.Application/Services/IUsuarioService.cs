using GestionUsuarios.Application.DTOs;

namespace GestionUsuarios.Application.Services;

public interface IUsuarioService
{
    Task<List<UsuarioDto>> ObtenerTodosAsync();
    Task<UsuarioDto?> ObtenerPorNombreAsync(string nombreUsuario);
    Task<UsuarioDto> CrearOEncontrarAsync(string nombreUsuario);

    /// <summary>Incrementa contadores cuando se le asigna un nuevo ítem al usuario.</summary>
    Task<UsuarioDto> RegistrarAsignacionAsync(string nombreUsuario, bool esAltaRelevancia);

    /// <summary>Mueve un ítem de "pendiente" a "completado" en los contadores del usuario.</summary>
    Task<UsuarioDto> RegistrarCompletadoAsync(string nombreUsuario, bool esAltaRelevancia);
}
