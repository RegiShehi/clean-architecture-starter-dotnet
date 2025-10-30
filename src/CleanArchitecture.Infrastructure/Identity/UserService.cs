namespace CleanArchitecture.Infrastructure.Identity;

using Application.Abstractions.Identity;
using Application.Features.Identity.Users;
using Application.Features.Identity.Users.ChangeUserPassword;
using Application.Features.Identity.Users.CreateUser;
using Application.Features.Identity.Users.UpdateUser;
using Application.Features.Identity.Users.UpdateUserRoles;
using Constants;
using Database;
using Domain.Features.Identity.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using SharedKernel;

public class UserService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    SignInManager<ApplicationUser> signInManager,
    ApplicationDbContext applicationDbContext
) : IUserService
{
    public async Task<Result<Guid>> CreateUserAsync(CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (request.Password != request.ConfirmPassword)
        {
            return Result.Failure<Guid>(UserErrors.PasswordsDoNotMatch);
        }

        var emailTaken = await IsEmailTakenInternalAsync(request.Email);

        if (emailTaken)
        {
            return Result.Failure<Guid>(UserErrors.EmailTaken);
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            UserName = request.Email,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.IsActive
        };

        var result = await userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(UserErrors.CreateFailed(GetErrorDescriptions(result)));
        }

        return user.Id;
    }

    public async Task<Result<Guid>> AssignRolesAsync(Guid userId, UpdateUserRolesRequest request)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;

        // Guard rails for Admin role removal
        var isAdmin = await userManager.IsInRoleAsync(user, RoleConstants.Admin);

        var isRemovingAdminRole =
            request.UserRoles.Any(ur => ur is { IsAssigned: false, Name: RoleConstants.Admin });

        if (isAdmin && isRemovingAdminRole)
        {
            var adminUsersCount = (await userManager.GetUsersInRoleAsync(RoleConstants.Admin)).Count;

            // Ensure there are at least 2 admin users
            if (adminUsersCount <= 2)
            {
                return Result.Failure<Guid>(UserErrors.AdminCountTooLow);
            }
        }

        // Check which roles exist
        var rolesWithExistence = await Task.WhenAll(
            request.UserRoles.Select(async r => new
            {
                r.RoleId,
                r.Name,
                r.IsAssigned,
                Exists = await roleManager.FindByIdAsync(r.RoleId.ToString()) is not null
            }));

        // Process only existing roles
        foreach (var entry in rolesWithExistence.Where(x => x.Exists))
        {
            if (string.IsNullOrWhiteSpace(entry.Name))
            {
                continue;
            }

            if (entry.IsAssigned)
            {
                var inRole = await userManager.IsInRoleAsync(user, entry.Name);

                if (!inRole)
                {
                    var addRes = await userManager.AddToRoleAsync(user, entry.Name);
                    if (!addRes.Succeeded)
                    {
                        return Result.Failure<Guid>(UserErrors.AssignRoleFailed(entry.Name,
                            GetErrorDescriptions(addRes)));
                    }
                }
            }
            else
            {
                var removeRes = await userManager.RemoveFromRoleAsync(user, entry.Name);
                if (!removeRes.Succeeded)
                {
                    return Result.Failure<Guid>(UserErrors.RemoveRoleFailed(entry.Name,
                        GetErrorDescriptions(removeRes)));
                }
            }
        }

        return user.Id;
    }

    public async Task<Result<Guid>> ActivateOrDeactivateAsync(Guid userId, bool activation)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;
        user.IsActive = activation;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(UserErrors.UpdateFailed(GetErrorDescriptions(result)));
        }

        return user.Id;
    }

    public async Task<Result<Guid>> ChangePasswordAsync(ChangePasswordRequest request)
    {
        var userResult = await GetUserAsync(request.UserId);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        if (request.NewPassword != request.ConfirmNewPassword)
        {
            return Result.Failure<Guid>(UserErrors.PasswordsDoNotMatch);
        }

        var result =
            await userManager.ChangePasswordAsync(userResult.Value, request.CurrentPassword, request.NewPassword);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(UserErrors.ChangePasswordFailed(GetErrorDescriptions(result)));
        }

        return userResult.Value.Id;
    }

    public async Task<Result<Guid>> DeleteUserAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;

        var result = await userManager.DeleteAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(UserErrors.DeleteFailed(GetErrorDescriptions(result)));
        }

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        return user.Id;
    }

    public async Task<Result<UserDto>> GetUserByIdAsync(Guid userId)
    {
        var user = await GetUserAsync(userId);

        if (user.IsFailure)
        {
            return Result.Failure<UserDto>(user.Error);
        }

        return MapToUserDto(user.Value);
    }

    public async Task<Result<List<UserRoleDto>>> GetUserRolesAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<List<UserRoleDto>>(userResult.Error);
        }

        var user = userResult.Value;
        var roles = await roleManager.Roles.AsNoTracking().ToListAsync(cancellationToken);

        var userRoles = new List<UserRoleDto>(roles.Count);

        foreach (var role in roles)
        {
            var isAssigned = !string.IsNullOrWhiteSpace(role.Name) &&
                             await userManager.IsInRoleAsync(user, role.Name);

            userRoles.Add(new UserRoleDto
            {
                RoleId = role.Id,
                Name = role.Name,
                Description = role.Description,
                IsAssigned = isAssigned
            });
        }

        return userRoles;
    }

    public async Task<Result<List<UserDto>>> GetUsersAsync(CancellationToken cancellationToken)
    {
        var users = await userManager.Users.AsNoTracking().ToListAsync(cancellationToken);

        var list = users.Select(MapToUserDto).ToList();

        return list;
    }

    public async Task<bool> IsEmailTakenAsync(string email)
    {
        var taken = await IsEmailTakenInternalAsync(email);

        return taken;
    }

    public async Task<Result<List<string>>> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<List<string>>(userResult.Error);
        }

        var userRoles = await userManager.GetRolesAsync(userResult.Value);

        // Fetch only roles present in userRoles
        var roles = await roleManager.Roles
            .Where(r => r.Name != null && userRoles.Contains(r.Name))
            .ToListAsync(cancellationToken);

        var permissions = new List<string>();

        foreach (var role in roles)
        {
            var claims = await applicationDbContext.RoleClaims
                .Where(rc => rc.RoleId == role.Id && rc.ClaimType == ClaimConstants.Permission)
                .Select(rc => rc.ClaimValue!)
                .ToListAsync(cancellationToken);

            permissions.AddRange(claims);
        }

        return permissions.Distinct().ToList();
    }

    public async Task<Result<bool>> IsPermissionAssignedAsync(Guid userId, string permission,
        CancellationToken cancellationToken = default)
    {
        var permissions = await GetPermissionsAsync(userId, cancellationToken);

        if (permissions.IsFailure)
        {
            return Result.Failure<bool>(permissions.Error);
        }

        return permissions.Value.Contains(permission);
    }

    public async Task<Result<Guid>> UpdateUserAsync(Guid userId, UpdateUserRequest request)
    {
        var userResult = await GetUserAsync(userId);

        if (userResult.IsFailure)
        {
            return Result.Failure<Guid>(userResult.Error);
        }

        var user = userResult.Value;
        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.PhoneNumber;

        var result = await userManager.UpdateAsync(user);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(UserErrors.UpdateFailed(GetErrorDescriptions(result)));
        }

        await signInManager.RefreshSignInAsync(user);

        return user.Id;
    }

    private static List<string> GetErrorDescriptions(IdentityResult result) =>
        [.. result.Errors.Select(e => e.Description)];

    private async Task<bool> IsEmailTakenInternalAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);

        return user is not null;
    }

    private async Task<Result<ApplicationUser>> GetUserAsync(Guid userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());

        return user is null
            ? Result.Failure<ApplicationUser>(UserErrors.NotFound(userId.ToString()))
            : Result.Success(user);
    }

    private static UserDto MapToUserDto(ApplicationUser user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email ?? string.Empty,
        UserName = user.Email ?? string.Empty,
        PhoneNumber = user.PhoneNumber,
        IsActive = user.IsActive
    };
}
