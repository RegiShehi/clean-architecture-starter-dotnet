namespace CleanArchitecture.Domain.Features.Authentication;

using SharedKernel;

public static class AuthErrors
{
    public static readonly Error InvalidCredentials = Error.Unauthorized(
        "Auth.InvalidCredentials", "Authentication failed.");

    public static readonly Error InvalidToken = Error.Unauthorized(
        "Auth.InvalidToken", "Invalid token.");

    public static readonly Error AuthenticationFailed = Error.Unauthorized(
        "Auth.Failed", "Authentication failed.");

    public static readonly Error InactiveUser = Error.Forbidden(
        "Auth.InactiveUser", "User is not active. Please contact admin.");

    public static readonly Error PersistenceFailed = Error.Problem(
        "Auth.PersistenceFailed", "Failed to persist refresh token.");
}
