using Microsoft.AspNetCore.Identity;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Infra.Domain.Hashing;

public sealed class HashedRefreshTokenGenerator : IGeradorRefreshToken
{
    private readonly IPasswordHasher<Usuario> _hasher;

    public HashedRefreshTokenGenerator(IPasswordHasher<Usuario> hasher)
    {
        _hasher = hasher;
    }

    public string Gerar(Sessao sessao)
    {
        var token = $"{sessao.Id}-{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}";
        var hash = _hasher.HashPassword(null!, token);
        return hash;
    }
}
