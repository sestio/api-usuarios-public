using Sestio.Usuarios.App.Services.Sessoes.Responses;

namespace Sestio.Usuarios.Api.Rest.Sessoes;

public static class SessaoViewMapper
{
    public static SessaoView Map(SessaoResponse response)
    {
        var view = new SessaoView
        {
            AccessToken = MapAccessToken(response.AccessToken!),
            Usuario = response.Usuario!
        };
        return view;
    }

    private static AccessTokenView MapAccessToken(AccessTokenResponse accessTokenResponse)
    {
        return new AccessTokenView
        {
            Token = accessTokenResponse.Token,
            Ttl = (long)accessTokenResponse.TimeToLive.TotalSeconds
        };
    }
}
