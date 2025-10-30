namespace Destination.Application.Features.Identity.Roles.DeleteRole;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class DeleteRoleCommandHandler(IRoleService roleService) : ICommandHandler<DeleteRoleCommand, string>
{
    public async Task<Result<string>> Handle(DeleteRoleCommand command, CancellationToken cancellationToken)
    {
        var roleName = await roleService.DeleteAsync(command.RoleId);

        return roleName.Value;
    }
}
