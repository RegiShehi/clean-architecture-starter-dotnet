namespace CleanArchitecture.Application.Features.Identity.Roles.GetRoleWithPermissions;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetRoleWithPermissionsQueryHandler(IRoleService roleService)
    : IQueryHandler<GetRoleWithPermissionsQuery, RoleDto>
{
    public async Task<Result<RoleDto>> Handle(
        GetRoleWithPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleWithPermissionsAsync(request.RoleId, cancellationToken);

        return role;
    }
}
