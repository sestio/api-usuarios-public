namespace Sestio.Usuarios.App.Services.Usuarios.Requests;

public sealed class CriarUsuarioRequest
{
    public Guid IdTenant { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Senha { get; init; } = string.Empty;
}
