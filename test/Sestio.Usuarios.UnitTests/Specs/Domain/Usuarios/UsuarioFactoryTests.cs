using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.Domain.Hashing;

namespace Sestio.Usuarios.UnitTests.Specs.Domain.Usuarios;

public class UsuarioFactoryTests
{
    [Fact]
    public void Cria_usuario_com_senha_criptografada_valida()
    {
        var passwordHasher = CriarPasswordHasher();
        var sut = new UsuarioFactory(passwordHasher);

        var dto = new CriarUsuarioDto(
            IdTenant: Guid.NewGuid(),
            Nome: "user",
            Email: "user@example.com",
            Senha: "password");
        var usuario = sut.Criar(dto);

        usuario.Senha.Value.Should().NotBeNullOrEmpty();
        passwordHasher.IsValid(usuario, "password").Should().BeTrue();
    }

    private static IUserPasswordHasher<Usuario> CriarPasswordHasher()
    {
        var baseHasher = new PasswordHasher<Usuario>();
        return new UserPasswordHasher<Usuario>(baseHasher);
    }
}
