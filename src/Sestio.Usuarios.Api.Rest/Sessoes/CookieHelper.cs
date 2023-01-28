using Microsoft.AspNetCore.Http;
using Sestio.Usuarios.App.Services.Sessoes.Responses;

namespace Sestio.Usuarios.Api.Rest.Sessoes;

public static class CookieHelper
{
    private const string RefreshTokenCookieName = "SID";

    public static void WriteRefreshTokenCookie(IResponseCookies cookies, RefreshTokenResponse refreshToken)
    {
        // O token deve durar mais tempo que a sessão para garantir que:
        // a) A sessão continue ativa mesmo se o navegador for fechado
        // b) A rotina de expiração de sessão sempre seja executada. Sessões no banco devem ter a situação atualizada
        //    quando passam da validade.
        var expires = DateTime.MaxValue.ToUniversalTime();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = expires,
            MaxAge = expires - DateTime.UtcNow
        };
        cookies.Append(RefreshTokenCookieName, refreshToken.Token, cookieOptions);
    }

    public static string ReadRefreshTokenCookie(IRequestCookieCollection cookies)
    {
        if (cookies.TryGetValue(RefreshTokenCookieName, out var token))
            return token;
        return string.Empty;
    }

    public static void RemoveRefreshTokenCookie(IResponseCookies cookies)
    {
        cookies.Delete(RefreshTokenCookieName);
    }
}
