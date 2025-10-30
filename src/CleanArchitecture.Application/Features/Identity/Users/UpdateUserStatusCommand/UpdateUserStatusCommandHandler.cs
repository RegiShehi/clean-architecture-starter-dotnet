namespace CleanArchitecture.Application.Features.Identity.Users.UpdateUserStatusCommand;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class UpdateUserStatusCommandHandler(IUserService userService)
    : ICommandHandler<UpdateUserStatusCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateUserStatusCommand command, CancellationToken cancellationToken)
    {
        var userId = await userService.ActivateOrDeactivateAsync(command.UserId, command.Activation);

        return userId;
    }
}
