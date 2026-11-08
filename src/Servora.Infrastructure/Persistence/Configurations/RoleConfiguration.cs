using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Servora.Domain.Enums;

namespace Servora.Infrastructure.Persistence.Configurations;

internal sealed class RoleConfiguration : IEntityTypeConfiguration<IdentityRole<Guid>>
{
    public static readonly Guid PlatformAdminId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-111111111111");
    public static readonly Guid OwnerId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-222222222222");
    public static readonly Guid ManagerId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-333333333333");
    public static readonly Guid WaiterId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-444444444444");
    public static readonly Guid KitchenId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-555555555555");
    public static readonly Guid CashierId = Guid.Parse("6f1c1d2a-5b4e-4a1a-9c11-666666666666");

    public void Configure(EntityTypeBuilder<IdentityRole<Guid>> builder)
    {
        builder.ToTable("roles");

        builder.HasData(
            Create(PlatformAdminId, Role.PlatformAdmin),
            Create(OwnerId, Role.Owner),
            Create(ManagerId, Role.Manager),
            Create(WaiterId, Role.Waiter),
            Create(KitchenId, Role.Kitchen),
            Create(CashierId, Role.Cashier));
    }

    private static IdentityRole<Guid> Create(Guid id, Role role)
    {
        var name = role.ToString();
        return new IdentityRole<Guid>
        {
            Id = id,
            Name = name,
            NormalizedName = name.ToUpperInvariant(),
            ConcurrencyStamp = id.ToString()
        };
    }
}
