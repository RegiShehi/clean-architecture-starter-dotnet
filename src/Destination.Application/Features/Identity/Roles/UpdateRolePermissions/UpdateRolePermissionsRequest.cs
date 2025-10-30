namespace Destination.Application.Features.Identity.Roles.UpdateRolePermissions;

public class UpdateRolePermissionsRequest
{
    private readonly List<string> _permissions = [];

    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();
}
