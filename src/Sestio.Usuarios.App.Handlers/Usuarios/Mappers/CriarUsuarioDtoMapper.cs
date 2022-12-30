using Sestio.Usuarios.App.Services.Usuarios.Commands;
using Sestio.Usuarios.Domain.Usuarios;

namespace Sestio.Usuarios.App.Handlers.Usuarios.Mappers;

public static class CriarUsuarioDtoMapper
{
    public static CriarUsuarioDto Map(CriarUsuarioCommand command)
    {
        var result = new CriarUsuarioDto(
            command.IdTenant,
            command.Nome,
            command.Email,
            command.Senha);
        return result;
    }
}
