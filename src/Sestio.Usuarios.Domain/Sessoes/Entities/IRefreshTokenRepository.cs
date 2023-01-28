using Sestio.Commons.Domain;

namespace Sestio.Usuarios.Domain.Sessoes.Entities;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> ObterPorTokenAsync(string token);
    Task<RefreshToken?> ObterAtivoPorSessaoAsync(Sessao sessao);
}
