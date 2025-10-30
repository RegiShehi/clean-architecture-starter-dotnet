namespace Destination.Application.Abstractions.Identity;

using Features.Identity.Users;
using Features.Identity.Users.ChangeUserPassword;
using Features.Identity.Users.CreateUser;
using Features.Identity.Users.UpdateUser;
using Features.Identity.Users.UpdateUserRoles;
using SharedKernel;

public interface IUserService
{
    Task<Result<Guid>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken);

    Task<Result<Guid>> UpdateUserAsync(Guid userId, UpdateUserRequest request);

    Task<Result<Guid>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result<Guid>> ActivateOrDeactivateAsync(Guid userId, bool activation);

    Task<Result<Guid>> ChangePasswordAsync(ChangePasswordRequest request);

    Task<Result<Guid>> AssignRolesAsync(Guid userId, UpdateUserRolesRequest request);

    Task<Result<List<UserDto>>> GetUsersAsync(CancellationToken cancellationToken);

    Task<Result<UserDto>> GetUserByIdAsync(Guid userId);

    Task<Result<List<UserRoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken);

    Task<bool> IsEmailTakenAsync(string email);

    Task<Result<List<string>>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken);

    Task<Result<bool>> IsPermissionAssignedAsync(
        Guid userId,
        string permission,
        CancellationToken cancellationToken = default);
}
