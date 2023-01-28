using Sestio.Usuarios.App.Services.Usuarios.Requests;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.App.Handlers.Usuarios.Mappers;

public static class CriarUsuarioDtoMapper
{
    public static CriarUsuarioDto Map(CriarUsuarioRequest request)
    {
        var result = new CriarUsuarioDto(
            request.IdTenant,
            request.Nome,
            request.Email,
            request.Senha);
        return result;
    }
}
