namespace Sestio.Usuarios.App.Services.Sessoes.Responses;

public sealed record AccessTokenResponse(string Token, TimeSpan TimeToLive);
