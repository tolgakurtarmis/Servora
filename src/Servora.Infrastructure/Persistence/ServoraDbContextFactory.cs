using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Servora.Application.Abstractions;

namespace Servora.Infrastructure.Persistence;

public sealed class ServoraDbContextFactory : IDesignTimeDbContextFactory<ServoraDbContext>
{
    public ServoraDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<ServoraDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=servora;Username=postgres;Password=postgres")
            .UseSnakeCaseNamingConvention()
            .Options;

        return new ServoraDbContext(options, new DesignTimeCurrentTenant());
    }

    private sealed class DesignTimeCurrentTenant : ICurrentTenant
    {
        public Guid? TenantId => null;

        public Guid? BranchId => null;

        public bool IsPlatformAdmin => true;
    }
}
