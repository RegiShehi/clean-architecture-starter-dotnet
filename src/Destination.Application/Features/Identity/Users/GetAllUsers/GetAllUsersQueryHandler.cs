namespace Destination.Application.Features.Identity.Users.GetAllUsers;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetAllUsersQueryHandler(IUserService userService)
    : IQueryHandler<GetAllUsersQuery, List<UserDto>>
{
    public async Task<Result<List<UserDto>>> Handle(
        GetAllUsersQuery request,
        CancellationToken cancellationToken)
    {
        var users = await userService.GetUsersAsync(cancellationToken);

        return users;
    }
}
