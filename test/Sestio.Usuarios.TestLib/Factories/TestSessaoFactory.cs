using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.TestLib.Factories;

public static class TestSessaoFactory
{
    public static Sessao CriarAtiva(Usuario usuario)
    {
        return CriarAtiva(usuario, TimeSpan.FromDays(1));
    }

    public static Sessao CriarAtiva(Usuario usuario, TimeSpan duracao)
    {
        var sessao = new Sessao(usuario, duracao);
        return sessao;
    }

    public static Sessao CriarAtiva()
    {
        var (usuario, _) = TestUsuarioFactory.Criar();
        return CriarAtiva(usuario);
    }

    public static Sessao CriarExpirada()
    {
        var sessao = CriarAtiva();
        SetSituacao(sessao, SituacaoSessao.Expirada);

        return sessao;
    }

    public static Sessao CriarExpirada(Usuario usuario)
    {
        var sessao = CriarAtiva(usuario);
        SetSituacao(sessao, SituacaoSessao.Expirada);

        return sessao;
    }

    public static Sessao CriarRevogada()
    {
        var sessao = CriarAtiva();
        SetSituacao(sessao, SituacaoSessao.Revogada);

        return sessao;
    }

    public static Sessao CriarRevogada(Usuario usuario)
    {
        var sessao = CriarAtiva(usuario);
        SetSituacao(sessao, SituacaoSessao.Revogada);

        return sessao;
    }

    private static void SetSituacao(Sessao sessao, SituacaoSessao situacao)
    {
        var prop = typeof(Sessao).GetProperty(nameof(Sessao.Situacao))!;
        prop.SetValue(sessao, situacao);
    }

}
