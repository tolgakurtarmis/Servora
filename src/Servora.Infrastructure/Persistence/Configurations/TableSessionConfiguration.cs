using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class TableSessionConfiguration : IEntityTypeConfiguration<TableSession>
{
    public void Configure(EntityTypeBuilder<TableSession> builder)
    {
        builder.ToTable("table_sessions");
        builder.ConfigureEntity();

        builder.Property(session => session.Status)
            .IsRequired();

        builder.HasOne(session => session.Table)
            .WithMany()
            .HasForeignKey(session => session.TableId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Branch>()
            .WithMany()
            .HasForeignKey(session => session.BranchId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(session => session.TenantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(session => session.TableId);
        builder.HasIndex(session => session.BranchId);
        builder.HasIndex(session => session.TenantId);
        builder.HasIndex(session => session.TableId)
            .IsUnique()
            .HasFilter($"\"status\" = {(int)TableSessionStatus.Open} AND \"is_deleted\" = false");
    }
}
