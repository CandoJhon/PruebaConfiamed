using ItemsTrabajo.Domain.Entities;
using ItemsTrabajo.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ItemsTrabajo.Infrastructure.Repositories;

public class ItemTrabajoRepository : IItemTrabajoRepository
{
    private readonly ItemsTrabajoDbContext _context;

    public ItemTrabajoRepository(ItemsTrabajoDbContext context)
    {
        _context = context;
    }

    public async Task<List<ItemTrabajo>> ObtenerTodosAsync()
        => await _context.ItemsTrabajo.AsNoTracking().ToListAsync();

    public async Task<List<ItemTrabajo>> ObtenerPorUsuarioAsync(string nombreUsuario)
        => await _context.ItemsTrabajo
            .Where(i => i.AsignadoA == nombreUsuario)
            .OrderBy(i => i.FechaEntrega)
            .ToListAsync();

    public async Task<ItemTrabajo?> ObtenerPorIdAsync(int id)
        => await _context.ItemsTrabajo.FirstOrDefaultAsync(i => i.Id == id);

    public async Task<ItemTrabajo> CrearAsync(ItemTrabajo item)
    {
        _context.ItemsTrabajo.Add(item);
        await _context.SaveChangesAsync();
        return item;
    }

    public async Task ActualizarAsync(ItemTrabajo item)
    {
        _context.ItemsTrabajo.Update(item);
        await _context.SaveChangesAsync();
    }
}
