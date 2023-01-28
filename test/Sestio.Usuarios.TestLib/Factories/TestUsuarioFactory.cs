using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.TestLib.Factories;

public static class TestUsuarioFactory
{
    public static (Usuario, CriarUsuarioDto) Criar(IUserPasswordHasher<Usuario> passwordHasher)
    {
        var usuarioFactory = new UsuarioFactory(passwordHasher);

        var dto = new CriarUsuarioDto(
            IdTenant: Guid.NewGuid(),
            Nome: "Usuario Teste",
            Email: "usuario@example.com",
            Senha: "password");

        var usuario = usuarioFactory.Criar(dto);
        return (usuario, dto);
    }

    public static (Usuario, CriarUsuarioDto) Criar()
    {
        var passwordHasher = TestUserPasswordHasherFactory.Criar();
        var usuarioFactory = new UsuarioFactory(passwordHasher);

        var dto = new CriarUsuarioDto(
            IdTenant: Guid.NewGuid(),
            Nome: "Usuario Teste",
            Email: "usuario@example.com",
            Senha: Guid.NewGuid().ToString());

        var usuario = usuarioFactory.Criar(dto);
        return (usuario, dto);
    }
}
