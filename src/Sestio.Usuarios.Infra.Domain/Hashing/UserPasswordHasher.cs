using Microsoft.AspNetCore.Identity;
using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios;

namespace Sestio.Usuarios.Infra.Domain.Hashing;

public sealed class UserPasswordHasher<TUser> : IUserPasswordHasher<TUser>
    where TUser : Usuario
{
    private readonly IPasswordHasher<TUser> _hasher;

    public UserPasswordHasher(IPasswordHasher<TUser> hasher)
    {
        _hasher = hasher;
    }

    public HashedPassword Hash(TUser user, string password)
    {
        var result = _hasher.HashPassword(user, password);
        return new HashedPassword(result);
    }

    public bool IsValid(TUser user, string password)
    {
        var result = _hasher.VerifyHashedPassword(user, user.Senha.Value, password);

        return result switch
        {
            PasswordVerificationResult.Success => true,
            PasswordVerificationResult.Failed => false,
            PasswordVerificationResult.SuccessRehashNeeded or _ =>
                throw new Exception($"[Password verification] unexpected result: '{result}'")
        };
    }
}
