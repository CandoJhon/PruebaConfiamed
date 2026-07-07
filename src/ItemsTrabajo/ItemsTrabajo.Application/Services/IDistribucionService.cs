using ItemsTrabajo.Domain.Entities;

namespace ItemsTrabajo.Application.Services;

public interface IDistribucionService
{
    /// <summary>
    /// Determina a qué usuario debe asignarse un ítem de trabajo según las
    /// reglas de negocio. Lanza InvalidOperationException si no hay ningún
    /// usuario disponible (todos saturados).
    /// </summary>
    Task<string> DeterminarUsuarioAsignadoAsync(ItemTrabajo item, DateTime momentoEvaluacion);
}
