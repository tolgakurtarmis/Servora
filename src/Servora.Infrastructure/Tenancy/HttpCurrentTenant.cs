using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Servora.Application.Abstractions;

namespace Servora.Infrastructure.Tenancy;

internal sealed class HttpCurrentTenant : ICurrentTenant
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentTenant(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return null;
            }

            var raw =
                user.FindFirstValue("sub")
                ?? user.FindFirstValue(ClaimTypes.NameIdentifier);

            return Guid.TryParse(raw, out var id) ? id : null;
        }
    }

    public Guid? TenantId => ReadGuid("tenant_id");

    public Guid? BranchId => ReadGuid("branch_id");

    public string? Role
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return null;
            }

            return user.FindFirstValue("role")
                ?? user.FindFirstValue(ClaimTypes.Role);
        }
    }

    public bool IsPlatformAdmin => Role == "PlatformAdmin";

    private Guid? ReadGuid(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
