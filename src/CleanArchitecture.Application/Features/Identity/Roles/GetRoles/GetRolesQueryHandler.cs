namespace CleanArchitecture.Application.Features.Identity.Roles.GetRoles;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class
    GetRolesQueryHandler(IRoleService roleService)
    : IQueryHandler<GetRolesQuery, List<RoleDto>>
{
    public async Task<Result<List<RoleDto>>> Handle(
        GetRolesQuery request,
        CancellationToken cancellationToken)
    {
        var roles = await roleService.GetRolesAsync(cancellationToken);

        return roles;
    }
}
