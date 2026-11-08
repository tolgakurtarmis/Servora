using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("payments");
        builder.ConfigureEntity();

        builder.Property(payment => payment.Amount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(payment => payment.Method)
            .IsRequired();

        builder.HasOne<TableSession>()
            .WithMany()
            .HasForeignKey(payment => payment.TableSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(payment => payment.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(payment => payment.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(payment => payment.BranchId);
        builder.HasIndex(payment => payment.TenantId);
        builder.HasIndex(payment => payment.TableSessionId)
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
