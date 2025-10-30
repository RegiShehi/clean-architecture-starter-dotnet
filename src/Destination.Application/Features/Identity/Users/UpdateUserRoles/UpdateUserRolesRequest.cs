namespace Destination.Application.Features.Identity.Users.UpdateUserRoles;

public class UpdateUserRolesRequest
{
    private readonly List<UserRoleDto> _userRoles = [];

    public IReadOnlyCollection<UserRoleDto> UserRoles => _userRoles.AsReadOnly();
}
