namespace CleanArchitecture.Application.Features.Identity.Users.GetUserRoles;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetUserRolesQueryHandler(IUserService userService)
    : IQueryHandler<GetUserRolesQuery, List<UserRoleDto>>
{
    public async Task<Result<List<UserRoleDto>>> Handle(
        GetUserRolesQuery request,
        CancellationToken cancellationToken)
    {
        var userRoles = await userService.GetUserRolesAsync(request.UserId, cancellationToken);

        return userRoles;
    }
}
