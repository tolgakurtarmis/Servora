using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Auth.Refresh;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly UserManager<User> _users;
    private readonly IUserBranchRepository _userBranches;
    private readonly IAccessTokenIssuer _accessTokens;
    private readonly IRefreshTokenService _refreshTokens;

    public RefreshTokenCommandHandler(
        UserManager<User> users,
        IUserBranchRepository userBranches,
        IAccessTokenIssuer accessTokens,
        IRefreshTokenService refreshTokens)
    {
        _users = users;
        _userBranches = userBranches;
        _accessTokens = accessTokens;
        _refreshTokens = refreshTokens;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var existing = await _refreshTokens.FindActiveAsync(request.RefreshToken, cancellationToken);
        if (existing is null)
        {
            throw new UnauthorizedException("Refresh token is invalid or expired.");
        }

        var user = await _users.FindByIdAsync(existing.UserId.ToString());
        if (user is null || !user.IsActive || user.IsDeleted)
        {
            _refreshTokens.Revoke(existing);
            throw new UnauthorizedException("Refresh token is invalid or expired.");
        }

        _refreshTokens.Revoke(existing);

        Guid? branchId = null;
        if (user.TenantId is not null)
        {
            var membership = await _userBranches.GetFirstByUserIdAsync(user.Id, cancellationToken);
            branchId = membership?.BranchId;
        }

        var accessToken = _accessTokens.Issue(
            user.Id,
            user.Email ?? string.Empty,
            user.Role.ToString(),
            user.TenantId,
            branchId);
        var refreshToken = await _refreshTokens.IssueAsync(user.Id, cancellationToken);

        return new AuthResponse(
            accessToken.Value,
            accessToken.ExpiresAt,
            refreshToken.Value,
            refreshToken.ExpiresAt);
    }
}
