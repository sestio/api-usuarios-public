using Sestio.Commons.Infra.Repositories;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Infra.Repositories.Sessoes;

public sealed class RefreshTokenRepository : Repository<UsuariosDbContext, RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(UsuariosDbContext db)
        : base(db)
    {
    }

    public Task<RefreshToken?> ObterPorTokenAsync(string token)
    {
        return GetAsync(p => p.Token == token);
    }

    public async Task<RefreshToken?> ObterAtivoPorSessaoAsync(Sessao sessao)
    {
        var tokens = await GetAllAsync(p => p.IdSessao == sessao.Id && p.Situacao == SituacaoRefreshToken.Valido);

        if (tokens.Count > 1)
            throw new Exception($"Apenas um token ativo por sessão é permitida. Sessão '{sessao.Id}");

        return tokens.FirstOrDefault();
    }
}
