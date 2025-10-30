namespace CleanArchitecture.Application.Features.Authentication.RefreshToken;

public class RefreshTokenRequest
{
    public required string JwtToken { get; set; }

    public required string RefreshToken { get; set; }
}
