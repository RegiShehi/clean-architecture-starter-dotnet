namespace Destination.Domain.Features.Identity.Users;

using SharedKernel;

public static class UserErrors
{
    public static readonly Error PasswordsDoNotMatch =
        Error.Conflict("User.PasswordsDoNotMatch", "Passwords do not match.");

    public static readonly Error EmailTaken =
        Error.Conflict("User.EmailTaken", "Email is already taken.");

    public static readonly Error AdminCountTooLow =
        Error.Conflict("User.AdminCountTooLow", "Tenant should have at least 2 admin users.");

    public static Error NotFound(string id) =>
        Error.NotFound("User.NotFound", $"User {id} not found.");

    public static Error CreateFailed(IEnumerable<string> reasons) =>
        Error.Problem("User.CreateFailed", "Failed to create user. " + string.Join(" ", reasons));

    public static Error UpdateFailed(IEnumerable<string> reasons) =>
        Error.Problem("User.UpdateFailed", "Failed to update user. " + string.Join(" ", reasons));

    public static Error DeleteFailed(IEnumerable<string> reasons) =>
        Error.Problem("User.DeleteFailed", "Failed to delete user. " + string.Join(" ", reasons));

    public static Error ChangePasswordFailed(IEnumerable<string> reasons) =>
        Error.Problem("User.ChangePasswordFailed", "Failed to change password. " + string.Join(" ", reasons));

    public static Error AssignRoleFailed(string roleName, IEnumerable<string> reasons) =>
        Error.Problem("User.AssignRoleFailed", $"Failed to assign role '{roleName}'. {string.Join(" ", reasons)}");

    public static Error RemoveRoleFailed(string roleName, IEnumerable<string> reasons) =>
        Error.Problem("User.RemoveRoleFailed", $"Failed to remove role '{roleName}'. {string.Join(" ", reasons)}");
}
