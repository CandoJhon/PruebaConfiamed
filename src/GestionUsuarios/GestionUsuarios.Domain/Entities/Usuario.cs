namespace GestionUsuarios.Domain.Entities;

/// <summary>
/// Representa el estado de seguimiento de un usuario dentro del sistema
/// de distribución de ítems de trabajo.
/// Nota: los usuarios "existen" en otra parte del sistema; aquí solo
/// se mantiene la referencia por nombre de usuario y sus contadores.
/// </summary>
public class Usuario
{
    public int Id { get; set; }

    public string NombreUsuario { get; set; } = string.Empty;

    /// <summary>Cantidad total de ítems de trabajo pendientes asignados.</summary>
    public int ItemsPendientes { get; set; }

    /// <summary>Cantidad total de ítems de trabajo completados.</summary>
    public int ItemsCompletados { get; set; }

    /// <summary>
    /// Cantidad de ítems PENDIENTES de alta relevancia asignados.
    /// Se usa para determinar si el usuario está "saturado".
    /// </summary>
    public int ItemsAltaRelevanciaPendientes { get; set; }

    /// <summary>
    /// Regla de negocio: un usuario con más de 3 ítems altamente relevantes
    /// pendientes se considera saturado y se excluye de la distribución.
    /// </summary>
    public bool EstaSaturado => ItemsAltaRelevanciaPendientes > 3;
}
