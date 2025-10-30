namespace Destination.Application.Features.Identity.Users.GetUserById;

using Abstractions.Identity;
using Abstractions.Messaging;
using SharedKernel;

internal sealed class GetUserByIdQueryHandler(IUserService userService)
    : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<Result<UserDto>> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user = await userService.GetUserByIdAsync(request.UserId);

        return user;
    }
}
