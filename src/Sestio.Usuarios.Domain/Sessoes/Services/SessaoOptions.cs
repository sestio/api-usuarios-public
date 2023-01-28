namespace Sestio.Usuarios.Domain.Sessoes.Services;

public sealed record SessaoOptions(
    TimeSpan DuracaoSessao,
    TimeSpan DuracaoAccessToken);
