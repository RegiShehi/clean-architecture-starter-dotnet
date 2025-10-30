namespace Destination.Infrastructure.Identity;

using System.Security.Claims;
using Application.Abstractions.Authentication;
using Application.Features.Identity.Roles;
using Application.Features.Identity.Roles.CreateRole;
using Application.Features.Identity.Roles.UpdateRole;
using Application.Features.Identity.Roles.UpdateRolePermissions;
using Constants;
using Database;
using Domain.Features.Identity.Roles;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Models;
using SharedKernel;

public class RoleService(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ApplicationDbContext context
) : IRoleService
{
    public async Task<Result<string>> CreateAsync(CreateRoleRequest request)
    {
        var role = new ApplicationRole
        {
            Name = request.Name,
            Description = request.Description
        };

        var result = await roleManager.CreateAsync(role);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(RoleErrors.CreateFailed(GetErrorDescriptions(result)));
        }

        return role.Name;
    }

    public async Task<Result<string>> DeleteAsync(Guid id)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());

        if (role?.Name is null)
        {
            return Result.Failure<string>(RoleErrors.NotFound);
        }

        if (RoleConstants.IsDefault(role.Name))
        {
            return Result.Failure<string>(RoleErrors.DefaultRoleConflict(role.Name));
        }

        var usersInRole = await userManager.GetUsersInRoleAsync(role.Name);

        if (usersInRole.Count > 0)
        {
            return Result.Failure<string>(RoleErrors.RoleInUse(role.Name));
        }

        var result = await roleManager.DeleteAsync(role);

        if (!result.Succeeded)
        {
            return Result.Failure<string>(RoleErrors.DeleteFailed(role.Name, GetErrorDescriptions(result)));
        }

        return role.Name;
    }

    public async Task<Result<List<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        var roles = await roleManager.Roles.ToListAsync(cancellationToken);

        var dtoList = roles.Select(MapToRoleDto).ToList();

        return dtoList;
    }

    public async Task<Result<RoleDto>> GetRoleByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.SingleOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDto>(RoleErrors.NotFound);
        }

        return MapToRoleDto(role);
    }

    public async Task<Result<RoleDto>> GetRoleWithPermissionsAsync(Guid roleId,
        CancellationToken cancellationToken = default)
    {
        var role = await context.Roles.SingleOrDefaultAsync(r => r.Id == roleId, cancellationToken);

        if (role is null)
        {
            return Result.Failure<RoleDto>(RoleErrors.NotFound);
        }

        var roleWithPermissions = MapToRoleDto(role);

        var permissions = await context.RoleClaims
            .Where(rc => rc.RoleId == roleId && rc.ClaimType == ClaimConstants.Permission)
            .Select(rc => rc.ClaimValue)
            .Where(v => v != null)
            .Cast<string>()
            .ToListAsync(cancellationToken);

        roleWithPermissions.SetPermissions(permissions);

        return roleWithPermissions;
    }

    public async Task<Result<Guid>> UpdateAsync(Guid id, UpdateRoleRequest request)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());

        if (role is null)
        {
            return Result.Failure<Guid>(RoleErrors.NotFound);
        }

        if (role.Name is not null && RoleConstants.IsDefault(role.Name))
        {
            return Result.Failure<Guid>(RoleErrors.DefaultRoleConflict(role.Name));
        }

        role.Name = request.Name;
        role.Description = request.Description;
        role.NormalizedName = request.Name.ToUpperInvariant();

        var result = await roleManager.UpdateAsync(role);

        if (!result.Succeeded)
        {
            return Result.Failure<Guid>(RoleErrors.UpdateFailed(GetErrorDescriptions(result)));
        }

        return role.Id;
    }

    public async Task<Result<Guid>> UpdatePermissionsAsync(Guid id, UpdateRolePermissionsRequest request)
    {
        var role = await roleManager.FindByIdAsync(id.ToString());

        if (role is null)
        {
            return Result.Failure<Guid>(RoleErrors.NotFound);
        }

        var existingClaims = await roleManager.GetClaimsAsync(role);

        var existingPermissions = existingClaims
            .Where(c => c.Type == ClaimConstants.Permission)
            .Select(c => c.Value)
            // .Where(_ => true)
            .ToHashSet();

        var requestedPermissions = request.Permissions.ToHashSet();

        var permissionsToRemove = existingPermissions.Except(requestedPermissions).ToList();
        var permissionsToAdd = requestedPermissions.Except(existingPermissions).ToList();

        foreach (var permission in permissionsToRemove)
        {
            var res = await roleManager.RemoveClaimAsync(role, new Claim(ClaimConstants.Permission, permission));

            if (!res.Succeeded)
            {
                return Result.Failure<Guid>(RoleErrors.PermissionRemoveFailed(GetErrorDescriptions(res)));
            }
        }

        foreach (var permissions in permissionsToAdd)
        {
            var res = await roleManager.AddClaimAsync(role, new Claim(ClaimConstants.Permission, permissions));

            if (!res.Succeeded)
            {
                return Result.Failure<Guid>(RoleErrors.PermissionAddFailed(GetErrorDescriptions(res)));
            }
        }

        return role.Id;
    }

    private static List<string> GetErrorDescriptions(IdentityResult result) =>
        [.. result.Errors.Select(error => error.Description)];

    private static RoleDto MapToRoleDto(ApplicationRole role)
    {
        return new RoleDto
        {
            Id = role.Id,
            Name = role.Name ?? string.Empty,
            Description = role.Description
        };
    }
}
