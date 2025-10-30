namespace Destination.Application.Abstractions.Identity;

using System.Security.Claims;

public interface ICurrentUserService
{
    string? Name { get; }

    Guid GetUserId();

    string GetUserEmail();

    // string GetUserTenant();

    bool IsAuthenticated();

    bool IsInRole(string role);

    IEnumerable<Claim> GetUserClaims();

    void SetCurrentUser(ClaimsPrincipal? claimsPrincipal);
}
