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

        builder.HasOne<User>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(membership => membership.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(membership => new { membership.UserId, membership.BranchId })
            .IsUnique()
            .HasFilter("\"is_deleted\" = false");
    }
}
