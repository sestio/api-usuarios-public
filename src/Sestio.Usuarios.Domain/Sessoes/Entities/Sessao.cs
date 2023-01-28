using Sestio.Commons.SourceGeneration.Attributes;
using Sestio.Usuarios.Domain.Base;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Domain.Sessoes.Entities;

public enum SituacaoSessao
{
    Valida = 1,
    Revogada = 2,
    Expirada = 3
}

[HasGeneratedParameterlessConstructor]
public sealed partial class Sessao : Entity
{
    public Sessao(Usuario usuario, TimeSpan duracao)
    {
        Situacao = SituacaoSessao.Valida;
        IdUsuario = usuario.Id;
        Data = DateTime.UtcNow;
        Validade = Data.Add(duracao);
    }

    public SituacaoSessao Situacao { get; private set; }
    public Guid IdUsuario { get; }
    public DateTime Data { get; }
    public DateTime Validade { get; }

    public void Revogar() => Situacao = SituacaoSessao.Revogada;
    public bool IsValidaParaRenovacao() => Situacao == SituacaoSessao.Valida;

    public void AtualizarSituacaoValidade()
    {
        if (Validade <= DateTime.UtcNow)
            Situacao = SituacaoSessao.Expirada;
    }
}
