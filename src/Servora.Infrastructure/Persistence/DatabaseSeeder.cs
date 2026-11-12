using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Servora.Domain.Enums;
using Servora.Domain.Models;

namespace Servora.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    private const string LegacyPlatformAdminEmail = "admin@servora.local";

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

        var userName = options.PlatformAdminUserName.Trim();
        var existing = await users.FindByNameAsync(userName);
        if (existing is not null)
        {
            return;
        }

        var legacy = await users.FindByEmailAsync(LegacyPlatformAdminEmail)
            ?? await users.FindByNameAsync(LegacyPlatformAdminEmail);
        if (legacy is not null)
        {
            legacy.UserName = userName;
            legacy.Email = null;
            legacy.NormalizedEmail = null;
            var updated = await users.UpdateAsync(legacy);
            if (!updated.Succeeded)
            {
                var errors = string.Join(" ", updated.Errors.Select(error => error.Description));
                throw new InvalidOperationException($"Failed to normalize platform admin username: {errors}");
            }

            return;
        }

        var user = new User
        {
            UserName = userName,
            Email = null,
            EmailConfirmed = false,
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
