using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.Repositories;
using Sestio.Usuarios.Domain.Usuarios;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Infra.Repositories.Usuarios;

public sealed class UsuarioRepository : Repository<UsuarioDbContext, Usuario>, IUsuarioRepository
{
    public UsuarioRepository(UsuarioDbContext db)
        : base(db)
    {
    }

    public async Task<Usuario?> ObterPorEmailAsync(string email)
    {
        var query = CreateQueryable();
        var usuario = await query.FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower());
        return usuario;
    }
}
