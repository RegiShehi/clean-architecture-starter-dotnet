namespace Destination.Application.Features.Identity.Users.CreateUser;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class CreateUserCommandHandler(IUserService userService)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.CreateUserAsync(command.CreateUserRequest, cancellationToken);

        return userId;
    }
}
