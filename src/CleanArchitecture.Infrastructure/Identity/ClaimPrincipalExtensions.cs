namespace CleanArchitecture.Infrastructure.Identity;

using System.Security.Claims;

public static class ClaimPrincipalExtensions
{
    public static string GetEmail(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);

        return Guid.TryParse(userId, out var guid) ? guid : Guid.Empty;
    }

    public static string GetFirstName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public static string GetLastName(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty;

    public static string GetPhoneNumber(this ClaimsPrincipal principal) =>
        principal.FindFirstValue(ClaimTypes.MobilePhone) ?? string.Empty;
}
