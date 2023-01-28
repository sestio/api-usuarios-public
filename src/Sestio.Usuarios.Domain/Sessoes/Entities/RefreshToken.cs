using Sestio.Commons.SourceGeneration.Attributes;
using Sestio.Usuarios.Domain.Base;

namespace Sestio.Usuarios.Domain.Sessoes.Entities;

public enum SituacaoRefreshToken
{
    Novo = 1,
    Valido = 2,
    Substituido = 3,
    Invalido = 4
}

[HasGeneratedParameterlessConstructor]
public sealed partial class RefreshToken : Entity
{
    public RefreshToken(Sessao sessao, string token)
    {
        Situacao = SituacaoRefreshToken.Novo;
        IdSessao = sessao.Id;
        Token = token;
    }

    public SituacaoRefreshToken Situacao { get; private set; }
    public Guid IdSessao { get; }
    public string Token { get; }

    public void Ativar() => Situacao = SituacaoRefreshToken.Valido;
    public void Substituir() => Situacao = SituacaoRefreshToken.Substituido;
    public void Invalidar() => Situacao = SituacaoRefreshToken.Invalido;

    public bool IsValidoParaRenovacao()
    {
        if (Situacao == SituacaoRefreshToken.Novo)
            throw new Exception("Token novo não deve ser utilizado em tentativa de renovação de acesso.");

        return Situacao == SituacaoRefreshToken.Valido;
    }
}
