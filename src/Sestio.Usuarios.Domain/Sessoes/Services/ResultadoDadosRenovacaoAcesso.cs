using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.Domain.Sessoes.Services;

public sealed record ResultadoDadosRenovacaoAcesso(bool Sucesso, Sessao? Sessao)
{
    public static ResultadoDadosRenovacaoAcesso Criar(Sessao? sessao)
    {
        return sessao?.Situacao switch
        {
            SituacaoSessao.Valida => new ResultadoDadosRenovacaoAcesso(true, sessao),
            SituacaoSessao.Revogada => new ResultadoDadosRenovacaoAcesso(false, sessao),
            SituacaoSessao.Expirada => new ResultadoDadosRenovacaoAcesso(false, sessao),
            null => new ResultadoDadosRenovacaoAcesso(false, null),
            _ => throw new Exception($"Situação de sessao '{sessao.Situacao}' não tratado.")
        };
    }
}
