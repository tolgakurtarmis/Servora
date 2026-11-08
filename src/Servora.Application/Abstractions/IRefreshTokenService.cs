using Servora.Domain.Models;

namespace Servora.Application.Abstractions;

public interface IRefreshTokenService
{
    Task<IssuedRefreshToken> IssueAsync(Guid userId, CancellationToken cancellationToken);

    Task<RefreshToken?> FindActiveAsync(string refreshToken, CancellationToken cancellationToken);

    void Revoke(RefreshToken refreshToken);
}

public sealed record IssuedRefreshToken(string Value, DateTime ExpiresAt);
