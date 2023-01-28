using Microsoft.AspNetCore.Identity;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.Domain.Hashing;

namespace Sestio.Usuarios.IntegrationTests.Base.Factories;

public static class TestHashedRefreshTokenGeneratorFactory
{
    public static HashedRefreshTokenGenerator Criar()
    {
        return new HashedRefreshTokenGenerator(new PasswordHasher<Usuario>());
    }
}
