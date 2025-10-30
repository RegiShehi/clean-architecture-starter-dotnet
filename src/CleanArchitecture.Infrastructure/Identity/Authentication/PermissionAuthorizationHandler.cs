namespace CleanArchitecture.Infrastructure.Identity.Authentication;

using Application.Abstractions.Identity;
using Microsoft.AspNetCore.Authorization;

public class PermissionAuthorizationHandler(IUserService userService) : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userId = context.User.GetUserId();

        if (userId == Guid.Empty)
        {
            return;
        }

        var permissionsAssigned = await userService.IsPermissionAssignedAsync(userId, requirement.Permission);

        if (permissionsAssigned.Value)
        {
            context.Succeed(requirement);
        }
    }
}
