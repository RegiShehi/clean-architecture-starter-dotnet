namespace Destination.Application.Features.Identity.Users.DeleteUser;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class DeleteUserCommandHandler(IUserService userService)
    : ICommandHandler<DeleteUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.DeleteUserAsync(command.UserId, cancellationToken);

        return userId;
    }
}
