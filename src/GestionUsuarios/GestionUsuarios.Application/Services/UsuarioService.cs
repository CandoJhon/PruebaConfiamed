using GestionUsuarios.Application.DTOs;
using GestionUsuarios.Domain.Entities;
using GestionUsuarios.Infrastructure.Repositories;

namespace GestionUsuarios.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _repository;

    public UsuarioService(IUsuarioRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<UsuarioDto>> ObtenerTodosAsync()
    {
        var usuarios = await _repository.ObtenerTodosAsync();
        return usuarios.Select(MapearADto).ToList();
    }

    public async Task<UsuarioDto?> ObtenerPorNombreAsync(string nombreUsuario)
    {
        var usuario = await _repository.ObtenerPorNombreAsync(nombreUsuario);
        return usuario is null ? null : MapearADto(usuario);
    }

    public async Task<UsuarioDto> CrearOEncontrarAsync(string nombreUsuario)
    {
        var existente = await _repository.ObtenerPorNombreAsync(nombreUsuario);
        if (existente is not null)
        {
            return MapearADto(existente);
        }

        var nuevo = new Usuario { NombreUsuario = nombreUsuario };
        var creado = await _repository.CrearAsync(nuevo);
        return MapearADto(creado);
    }

    public async Task<UsuarioDto> RegistrarAsignacionAsync(string nombreUsuario, bool esAltaRelevancia)
    {
        var usuario = await _repository.ObtenerPorNombreAsync(nombreUsuario)
            ?? await _repository.CrearAsync(new Usuario { NombreUsuario = nombreUsuario });

        usuario.ItemsPendientes += 1;
        if (esAltaRelevancia)
        {
            usuario.ItemsAltaRelevanciaPendientes += 1;
        }

        await _repository.ActualizarAsync(usuario);
        return MapearADto(usuario);
    }

    public async Task<UsuarioDto> RegistrarCompletadoAsync(string nombreUsuario, bool esAltaRelevancia)
    {
        var usuario = await _repository.ObtenerPorNombreAsync(nombreUsuario);
        if (usuario is null)
        {
            throw new InvalidOperationException($"El usuario '{nombreUsuario}' no existe.");
        }

        if (usuario.ItemsPendientes > 0)
        {
            usuario.ItemsPendientes -= 1;
        }

        if (esAltaRelevancia && usuario.ItemsAltaRelevanciaPendientes > 0)
        {
            usuario.ItemsAltaRelevanciaPendientes -= 1;
        }

        usuario.ItemsCompletados += 1;

        await _repository.ActualizarAsync(usuario);
        return MapearADto(usuario);
    }

    private static UsuarioDto MapearADto(Usuario u) => new(
        u.Id,
        u.NombreUsuario,
        u.ItemsPendientes,
        u.ItemsCompletados,
        u.ItemsAltaRelevanciaPendientes,
        u.EstaSaturado
    );
}
