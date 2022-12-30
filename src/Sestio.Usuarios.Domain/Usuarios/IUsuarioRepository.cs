using Sestio.Commons.Domain;

namespace Sestio.Usuarios.Domain.Usuarios;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}
