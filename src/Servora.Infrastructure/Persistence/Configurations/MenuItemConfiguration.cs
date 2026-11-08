using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class MenuItemConfiguration : IEntityTypeConfiguration<MenuItem>
{
    public void Configure(EntityTypeBuilder<MenuItem> builder)
    {
        builder.ToTable("menu_items");
        builder.ConfigureEntity();

        builder.Property(item => item.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(item => item.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(item => item.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(item => item.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(item => item.BranchId);
        builder.HasIndex(item => item.TenantId);
        builder.HasIndex(item => new { item.BranchId, item.Name })
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
