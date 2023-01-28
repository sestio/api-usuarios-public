using Sestio.Commons.Infra.Repositories;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Infra.Repositories.Sessoes;

public sealed class SessaoRepository : Repository<UsuariosDbContext, Sessao>, ISessaoRepository
{
    public SessaoRepository(UsuariosDbContext db)
        : base(db)
    {
    }

    public async Task<Sessao> ObterPorTokenAsync(RefreshToken token)
    {
        var sessao = await GetAsync(p => p.Id == token.IdSessao);

        if (sessao == null)
            throw new Exception($"Não foi possível encontrar sessão para o token com id '{token.Id}'");

        return sessao;
    }
}
