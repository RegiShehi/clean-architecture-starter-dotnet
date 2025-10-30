namespace Destination.Application.Features.Identity.Users.UpdateUserRoles;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class UpdateUserRolesCommandHandler(IUserService userService)
    : ICommandHandler<UpdateUserRolesCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.AssignRolesAsync(command.UserId, command.UserRolesRequest);

        return userId;
    }
}
