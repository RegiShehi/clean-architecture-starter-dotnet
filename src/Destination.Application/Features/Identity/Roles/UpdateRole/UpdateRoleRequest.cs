namespace Destination.Application.Features.Identity.Roles.UpdateRole;

public class UpdateRoleRequest
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
