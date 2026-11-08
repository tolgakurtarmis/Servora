using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class OrderRoundConfiguration : IEntityTypeConfiguration<OrderRound>
{
    public void Configure(EntityTypeBuilder<OrderRound> builder)
    {
        builder.ToTable("order_rounds");
        builder.ConfigureEntity();

        builder.HasOne(round => round.TableSession)
            .WithMany()
            .HasForeignKey(round => round.TableSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(round => round.Branch)
            .WithMany()
            .HasForeignKey(round => round.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(round => round.Tenant)
            .WithMany()
            .HasForeignKey(round => round.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(round => round.Lines)
            .WithOne(line => line.OrderRound)
            .HasForeignKey(line => line.OrderRoundId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(round => round.TableSessionId);
        builder.HasIndex(round => round.BranchId);
        builder.HasIndex(round => round.TenantId);
    }
}
