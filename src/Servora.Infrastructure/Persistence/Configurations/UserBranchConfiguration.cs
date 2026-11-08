using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class UserBranchConfiguration : IEntityTypeConfiguration<UserBranch>
{
    public void Configure(EntityTypeBuilder<UserBranch> builder)
    {
        builder.ToTable("user_branches");
        builder.ConfigureEntity();

        builder.HasOne(membership => membership.User)
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(membership => membership.Branch)
            .WithMany()
            .HasForeignKey(membership => membership.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(membership => membership.Tenant)
            .WithMany()
            .HasForeignKey(membership => membership.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(membership => new { membership.UserId, membership.BranchId })
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
