namespace CleanArchitecture.Application.Features.Identity.Users.UpdateUser;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class UpdateUserCommandHandler(IUserService userService)
    : ICommandHandler<UpdateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.UpdateUserAsync(command.UserId, command.UpdateUserRequest);

        return userId;
    }
}
