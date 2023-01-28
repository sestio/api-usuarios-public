using FluentAssertions;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.TestLib.Factories;

namespace Sestio.Usuarios.UnitTests.Specs.Domain.Sessoes;

public class RefreshTokenTests
{
    [Theory]
    [InlineData(SituacaoRefreshToken.Valido, true)]
    [InlineData(SituacaoRefreshToken.Invalido, false)]
    public void Valida_situacao_para_uso_em_renovacao_de_acesso(SituacaoRefreshToken situacaoToken, bool resultadoEsperado)
    {
        var (usuario, _) = TestUsuarioFactory.Criar();
        var sessao = TestSessaoFactory.CriarAtiva(usuario);
        var token = TestRefreshTokenFactory.CriarNovo(sessao);
        token.GetType().GetProperty(nameof(RefreshToken.Situacao))!.SetValue(token, situacaoToken);

        var result = token.IsValidoParaRenovacao();

        result.Should().Be(resultadoEsperado);
    }

    [Fact]
    public void Lanca_erro_ao_tentar_validar_um_token_novo_para_uso_em_renovacao_de_acesso()
    {
        var (usuario, _) = TestUsuarioFactory.Criar();
        var sessao = TestSessaoFactory.CriarAtiva(usuario);
        var token = TestRefreshTokenFactory.CriarNovo(sessao);

        var act = () => token.IsValidoParaRenovacao();

        act.Should().Throw<Exception>();
    }
}
