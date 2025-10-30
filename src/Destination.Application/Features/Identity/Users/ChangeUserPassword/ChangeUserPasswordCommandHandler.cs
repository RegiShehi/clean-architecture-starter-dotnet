namespace Destination.Application.Features.Identity.Users.ChangeUserPassword;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class ChangeUserPasswordCommandHandler(IUserService userService)
    : ICommandHandler<ChangeUserPasswordCommand, Guid>
{
    public async Task<Result<Guid>> Handle(ChangeUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.ChangePasswordAsync(command.ChangePasswordRequest);

        return userId;
    }
}
