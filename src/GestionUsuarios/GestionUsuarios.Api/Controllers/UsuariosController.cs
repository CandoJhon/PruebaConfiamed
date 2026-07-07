using GestionUsuarios.Application.DTOs;
using GestionUsuarios.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestionUsuarios.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioService _usuarioService;

    public UsuariosController(IUsuarioService usuarioService)
    {
        _usuarioService = usuarioService;
    }

    /// <summary>Lista todos los usuarios registrados junto a sus contadores actuales.</summary>
    [HttpGet]
    public async Task<ActionResult<List<UsuarioDto>>> ObtenerTodos()
    {
        var usuarios = await _usuarioService.ObtenerTodosAsync();
        return Ok(usuarios);
    }

    /// <summary>Obtiene el detalle de un usuario por su nombre de usuario.</summary>
    [HttpGet("{nombreUsuario}")]
    public async Task<ActionResult<UsuarioDto>> ObtenerPorNombre(string nombreUsuario)
    {
        var usuario = await _usuarioService.ObtenerPorNombreAsync(nombreUsuario);
        return usuario is null ? NotFound() : Ok(usuario);
    }

    /// <summary>
    /// Crea el registro de seguimiento de un usuario si no existe aún
    /// (los usuarios como tales ya existen en otra parte del sistema;
    /// aquí solo se registra la referencia por nombre de usuario).
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<UsuarioDto>> CrearOEncontrar([FromBody] CrearUsuarioDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.NombreUsuario))
        {
            return BadRequest("El nombre de usuario es requerido.");
        }

        var usuario = await _usuarioService.CrearOEncontrarAsync(dto.NombreUsuario);
        return Ok(usuario);
    }

    /// <summary>
    /// Invocado por el microservicio de Ítems de Trabajo cuando le asigna
    /// un nuevo ítem a este usuario, para mantener sus contadores al día.
    /// </summary>
    [HttpPost("{nombreUsuario}/asignar-item")]
    public async Task<ActionResult<UsuarioDto>> RegistrarAsignacion(
        string nombreUsuario, [FromBody] RegistrarAsignacionDto dto)
    {
        var usuario = await _usuarioService.RegistrarAsignacionAsync(nombreUsuario, dto.EsAltaRelevancia);
        return Ok(usuario);
    }

    /// <summary>
    /// Invocado por el microservicio de Ítems de Trabajo cuando un ítem
    /// asignado a este usuario se marca como completado.
    /// </summary>
    [HttpPost("{nombreUsuario}/completar-item")]
    public async Task<ActionResult<UsuarioDto>> RegistrarCompletado(
        string nombreUsuario, [FromBody] RegistrarCompletadoDto dto)
    {
        try
        {
            var usuario = await _usuarioService.RegistrarCompletadoAsync(nombreUsuario, dto.EsAltaRelevancia);
            return Ok(usuario);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
