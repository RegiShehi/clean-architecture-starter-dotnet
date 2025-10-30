namespace Destination.Application.Features.Identity.Roles.UpdateRolePermissions;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class UpdateRolePermissionsCommandHandler(IRoleService roleService)
    : ICommandHandler<UpdateRolePermissionsCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateRolePermissionsCommand command, CancellationToken cancellationToken)
    {
        var result = await roleService.UpdatePermissionsAsync(command.RoleId, command.UpdateRolePermissionsRequest);

        return result.Value;
    }
}
