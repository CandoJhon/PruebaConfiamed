using ItemsTrabajo.Application.DTOs;
using ItemsTrabajo.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ItemsTrabajo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemsTrabajoController : ControllerBase
{
    private readonly IItemTrabajoService _itemService;

    public ItemsTrabajoController(IItemTrabajoService itemService)
    {
        _itemService = itemService;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemTrabajoDto>>> ObtenerTodos()
        => Ok(await _itemService.ObtenerTodosAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemTrabajoDto>> ObtenerPorId(int id)
    {
        var item = await _itemService.ObtenerPorIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("usuario/{nombreUsuario}")]
    public async Task<ActionResult<List<ItemTrabajoDto>>> ObtenerPorUsuario(string nombreUsuario)
        => Ok(await _itemService.ObtenerPorUsuarioAsync(nombreUsuario));

    /// <summary>Crea un ítem de trabajo y lo asigna de inmediato aplicando el algoritmo de distribución.</summary>
    [HttpPost]
    public async Task<ActionResult<ItemTrabajoDto>> Crear([FromBody] CrearItemTrabajoDto dto)
    {
        try
        {
            var item = await _itemService.CrearYAsignarAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = item.Id }, item);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    /// <summary>
    /// Crea y asigna varios ítems en un solo lote, respetando el orden de
    /// prioridad urgencia -> relevancia -> fecha de entrega.
    /// </summary>
    [HttpPost("lote")]
    public async Task<ActionResult<List<ItemTrabajoDto>>> CrearLote([FromBody] List<CrearItemTrabajoDto> items)
    {
        try
        {
            var resultado = await _itemService.CrearYAsignarLoteAsync(items);
            return Ok(resultado);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
    }

    [HttpPost("{id:int}/completar")]
    public async Task<ActionResult<ItemTrabajoDto>> Completar(int id)
    {
        try
        {
            var item = await _itemService.MarcarComoCompletadoAsync(id);
            return Ok(item);
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
