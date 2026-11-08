using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class MenuCategoryConfiguration : IEntityTypeConfiguration<MenuCategory>
{
    public void Configure(EntityTypeBuilder<MenuCategory> builder)
    {
        builder.ToTable("menu_categories");
        builder.ConfigureEntity();

        builder.Property(category => category.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(category => category.SortOrder)
            .IsRequired();

        builder.HasOne(category => category.Branch)
            .WithMany()
            .HasForeignKey(category => category.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(category => category.Tenant)
            .WithMany()
            .HasForeignKey(category => category.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(category => category.BranchId);
        builder.HasIndex(category => category.TenantId);
        builder.HasIndex(category => new { category.BranchId, category.Name })
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
