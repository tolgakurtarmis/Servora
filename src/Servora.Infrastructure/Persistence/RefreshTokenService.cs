using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Servora.Application.Abstractions;
using Servora.Domain.Models;
using Servora.Infrastructure.Auth;

namespace Servora.Infrastructure.Persistence;

internal sealed class RefreshTokenService : IRefreshTokenService
{
    private readonly ServoraDbContext _dbContext;
    private readonly JwtOptions _options;

    public RefreshTokenService(ServoraDbContext dbContext, IOptions<JwtOptions> options)
    {
        _dbContext = dbContext;
        _options = options.Value;
    }

    public async Task<IssuedRefreshToken> IssueAsync(Guid userId, CancellationToken cancellationToken)
    {
        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var expiresAt = DateTime.UtcNow.AddDays(_options.RefreshExpiresDays);

        await _dbContext.RefreshTokens.AddAsync(new RefreshToken
        {
            UserId = userId,
            TokenHash = Hash(value),
            ExpiresAt = expiresAt
        }, cancellationToken);

        return new IssuedRefreshToken(value, expiresAt);
    }

    public async Task<RefreshToken?> FindActiveAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var hash = Hash(refreshToken);
        var token = await _dbContext.RefreshTokens
            .FirstOrDefaultAsync(item => item.TokenHash == hash, cancellationToken);

        if (token is null || !token.IsUsable)
        {
            return null;
        }

        return token;
    }

    public void Revoke(RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        _dbContext.RefreshTokens.Update(refreshToken);
    }

    private static string Hash(string value)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(value));
        return Convert.ToHexString(bytes);
    }
}
