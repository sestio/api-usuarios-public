using Sestio.Usuarios.App.Services.Sessoes.Responses;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Sessoes.Values;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.App.Handlers.Sessoes;

public static class SessaoResponseMapper
{
    public static SessaoResponse Invalido(MotivoFalhaSessaoResponse motivo)
    {
        return new SessaoResponse(false, motivo, null, null, null);
    }

    public static SessaoResponse Valido(Usuario usuario, AccessToken accessToken, RefreshToken refreshToken)
    {
        var response = new SessaoResponse(
            Sucesso: true,
            MotivoFalhaSessaoResponse.Nenhum,
            SessaoUsuarioViewMapper.Map(usuario),
            new AccessTokenResponse(accessToken.Value, accessToken.TimeToLive),
            new RefreshTokenResponse(refreshToken.Token));
        return response;
    }
}
