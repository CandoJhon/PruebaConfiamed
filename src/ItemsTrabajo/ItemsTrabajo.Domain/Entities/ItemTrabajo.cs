using ItemsTrabajo.Domain.Enums;

namespace ItemsTrabajo.Domain.Entities;

public class ItemTrabajo
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public DateTime FechaEntrega { get; set; }

    public Relevancia Relevancia { get; set; }

    public EstadoItem Estado { get; set; } = EstadoItem.Pendiente;

    /// <summary>Nombre de usuario al que fue asignado el ítem.</summary>
    public string? AsignadoA { get; set; }

    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Un ítem es urgente si su fecha de entrega está a menos de 3 días
    /// desde el momento de la evaluación.
    /// </summary>
    public bool EsUrgente(DateTime momentoEvaluacion)
        => (FechaEntrega.Date - momentoEvaluacion.Date).TotalDays < 3;
}
