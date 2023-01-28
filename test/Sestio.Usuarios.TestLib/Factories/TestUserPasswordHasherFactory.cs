using Microsoft.AspNetCore.Identity;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.Domain.Hashing;

namespace Sestio.Usuarios.TestLib.Factories;

public static class TestUserPasswordHasherFactory
{
    public static IUserPasswordHasher<Usuario> Criar()
    {
        var passwordHasher = new UserPasswordHasher<Usuario>(new PasswordHasher<Usuario>());
        return passwordHasher;
    }
}
