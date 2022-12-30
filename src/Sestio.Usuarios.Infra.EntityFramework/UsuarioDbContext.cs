using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.EntityFramework;
using Sestio.Commons.Infra.EntityFramework.EntityTypeMapping;
using Sestio.Usuarios.Infra.EntityFramework.Mappings;

namespace Sestio.Usuarios.Infra.EntityFramework;

public class UsuarioDbContext : DefaultDbContext
{
    public UsuarioDbContext(DbContextOptions<UsuarioDbContext> options)
        : base(options, userInfo: null!)
    {
    }

    protected override void ConfigureMappings(EntityMappingBag mappings)
    {
        mappings.Add(new UsuarioMapping());
    }
}
