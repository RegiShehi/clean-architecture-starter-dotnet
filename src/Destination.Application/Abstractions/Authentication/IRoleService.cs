namespace Destination.Application.Abstractions.Authentication;

using Features.Identity.Roles;
using Features.Identity.Roles.CreateRole;
using Features.Identity.Roles.UpdateRole;
using Features.Identity.Roles.UpdateRolePermissions;
using SharedKernel;

public interface IRoleService
{
    Task<Result<string>> CreateAsync(CreateRoleRequest request);

    Task<Result<Guid>> UpdateAsync(Guid id, UpdateRoleRequest request);

    Task<Result<string>> DeleteAsync(Guid id);

    Task<Result<Guid>> UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request);

    Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<Result<RoleDto>> GetRoleWithPermissionsAsync(Guid roleId, CancellationToken cancellationToken = default);
}
