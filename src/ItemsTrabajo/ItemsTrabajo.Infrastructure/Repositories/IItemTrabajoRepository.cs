using ItemsTrabajo.Domain.Entities;

namespace ItemsTrabajo.Infrastructure.Repositories;

public interface IItemTrabajoRepository
{
    Task<List<ItemTrabajo>> ObtenerTodosAsync();
    Task<List<ItemTrabajo>> ObtenerPorUsuarioAsync(string nombreUsuario);
    Task<ItemTrabajo?> ObtenerPorIdAsync(int id);
    Task<ItemTrabajo> CrearAsync(ItemTrabajo item);
    Task ActualizarAsync(ItemTrabajo item);
}
