using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.Property(user => user.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(user => user.Role)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(user => user.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(user => user.TenantId);
        builder.HasIndex(user => user.IsDeleted);

        builder.HasQueryFilter(user => !user.IsDeleted);
    }
}
