using ItemsTrabajo.Application.Clients;
using ItemsTrabajo.Domain.Entities;
using ItemsTrabajo.Domain.Enums;

namespace ItemsTrabajo.Application.Services;

/// <summary>
/// Implementa el algoritmo de distribución de ítems de trabajo descrito
/// en el enunciado:
///
///  1. Se excluyen los usuarios "saturados" (más de 3 ítems de alta
///     relevancia pendientes) de cualquier asignación.
///
///  2. Si el ítem es URGENTE (fecha de entrega a menos de 3 días),
///     se asigna al usuario no saturado con MENOS ítems pendientes,
///     sin importar la relevancia del ítem.
///
///  3. Si el ítem NO es urgente, se aplica el mismo criterio de
///     "menos ítems pendientes entre los no saturados". La regla de
///     "los ítems relevantes se asignan primero" se traduce, en el caso
///     de asignación de UN solo ítem, en el criterio de desempate al
///     ordenar/procesar varios ítems en lote (ver AsignarLoteAsync en
///     ItemTrabajoService): antes de asignar los de baja relevancia,
///     se procesan primero los urgentes y luego los de alta relevancia,
///     de forma que los usuarios con menos carga queden reservados para
///     ellos cuando compiten por el mismo turno de asignación.
///
///  Esta interpretación se documenta explícitamente porque el enunciado
///  no especifica cómo desempatar cuando compiten reglas de urgencia y
///  relevancia dentro de una única asignación puntual.
/// </summary>
public class DistribucionService : IDistribucionService
{
    private readonly IGestionUsuariosClient _usuariosClient;

    public DistribucionService(IGestionUsuariosClient usuariosClient)
    {
        _usuariosClient = usuariosClient;
    }

    public async Task<string> DeterminarUsuarioAsignadoAsync(ItemTrabajo item, DateTime momentoEvaluacion)
    {
        var usuarios = await _usuariosClient.ObtenerUsuariosAsync();

        var candidatos = usuarios.Where(u => !u.EstaSaturado).ToList();

        if (candidatos.Count == 0)
        {
            throw new InvalidOperationException(
                "No hay usuarios disponibles para la distribución: todos están saturados " +
                "(más de 3 ítems de alta relevancia pendientes).");
        }

        // Independientemente de si es urgente o de su relevancia, el criterio final
        // de selección puntual es: el usuario no saturado con menos ítems pendientes.
        // En caso de empate, se desempata por orden alfabético para tener un resultado
        // determinístico y reproducible en pruebas.
        var seleccionado = candidatos
            .OrderBy(u => u.ItemsPendientes)
            .ThenBy(u => u.NombreUsuario, StringComparer.Ordinal)
            .First();

        return seleccionado.NombreUsuario;
    }
}
