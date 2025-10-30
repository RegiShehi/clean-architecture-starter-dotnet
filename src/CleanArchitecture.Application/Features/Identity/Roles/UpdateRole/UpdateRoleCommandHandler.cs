namespace CleanArchitecture.Application.Features.Identity.Roles.UpdateRole;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class UpdateRoleCommandHandler(IRoleService roleService) : ICommandHandler<UpdateRoleCommand, Guid>
{
    public async Task<Result<Guid>> Handle(UpdateRoleCommand command, CancellationToken cancellationToken)
    {
        var roleName = await roleService.UpdateAsync(command.RoleId, command.UpdateRoleRequest);

        return roleName.Value;
    }
}
