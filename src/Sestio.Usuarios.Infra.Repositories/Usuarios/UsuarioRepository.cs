using Sestio.Commons.Infra.Repositories;
using Sestio.Usuarios.Domain.Usuarios.Entities;
using Sestio.Usuarios.Infra.EntityFramework;

namespace Sestio.Usuarios.Infra.Repositories.Usuarios;

public sealed class UsuarioRepository : Repository<UsuariosDbContext, Usuario>, IUsuarioRepository
{
    public UsuarioRepository(UsuariosDbContext db)
        : base(db)
    {
    }
}
