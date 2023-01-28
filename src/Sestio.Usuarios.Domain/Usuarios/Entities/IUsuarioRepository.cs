using Sestio.Commons.Domain;
using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.Domain.Usuarios.Entities;

public interface IUsuarioRepository : IRepository<Usuario>
{
    Task<Usuario?> ObterPorEmailAsync(string email);
    Task<Usuario> ObterPorSessaoAsync(Sessao sessao);
}
