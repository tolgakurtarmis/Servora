using MediatR;
using Servora.Application.Abstractions;

namespace Servora.Application.Auth.Logout;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, bool>
{
    private readonly IRefreshTokenService _refreshTokens;

    public LogoutCommandHandler(IRefreshTokenService refreshTokens)
    {
        _refreshTokens = refreshTokens;
    }

    public async Task<bool> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var existing = await _refreshTokens.FindActiveAsync(request.RefreshToken, cancellationToken);
        if (existing is not null)
        {
            _refreshTokens.Revoke(existing);
        }

        return true;
    }
}
