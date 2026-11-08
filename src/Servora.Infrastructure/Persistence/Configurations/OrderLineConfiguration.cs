using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class OrderLineConfiguration : IEntityTypeConfiguration<OrderLine>
{
    public void Configure(EntityTypeBuilder<OrderLine> builder)
    {
        builder.ToTable("order_lines");
        builder.ConfigureEntity();

        builder.Property(line => line.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(line => line.UnitPrice)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(line => line.Quantity)
            .IsRequired();

        builder.Property(line => line.KitchenStatus)
            .IsRequired();

        builder.HasOne(line => line.MenuItem)
            .WithMany()
            .HasForeignKey(line => line.MenuItemId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(line => line.Branch)
            .WithMany()
            .HasForeignKey(line => line.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(line => line.Tenant)
            .WithMany()
            .HasForeignKey(line => line.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(line => line.OrderRoundId);
        builder.HasIndex(line => line.MenuItemId);
        builder.HasIndex(line => line.BranchId);
        builder.HasIndex(line => line.TenantId);
    }
}
