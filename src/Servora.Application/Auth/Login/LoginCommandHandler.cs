using MediatR;
using Microsoft.AspNetCore.Identity;
using Servora.Application.Abstractions;
using Servora.Application.Abstractions.Exceptions;
using Servora.Domain.Models;

namespace Servora.Application.Auth.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly UserManager<User> _users;
    private readonly IUserBranchRepository _userBranches;
    private readonly IAccessTokenIssuer _tokens;

    public LoginCommandHandler(
        UserManager<User> users,
        IUserBranchRepository userBranches,
        IAccessTokenIssuer tokens)
    {
        _users = users;
        _userBranches = userBranches;
        _tokens = tokens;
    }

    public async Task<LoginResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
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

        var token = _tokens.Issue(user.Id, user.Email ?? request.Email, user.Role.ToString(), user.TenantId, branchId);
        return new LoginResponse(token.Value, token.ExpiresAt);
    }
}
