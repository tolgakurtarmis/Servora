using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class TableConfiguration : IEntityTypeConfiguration<Table>
{
    public void Configure(EntityTypeBuilder<Table> builder)
    {
        builder.ToTable("tables");
        builder.ConfigureEntity();

        builder.Property(table => table.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(table => table.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(table => table.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(table => table.BranchId);
        builder.HasIndex(table => table.TenantId);
        builder.HasIndex(table => new { table.BranchId, table.Name })
            .IsUnique()
            .HasFilter("\"IsDeleted\" = false");
    }
}
