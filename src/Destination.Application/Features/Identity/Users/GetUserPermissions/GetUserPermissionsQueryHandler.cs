namespace Destination.Application.Features.Identity.Users.GetUserPermissions;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetAllUsersQueryHandler(IUserService userService)
    : IQueryHandler<GetUserPermissionsQuery, List<string>>
{
    public async Task<Result<List<string>>> Handle(
        GetUserPermissionsQuery request,
        CancellationToken cancellationToken)
    {
        var permissions = await userService.GetPermissionsAsync(request.UserId, cancellationToken);

        return permissions;
    }
}
