using Microsoft.EntityFrameworkCore;
using Sestio.Commons.Infra.EntityFramework.EntityTypeMapping;
using Sestio.Usuarios.Domain.Sessoes.Entities;

namespace Sestio.Usuarios.Infra.EntityFramework.Mappings;

public class SessaoMapping : RawEntityMappingConfiguration<Sessao>
{
    public override void Configure(RawEntityMappingBuilder<Sessao> builder)
    {
        builder.ToTable("sessao");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id").ValueGeneratedNever();
        builder.Property(p => p.IdUsuario).HasColumnName("id_usuario").IsRequired();
        builder.Property(p => p.Data).HasColumnName("data_criacao").IsRequired();
        builder.Property(p => p.Validade).HasColumnName("data_validade").IsRequired();
    }
}
