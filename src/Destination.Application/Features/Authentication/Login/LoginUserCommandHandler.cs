namespace Destination.Application.Features.Authentication.Login;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class LoginUserCommandHandler(IAuthService authService)
    : ICommandHandler<LoginUserCommand, LoginResponse>
{
    public async Task<Result<LoginResponse>> Handle(LoginUserCommand command, CancellationToken cancellationToken)
    {
        var token = await authService.LoginAsync(command.LoginRequest);

        return token;
    }
}
