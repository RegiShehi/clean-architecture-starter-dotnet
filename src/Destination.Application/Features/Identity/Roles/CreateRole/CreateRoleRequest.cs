namespace Destination.Application.Features.Identity.Roles.CreateRole;

public class CreateRoleRequest
{
    public required string Name { get; set; }

    public string? Description { get; set; }
}
