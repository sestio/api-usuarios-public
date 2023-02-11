using Sestio.Usuarios.App.Services.Usuarios.Responses;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.App.Handlers.Usuarios.Mappers;

public static class UsuarioResponseMapper
{
    public static UsuarioResponse Map(Usuario user)
    {
        return new UsuarioResponse
        {
            Id = user.Id,
            IdTenant = user.IdTenant,
            Nome = user.Nome,
            Email = user.Email
        };
    }
}
