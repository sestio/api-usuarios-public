using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.EntityFramework.EntityTypeMapping;
using Sestio.Usuarios.Domain.Usuarios.Entities;

namespace Sestio.Usuarios.Infra.EntityFramework.Mappings;

public class UsuarioMapping : RawEntityMappingConfiguration<Usuario>
{
    public override void Configure(RawEntityMappingBuilder<Usuario> builder)
    {
        builder.ToTable("usuario");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.IdTenant).HasColumnName("idtenant").IsRequired();
        builder.Property(p => p.Nome).HasColumnName("nome").IsRequired();
        builder.Property(p => p.Email).HasColumnName("email").IsRequired();
        builder.OwnsOne(p => p.Senha, b => b.Property(p => p.Value).HasColumnName("senha").IsRequired());

        builder.HasIndex(p => p.Email).IsUnique();
    }
}
