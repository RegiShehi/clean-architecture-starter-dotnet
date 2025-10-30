namespace Destination.Application.Features.Identity.Roles;

public class RoleDto
{
    private readonly List<string> _permissions = [];

    public required string Name { get; set; }

    public string? Description { get; set; }

    public required Guid Id { get; set; }

    public IReadOnlyCollection<string> Permissions => _permissions.AsReadOnly();

    public void SetPermissions(IReadOnlyList<string> newPermissions)
    {
        _permissions.Clear();
        _permissions.AddRange(newPermissions);
    }
}
