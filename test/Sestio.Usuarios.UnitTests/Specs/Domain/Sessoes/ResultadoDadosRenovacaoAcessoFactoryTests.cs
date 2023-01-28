using FluentAssertions;
using Sestio.Usuarios.Domain.Sessoes.Services;
using Sestio.Usuarios.TestLib.Factories;

namespace Sestio.Usuarios.UnitTests.Specs.Domain.Sessoes;

public class ResultadoDadosRenovacaoAcessoFactoryTests
{
    [Fact]
    public void Cria_resultado_valido_para_sessao_valida()
    {
        var sessao = TestSessaoFactory.CriarAtiva();

        var result = ResultadoDadosRenovacaoAcesso.Criar(sessao);

        result.Sucesso.Should().BeTrue();
        result.Sessao.Should().Be(sessao);
    }

    [Fact]
    public void Cria_resultado_invalido_para_sessao_expirada()
    {
        var sessao = TestSessaoFactory.CriarExpirada();

        var result = ResultadoDadosRenovacaoAcesso.Criar(sessao);

        result.Sucesso.Should().BeFalse();
        result.Sessao.Should().Be(sessao);
    }

    [Fact]
    public void Cria_resultado_invalido_para_sessao_revogada()
    {
        var sessao = TestSessaoFactory.CriarRevogada();

        var result = ResultadoDadosRenovacaoAcesso.Criar(sessao);

        result.Sucesso.Should().BeFalse();
        result.Sessao.Should().Be(sessao);
    }

    [Fact]
    public void Cria_resultado_invalido_para_sessao_nao_encontrada()
    {
        var result = ResultadoDadosRenovacaoAcesso.Criar(sessao: null);

        result.Sucesso.Should().BeFalse();
        result.Sessao.Should().BeNull();
    }
}
