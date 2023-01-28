using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.TestLib.Factories;

public static class TestRefreshTokenFactory
{

    public static RefreshToken CriarNovo(Sessao sessao)
    {
        return new RefreshToken(sessao, Guid.NewGuid().ToString());
    }

    public static RefreshToken CriarAtivo(Sessao sessao)
    {
        var refreshToken = CriarNovo(sessao);
        refreshToken.Ativar();
        return refreshToken;
    }

    public static RefreshToken CriarInvalido(Sessao sessao)
    {
        var refreshToken = CriarNovo(sessao);
        refreshToken.Invalidar();
        return refreshToken;
    }
}
