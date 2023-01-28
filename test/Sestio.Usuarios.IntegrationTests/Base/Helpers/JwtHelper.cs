using Microsoft.IdentityModel.JsonWebTokens;

namespace Sestio.Usuarios.IntegrationTests.Base.Helpers;

public static class JwtHelper
{
    public static JsonWebToken ReadToken(string Token)
    {
        return new JsonWebTokenHandler().ReadJsonWebToken(Token);
    }

    public static T? ReadClaim<T>(JsonWebToken tokenInfo, string key)
    {
        return tokenInfo.TryGetPayloadValue(key, out T result) ? result : default(T);
    }
}
