namespace CleanArchitecture.Application.Features.Identity.Roles.CreateRole;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class CreateRoleCommandHandler(IRoleService roleService) : ICommandHandler<CreateRoleCommand, string>
{
    public async Task<Result<string>> Handle(CreateRoleCommand command, CancellationToken cancellationToken)
    {
        var roleName = await roleService.CreateAsync(command.CreateRoleRequest);

        return roleName.Value;
    }
}
