using System.Net.Http.Json;
using ItemsTrabajo.Application.DTOs;

namespace ItemsTrabajo.Application.Clients;

public class GestionUsuariosClient : IGestionUsuariosClient
{
    private readonly HttpClient _httpClient;

    public GestionUsuariosClient(HttpClient httpClient)
    {
        // BaseAddress se configura en Program.cs vía appsettings (Services:GestionUsuarios).
        _httpClient = httpClient;
    }

    public async Task<List<UsuarioResumenDto>> ObtenerUsuariosAsync()
    {
        var usuarios = await _httpClient.GetFromJsonAsync<List<UsuarioResumenDto>>("api/usuarios");
        return usuarios ?? new List<UsuarioResumenDto>();
    }

    public async Task RegistrarAsignacionAsync(string nombreUsuario, bool esAltaRelevancia)
    {
        // Aseguramos que el usuario exista en GestionUsuarios antes de asignarle un ítem.
        await _httpClient.PostAsJsonAsync("api/usuarios", new { NombreUsuario = nombreUsuario });

        var respuesta = await _httpClient.PostAsJsonAsync(
            $"api/usuarios/{nombreUsuario}/asignar-item",
            new { EsAltaRelevancia = esAltaRelevancia });

        respuesta.EnsureSuccessStatusCode();
    }

    public async Task RegistrarCompletadoAsync(string nombreUsuario, bool esAltaRelevancia)
    {
        var respuesta = await _httpClient.PostAsJsonAsync(
            $"api/usuarios/{nombreUsuario}/completar-item",
            new { EsAltaRelevancia = esAltaRelevancia });

        respuesta.EnsureSuccessStatusCode();
    }
}
