using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Entities;

namespace Servora.Infrastructure.Persistence.Configurations;

internal static class EntityTypeBuilderExtensions
{
    public static void ConfigureEntity<TEntity>(this EntityTypeBuilder<TEntity> builder)
        where TEntity : Entity
    {
        builder.HasKey(entity => entity.Id);

        builder.Property(entity => entity.CreateBy);
        builder.Property(entity => entity.CreateDate).IsRequired();
        builder.Property(entity => entity.ModifyBy);
        builder.Property(entity => entity.ModifyDate);
        builder.Property(entity => entity.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(entity => entity.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.HasIndex(entity => entity.IsDeleted);
    }
}
