using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("subscriptions");
        builder.ConfigureEntity();

        builder.Property(subscription => subscription.Status)
            .IsRequired();

        builder.Property(subscription => subscription.MaxBranches)
            .IsRequired();

        builder.Property(subscription => subscription.MaxUsers)
            .IsRequired();

        builder.HasOne(subscription => subscription.Tenant)
            .WithMany()
            .HasForeignKey(subscription => subscription.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(subscription => subscription.TenantId)
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
