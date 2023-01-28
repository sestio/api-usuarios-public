namespace Sestio.Usuarios.App.Services.Sessoes.Requests;

public sealed class IniciarSessaoRequest
{
    public required string Email { get; init; }
    public required string Senha { get; init; }
}
