using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Servora.Application.Abstractions;
using Servora.Domain.Abstractions;
using Servora.Domain.Entities;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

public sealed class ServoraDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    private readonly ICurrentTenant _currentTenant;

    public ServoraDbContext(DbContextOptions<ServoraDbContext> options, ICurrentTenant currentTenant)
        : base(options)
    {
        _currentTenant = currentTenant;
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Branch> Branches => Set<Branch>();

    public DbSet<Subscription> Subscriptions => Set<Subscription>();

    public DbSet<Table> Tables => Set<Table>();

    public DbSet<MenuItem> MenuItems => Set<MenuItem>();

    public DbSet<TableSession> TableSessions => Set<TableSession>();

    public DbSet<OrderRound> OrderRounds => Set<OrderRound>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    public DbSet<Payment> Payments => Set<Payment>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    public Guid? CurrentTenantId => _currentTenant.TenantId;

    public Guid? CurrentBranchId => _currentTenant.BranchId;

    public bool CurrentUserIsPlatformAdmin => _currentTenant.IsPlatformAdmin;

    public override int SaveChanges()
    {
        ApplyAuditInfo();
        return base.SaveChanges();
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ApplyAuditInfo();
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ServoraDbContext).Assembly);
        modelBuilder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        modelBuilder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        modelBuilder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        modelBuilder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        modelBuilder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
        ApplyQueryFilters(modelBuilder);
    }

    private void ApplyAuditInfo()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<IAudited>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreateDate = now;
                entry.Property(entity => entity.ModifyDate).IsModified = false;
                entry.Property(entity => entity.ModifyBy).IsModified = false;
            }
            else if (entry.State == EntityState.Modified)
            {
                entry.Entity.ModifyDate = now;
                entry.Property(entity => entity.CreateDate).IsModified = false;
                entry.Property(entity => entity.CreateBy).IsModified = false;
            }
        }
    }

    private void ApplyQueryFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (!typeof(Entity).IsAssignableFrom(clrType))
            {
                continue;
            }

            var isTenantEntity = typeof(ITenantEntity).IsAssignableFrom(clrType);
            var isBranchEntity = typeof(IBranchEntity).IsAssignableFrom(clrType);

            var methodName = (isTenantEntity, isBranchEntity) switch
            {
                (true, true) => nameof(ApplyTenantAndBranchFilter),
                (true, false) => nameof(ApplyTenantFilter),
                (false, true) => nameof(ApplyBranchFilter),
                _ => nameof(ApplySoftDeleteFilter)
            };

            GetType()
                .GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)!
                .MakeGenericMethod(clrType)
                .Invoke(this, [modelBuilder]);
        }
    }

    private void ApplySoftDeleteFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity => !entity.IsDeleted);
    }

    private void ApplyTenantFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entity, ITenantEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity =>
            !entity.IsDeleted
            && (CurrentUserIsPlatformAdmin || entity.TenantId == CurrentTenantId));
    }

    private void ApplyBranchFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entity, IBranchEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity =>
            !entity.IsDeleted
            && (CurrentUserIsPlatformAdmin || CurrentBranchId == null || entity.BranchId == CurrentBranchId));
    }

    private void ApplyTenantAndBranchFilter<TEntity>(ModelBuilder modelBuilder)
        where TEntity : Entity, ITenantEntity, IBranchEntity
    {
        modelBuilder.Entity<TEntity>().HasQueryFilter(entity =>
            !entity.IsDeleted
            && (CurrentUserIsPlatformAdmin
                || (entity.TenantId == CurrentTenantId
                    && (CurrentBranchId == null || entity.BranchId == CurrentBranchId))));
    }
}
