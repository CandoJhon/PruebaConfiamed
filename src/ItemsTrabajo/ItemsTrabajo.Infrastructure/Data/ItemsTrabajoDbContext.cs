using ItemsTrabajo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ItemsTrabajo.Infrastructure.Data;

public class ItemsTrabajoDbContext : DbContext
{
    public ItemsTrabajoDbContext(DbContextOptions<ItemsTrabajoDbContext> options) : base(options)
    {
    }

    public DbSet<ItemTrabajo> ItemsTrabajo => Set<ItemTrabajo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ItemTrabajo>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.Titulo).IsRequired();
        });
    }
}
