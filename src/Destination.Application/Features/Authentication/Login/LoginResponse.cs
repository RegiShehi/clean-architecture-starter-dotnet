namespace Destination.Application.Features.Authentication.Login;

public class LoginResponse
{
    public required string JwtToken { get; set; }

    public required string RefreshToken { get; set; }

    public DateTime RefreshTokenExpiryDate { get; set; }
}
