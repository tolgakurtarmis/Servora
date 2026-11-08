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

    public Guid? TenantId => ReadGuid("tenant_id");

    public Guid? BranchId => ReadGuid("branch_id");

    public bool IsPlatformAdmin
    {
        get
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user is null)
            {
                return false;
            }

            return user.FindAll("role")
                .Concat(user.FindAll(ClaimTypes.Role))
                .Any(claim => claim.Value == "PlatformAdmin");
        }
    }

    private Guid? ReadGuid(string claimType)
    {
        var value = _httpContextAccessor.HttpContext?.User.FindFirstValue(claimType);
        return Guid.TryParse(value, out var id) ? id : null;
    }
}
