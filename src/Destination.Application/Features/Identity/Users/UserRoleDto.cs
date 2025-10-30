namespace Destination.Application.Features.Identity.Users;

public class UserRoleDto
{
    public required Guid RoleId { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public bool IsAssigned { get; set; }
}
