namespace CleanArchitecture.Domain.Features.Identity.Roles;

using SharedKernel;

public static class RoleErrors
{
    public static readonly Error NotFound = Error.NotFound(
        "Role.NotFound", "Role not found.");

    public static Error DefaultRoleConflict(string roleName) => Error.Conflict(
        "Role.DefaultRoleConflict", $"Changes not allowed on {roleName} role.");

    public static Error RoleInUse(string roleName) => Error.Conflict(
        "Role.InUse", $"Not allowed to delete {roleName} role as it is already in use.");

    public static Error CreateFailed(IEnumerable<string> reasons) => Error.Problem(
        "Role.CreateFailed", $"Failed to create role. {string.Join(" ", reasons)}");

    public static Error UpdateFailed(IEnumerable<string> reasons) => Error.Problem(
        "Role.UpdateFailed", $"Failed to update role. {string.Join(" ", reasons)}");

    public static Error DeleteFailed(string roleName, IEnumerable<string> reasons) => Error.Problem(
        "Role.DeleteFailed", $"Failed to delete {roleName} role. {string.Join(" ", reasons)}");

    public static Error PermissionRemoveFailed(IEnumerable<string> reasons) => Error.Problem(
        "Role.PermissionRemoveFailed", $"Failed to remove permission(s). {string.Join(" ", reasons)}");

    public static Error PermissionAddFailed(IEnumerable<string> reasons) => Error.Problem(
        "Role.PermissionAddFailed", $"Failed to add permission(s). {string.Join(" ", reasons)}");
}
