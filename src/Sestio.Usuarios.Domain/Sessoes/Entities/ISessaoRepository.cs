using Sestio.Commons.Domain;

namespace Sestio.Usuarios.Domain.Sessoes.Entities;

public interface ISessaoRepository : IRepository<Sessao>
{
    Task<Sessao> ObterPorTokenAsync(RefreshToken token);
}
