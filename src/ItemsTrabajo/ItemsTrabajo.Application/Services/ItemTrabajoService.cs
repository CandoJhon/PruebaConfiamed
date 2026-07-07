using ItemsTrabajo.Application.Clients;
using ItemsTrabajo.Application.DTOs;
using ItemsTrabajo.Domain.Entities;
using ItemsTrabajo.Domain.Enums;
using ItemsTrabajo.Infrastructure.Repositories;

namespace ItemsTrabajo.Application.Services;

public class ItemTrabajoService : IItemTrabajoService
{
    private readonly IItemTrabajoRepository _repository;
    private readonly IDistribucionService _distribucionService;
    private readonly IGestionUsuariosClient _usuariosClient;

    public ItemTrabajoService(
        IItemTrabajoRepository repository,
        IDistribucionService distribucionService,
        IGestionUsuariosClient usuariosClient)
    {
        _repository = repository;
        _distribucionService = distribucionService;
        _usuariosClient = usuariosClient;
    }

    public async Task<List<ItemTrabajoDto>> ObtenerTodosAsync()
    {
        var items = await _repository.ObtenerTodosAsync();
        return items.Select(MapearADto).ToList();
    }

    public async Task<List<ItemTrabajoDto>> ObtenerPorUsuarioAsync(string nombreUsuario)
    {
        var items = await _repository.ObtenerPorUsuarioAsync(nombreUsuario);
        return items.Select(MapearADto).ToList();
    }

    public async Task<ItemTrabajoDto?> ObtenerPorIdAsync(int id)
    {
        var item = await _repository.ObtenerPorIdAsync(id);
        return item is null ? null : MapearADto(item);
    }

    public async Task<ItemTrabajoDto> CrearYAsignarAsync(CrearItemTrabajoDto dto)
    {
        var item = new ItemTrabajo
        {
            Titulo = dto.Titulo,
            FechaEntrega = dto.FechaEntrega,
            Relevancia = dto.Relevancia
        };

        var ahora = DateTime.UtcNow;
        var usuarioAsignado = await _distribucionService.DeterminarUsuarioAsignadoAsync(item, ahora);

        item.AsignadoA = usuarioAsignado;
        var creado = await _repository.CrearAsync(item);

        await _usuariosClient.RegistrarAsignacionAsync(usuarioAsignado, dto.Relevancia == Relevancia.Alta);

        return MapearADto(creado);
    }

    public async Task<List<ItemTrabajoDto>> CrearYAsignarLoteAsync(List<CrearItemTrabajoDto> items)
    {
        var ahora = DateTime.UtcNow;

        // Orden de prioridad para la asignación en lote:
        // 1) urgentes (fecha de entrega < 3 días) primero,
        // 2) entre los no urgentes, los de alta relevancia antes que los de baja,
        // 3) como último criterio, fecha de entrega más próxima primero.
        var itemsOrdenados = items
            .Select(dto => new ItemTrabajo
            {
                Titulo = dto.Titulo,
                FechaEntrega = dto.FechaEntrega,
                Relevancia = dto.Relevancia
            })
            .OrderByDescending(i => i.EsUrgente(ahora))
            .ThenByDescending(i => i.Relevancia == Relevancia.Alta)
            .ThenBy(i => i.FechaEntrega)
            .ToList();

        var resultado = new List<ItemTrabajoDto>();

        // Se procesa secuencialmente (no en paralelo) para que cada asignación
        // recalcule el estado de los usuarios ANTES de decidir la siguiente,
        // cumpliendo con "ordenar la lista de pendientes por usuario después
        // de cada asignación".
        foreach (var item in itemsOrdenados)
        {
            var usuarioAsignado = await _distribucionService.DeterminarUsuarioAsignadoAsync(item, ahora);
            item.AsignadoA = usuarioAsignado;

            var creado = await _repository.CrearAsync(item);
            await _usuariosClient.RegistrarAsignacionAsync(usuarioAsignado, item.Relevancia == Relevancia.Alta);

            resultado.Add(MapearADto(creado));
        }

        return resultado;
    }

    public async Task<ItemTrabajoDto> MarcarComoCompletadoAsync(int id)
    {
        var item = await _repository.ObtenerPorIdAsync(id);
        if (item is null)
        {
            throw new InvalidOperationException($"No existe un ítem de trabajo con id {id}.");
        }

        if (item.Estado == EstadoItem.Completado)
        {
            return MapearADto(item);
        }

        item.Estado = EstadoItem.Completado;
        await _repository.ActualizarAsync(item);

        if (!string.IsNullOrEmpty(item.AsignadoA))
        {
            await _usuariosClient.RegistrarCompletadoAsync(item.AsignadoA, item.Relevancia == Relevancia.Alta);
        }

        return MapearADto(item);
    }

    private static ItemTrabajoDto MapearADto(ItemTrabajo i) => new(
        i.Id,
        i.Titulo,
        i.FechaEntrega,
        i.Relevancia,
        i.Estado,
        i.AsignadoA,
        i.FechaCreacion
    );
}
