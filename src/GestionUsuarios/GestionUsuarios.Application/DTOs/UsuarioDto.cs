namespace GestionUsuarios.Application.DTOs;

public record UsuarioDto(
    int Id,
    string NombreUsuario,
    int ItemsPendientes,
    int ItemsCompletados,
    int ItemsAltaRelevanciaPendientes,
    bool EstaSaturado
);

public record CrearUsuarioDto(string NombreUsuario);

public record RegistrarAsignacionDto(bool EsAltaRelevancia);

public record RegistrarCompletadoDto(bool EsAltaRelevancia);
