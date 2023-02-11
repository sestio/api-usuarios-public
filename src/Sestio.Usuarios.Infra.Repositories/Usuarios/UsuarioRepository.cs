using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.Repositories;
using Sestio.Usuarios.Domain.Sessoes.Entities;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Infra.Repositories.Usuarios;

public sealed class UsuarioRepository : Repository<UsuariosDbContext, Usuario>, IUsuarioRepository
{
    public UsuarioRepository(UsuariosDbContext db)
        : base(db)
    {
    }

    public Task<bool> ExistePorEmailAsync(string email)
    {
        var query = CreateQueryablePorEmail(email);
        return query.AnyAsync();
    }

    public Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var query = CreateQueryablePorEmail(email);
        return query.FirstOrDefaultAsync();
    }

    private IQueryable<Usuario> CreateQueryablePorEmail(string email)
    {
        return CreateQueryable().Where(p => p.Email == email.ToUpperInvariant());
    }

    public async Task<Usuario> ObterPorSessaoAsync(Sessao sessao)
    {
        var usuario = await GetAsync(p => p.Id == sessao.IdUsuario);

        if (usuario == null)
            throw new Exception($"Não foi possível encontrar usuário para a sessão com id '{sessao.Id}'");

        return usuario;
    }
}
