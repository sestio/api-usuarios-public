namespace Sestio.Usuarios.Domain.Sessoes.Values;

public sealed record AccessToken(string Value, TimeSpan TimeToLive);
