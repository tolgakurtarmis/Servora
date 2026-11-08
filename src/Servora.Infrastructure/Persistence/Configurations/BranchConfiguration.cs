using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class BranchConfiguration : IEntityTypeConfiguration<Branch>
{
    public void Configure(EntityTypeBuilder<Branch> builder)
    {
        builder.ToTable("branches");
        builder.ConfigureEntity();

        builder.Property(branch => branch.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(branch => branch.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(branch => branch.TenantId);
    }
}
