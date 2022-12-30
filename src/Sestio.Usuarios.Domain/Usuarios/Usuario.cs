using Sestio.Commons.Domain;
using Sestio.Usuarios.Domain.Hashing;

namespace Sestio.Usuarios.Domain.Usuarios;

public class Usuario : IIdentifiable
{
    internal Usuario(Guid idTenant,
                     string nome,
                     string email)
    {
        IdTenant = idTenant;
        Nome = nome;
        Email = email;
        Senha = new HashedPassword("");
    }

    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid IdTenant { get; }
    public string Nome { get; }
    public string Email { get; }
    public HashedPassword Senha { get; private set; }

    public void AtualizarSenha(HashedPassword hashedPassword)
    {
        Senha = hashedPassword;
    }
}
