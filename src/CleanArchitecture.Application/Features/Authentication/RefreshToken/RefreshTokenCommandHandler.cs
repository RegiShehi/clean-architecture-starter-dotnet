namespace CleanArchitecture.Application.Features.Authentication.RefreshToken;

using Abstractions.Authentication;
using Abstractions.Messaging;
using Login;
using SharedKernel;

internal sealed class RefreshTokenCommandHandler(IAuthService authService)
    : ICommandHandler<RefreshTokenCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(RefreshTokenCommand command, CancellationToken cancellationToken)
    {
        var refreshToken = await authService.RefreshTokenAsync(command.RefreshTokenRequest);

        return refreshToken;
    }
}
