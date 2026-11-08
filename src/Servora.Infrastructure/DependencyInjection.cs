using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Servora.Application.Abstractions;
using Servora.Domain.Models;
using Servora.Infrastructure.Auth;
using Servora.Infrastructure.Persistence;
using Servora.Infrastructure.Tenancy;

namespace Servora.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentTenant, HttpCurrentTenant>();

        services.AddDbContext<ServoraDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("Servora")));

        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ServoraDbContext>();

        var jwtSection = configuration.GetSection(JwtOptions.SectionName);
        services.Configure<JwtOptions>(jwtSection);
        var jwt = jwtSection.Get<JwtOptions>() ?? new JwtOptions();


        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true, //Token’ı kim üretti? appsettings Jwt:Issuer ile aynı olmalı
                    ValidIssuer = jwt.Issuer,//Token kime? appsettings Jwt:Audience ile aynı olmalı
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateIssuerSigningKey = true,//İmza Jwt:Key ile doğrulanır. Key yanlışsa token sahte sayılır
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),//İmza Jwt:Key ile doğrulanır. Key yanlışsa token sahte sayılır
                    ValidateLifetime = true, // süre doldu token iptal
                    ClockSkew = TimeSpan.FromMinutes(1), // Sunucu saat farkı için 1 dk tolerans
                    RoleClaimType = "role",//[Authorize(Roles = "...")] hangi claim’e bakacak
                    NameClaimType = JwtRegisteredClaimNames.Sub //User.Identity.Name olarak kullanıcı id (sub) kullanır
                };
            });

        services.AddAuthorization();

        services.AddScoped<IAccessTokenIssuer, JwtAccessTokenIssuer>();
        services.AddScoped<IUserBranchRepository, UserBranchRepository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
        services.AddScoped<IBranchRepository, BranchRepository>();
        services.AddScoped<ITableRepository, TableRepository>();
        services.AddScoped<IMenuItemRepository, MenuItemRepository>();

        return services;
    }
}
