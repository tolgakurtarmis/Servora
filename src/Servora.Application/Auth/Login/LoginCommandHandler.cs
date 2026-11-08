using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly UserManager<User> _users;
    private readonly IUserBranchRepository _userBranches;
    private readonly IAccessTokenIssuer _accessTokens;
    private readonly IRefreshTokenService _refreshTokens;

    public LoginCommandHandler(
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

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _users.FindByEmailAsync(request.Email);
        var passwordMatches = user is not null && await _users.CheckPasswordAsync(user, request.Password);

        if (user is null || !passwordMatches || !user.IsActive)
        {
            throw new UnauthorizedException("Email or password is incorrect.");
        }

        Guid? branchId = null;
        if (user.TenantId is not null)
        {
            var membership = await _userBranches.GetFirstByUserIdAsync(user.Id, cancellationToken);
            branchId = membership?.BranchId;
        }

        var accessToken = _accessTokens.Issue(
            user.Id,
            user.Email ?? request.Email,
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
