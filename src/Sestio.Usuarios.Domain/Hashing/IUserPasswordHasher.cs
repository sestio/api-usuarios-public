using Sestio.Usuarios.Domain.Usuarios;

namespace Sestio.Usuarios.Domain.Hashing;

public sealed record HashedPassword(string Value);

public interface IUserPasswordHasher<in TUser>
    where TUser : Usuario
{
    public HashedPassword Hash(TUser user, string password);
    public bool IsValid(TUser user, string password);
}
