namespace Destination.Application.Features.Identity.Roles.GetRoleById;

using Abstractions.Authentication;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetRoleByIdQueryHandler(IRoleService roleService)
    : IQueryHandler<GetRoleByIdQuery, RoleDto>
{
    public async Task<Result<RoleDto>> Handle(
        GetRoleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var role = await roleService.GetRoleByIdAsync(request.RoleId, cancellationToken);

        return role;
    }
}
