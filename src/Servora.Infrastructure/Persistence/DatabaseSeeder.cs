using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(IHost host)
    {
        var environment = host.Services.GetRequiredService<IHostEnvironment>();
        if (!environment.IsDevelopment())
        {
            return;
        }

        var configuration = host.Services.GetRequiredService<IConfiguration>();
        var options = configuration.GetSection(SeedOptions.SectionName).Get<SeedOptions>();
        if (options is null || !options.Enabled)
        {
            return;
        }

        using var scope = host.Services.CreateScope();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        var existing = await users.FindByEmailAsync(options.PlatformAdminEmail);
        if (existing is not null)
        {
            return;
        }

        var user = new User
        {
            UserName = options.PlatformAdminEmail,
            Email = options.PlatformAdminEmail,
            EmailConfirmed = true,
            Name = "Platform Admin",
            Role = Role.PlatformAdmin
        };

        var created = await users.CreateAsync(user, options.PlatformAdminPassword);
        if (!created.Succeeded)
        {
            var errors = string.Join(" ", created.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to seed platform admin: {errors}");
        }

        var roleAssigned = await users.AddToRoleAsync(user, Role.PlatformAdmin.ToString());
        if (!roleAssigned.Succeeded)
        {
            var errors = string.Join(" ", roleAssigned.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Failed to assign platform admin role: {errors}");
        }
    }
}
