namespace Servora.Application.Abstractions;

public interface IAccessTokenIssuer
{
    AccessToken Issue(Guid userId, string userName, string role, Guid? tenantId, Guid? branchId);
}

public sealed record AccessToken(string Value, DateTime ExpiresAt);
