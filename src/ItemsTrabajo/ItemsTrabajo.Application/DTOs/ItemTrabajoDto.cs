using ItemsTrabajo.Domain.Enums;

namespace ItemsTrabajo.Application.DTOs;

public record ItemTrabajoDto(
    int Id,
    string Titulo,
    DateTime FechaEntrega,
    Relevancia Relevancia,
    EstadoItem Estado,
    string? AsignadoA,
    DateTime FechaCreacion
);

public record CrearItemTrabajoDto(
    string Titulo,
    DateTime FechaEntrega,
    Relevancia Relevancia
);

/// <summary>Snapshot del estado de un usuario, tal como lo reporta GestionUsuarios.Api.</summary>
public record UsuarioResumenDto(
    string NombreUsuario,
    int ItemsPendientes,
    int ItemsAltaRelevanciaPendientes,
    bool EstaSaturado
);
