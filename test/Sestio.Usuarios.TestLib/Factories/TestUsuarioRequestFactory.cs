using Sestio.Usuarios.App.Services.Usuarios.Requests;

namespace Sestio.Usuarios.TestLib.Factories;

public static class TestUsuarioRequestFactory
{
    public static CriarUsuarioRequest CriarUsuario(
        string? nome = null,
        string? senha = null,
        string? email = null)
    {
        return new CriarUsuarioRequest
        {
            IdTenant = Guid.NewGuid(),
            Nome = nome ?? $"Usuário Teste {Guid.NewGuid():N}",
            Email = email ?? $"email.{Guid.NewGuid():N}@example.com",
            Senha = senha ?? Guid.NewGuid().ToString("N")
        };
    }
}
