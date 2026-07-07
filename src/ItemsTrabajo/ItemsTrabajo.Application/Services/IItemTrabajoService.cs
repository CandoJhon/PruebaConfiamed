using ItemsTrabajo.Application.DTOs;

namespace ItemsTrabajo.Application.Services;

public interface IItemTrabajoService
{
    Task<List<ItemTrabajoDto>> ObtenerTodosAsync();
    Task<List<ItemTrabajoDto>> ObtenerPorUsuarioAsync(string nombreUsuario);
    Task<ItemTrabajoDto?> ObtenerPorIdAsync(int id);

    /// <summary>Crea un ítem y lo asigna inmediatamente aplicando el algoritmo de distribución.</summary>
    Task<ItemTrabajoDto> CrearYAsignarAsync(CrearItemTrabajoDto dto);

    /// <summary>
    /// Crea y asigna varios ítems en un mismo lote, respetando el orden de
    /// prioridad: urgentes primero, luego alta relevancia, luego baja relevancia.
    /// Recalcula el estado de los usuarios después de cada asignación individual.
    /// </summary>
    Task<List<ItemTrabajoDto>> CrearYAsignarLoteAsync(List<CrearItemTrabajoDto> items);

    Task<ItemTrabajoDto> MarcarComoCompletadoAsync(int id);
}
