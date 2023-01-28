using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.EntityFramework.EntityTypeMapping;
using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.Infra.EntityFramework.Mappings;

public class RefreshTokenMapping : RawEntityMappingConfiguration<RefreshToken>
{
    public override void Configure(RawEntityMappingBuilder<RefreshToken> builder)
    {
        builder.ToTable("token_sessao");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.Situacao).HasColumnName("situacao");
        builder.Property(p => p.IdSessao).HasColumnName("id_sessao");
        builder.Property(p => p.Token).HasColumnName("token");

        builder.HasIndex(p => p.Token).IsUnique();
    }
}
