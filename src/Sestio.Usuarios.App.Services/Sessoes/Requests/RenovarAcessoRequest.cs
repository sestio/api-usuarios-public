namespace Sestio.Usuarios.App.Services.Sessoes.Requests;

public sealed class RenovarAcessoRequest
{
    public required string RefreshToken { get; init; }
}
