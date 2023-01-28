using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.App.Handlers.Sessoes;

public static class SessaoUsuarioViewMapper
{
    public static SessaoUsuarioResponse Map(Usuario usuario)
    {
        var result = new SessaoUsuarioResponse(
            Id: usuario.Id,
            Nome: usuario.Nome,
            Email: usuario.Email);
        return result;
    }
}
