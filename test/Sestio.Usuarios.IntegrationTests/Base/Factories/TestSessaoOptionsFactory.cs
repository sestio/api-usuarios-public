using Sestio.Usuarios.Domain.Sessoes.Services;

namespace Sestio.Usuarios.IntegrationTests.Base.Factories;

public static class TestSessaoOptionsFactory
{
    public static SessaoOptions Criar(
        TimeSpan duracaoSessao,
        TimeSpan duracaoAccessToken)
    {
        return new SessaoOptions(duracaoSessao, duracaoAccessToken);
    }
}
