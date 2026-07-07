using ItemsTrabajo.Application.DTOs;

namespace ItemsTrabajo.Application.Clients;

/// <summary>
/// Abstrae la comunicación REST hacia el microservicio de Gestión de Usuarios.
/// Mantener esta interfaz permite testear la lógica de distribución con un
/// mock, sin necesidad de levantar el otro microservicio.
/// </summary>
public interface IGestionUsuariosClient
{
    Task<List<UsuarioResumenDto>> ObtenerUsuariosAsync();
    Task RegistrarAsignacionAsync(string nombreUsuario, bool esAltaRelevancia);
    Task RegistrarCompletadoAsync(string nombreUsuario, bool esAltaRelevancia);
}
