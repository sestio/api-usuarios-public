using Sestio.Usuarios.Domain.Hashing;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.TestLib.Stubs;

public class StubUserPasswordHasher : IUserPasswordHasher<Usuario>
{
    private readonly string _passwordHash;

    public StubUserPasswordHasher(string passwordHash)
    {
        _passwordHash = passwordHash;
    }

    public HashedPassword Hash(Usuario user, string password)
    {
        return new HashedPassword(_passwordHash);
    }

    public bool IsValid(Usuario user, string password)
    {
        throw new NotSupportedException();
    }
}
