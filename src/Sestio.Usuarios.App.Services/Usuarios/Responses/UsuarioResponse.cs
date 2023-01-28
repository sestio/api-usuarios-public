namespace Sestio.Usuarios.App.Services.Usuarios.Responses;

public class UsuarioResponse
{
    public Guid Id { get; init; }
    public Guid IdTenant { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
