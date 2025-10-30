namespace Destination.Application.Features.Identity.Users.UpdateUser;

public class UpdateUserRequest
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }

    public string? PhoneNumber { get; set; }
}
