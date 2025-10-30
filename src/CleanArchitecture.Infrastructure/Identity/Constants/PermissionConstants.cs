namespace CleanArchitecture.Infrastructure.Identity.Constants;

public static class DestinationAction
{
    public const string View = nameof(View);
    public const string Create = nameof(Create);
    public const string Update = nameof(Update);
    public const string Delete = nameof(Delete);
}

public static class DestinationFeature
{
    public const string Users = nameof(Users);
    public const string UserRoles = nameof(UserRoles);
    public const string Roles = nameof(Roles);
    public const string RoleClaims = nameof(RoleClaims);
    public const string Apartments = nameof(Apartments);
}

public record DestinationPermissionDetails(
    string Description,
    string Action,
    string Feature,
    bool IsBasic = false)
{
    public string Name => NameFor(Action, Feature);

    public static string NameFor(string action, string feature) => $"Permission.{feature}.{action}";
}

public static class DestinationPermissions
{
    private static readonly DestinationPermissionDetails[] AllPermissions =
    [
        new("View Users", DestinationAction.View, DestinationFeature.Users),
        new("Create Users", DestinationAction.Create, DestinationFeature.Users),
        new("Update Users", DestinationAction.Update, DestinationFeature.Users),
        new("Delete Users", DestinationAction.Delete, DestinationFeature.Users),

        new("View User Roles", DestinationAction.View, DestinationFeature.UserRoles),
        new("Update User Roles", DestinationAction.Update, DestinationFeature.UserRoles),

        new("View Roles", DestinationAction.View, DestinationFeature.Roles),
        new("Create Roles", DestinationAction.Create, DestinationFeature.Roles),
        new("Update Roles", DestinationAction.Update, DestinationFeature.Roles),

        new("Delete Roles", DestinationAction.Delete, DestinationFeature.Roles),

        new("View Role Claims/Permissions", DestinationAction.View, DestinationFeature.RoleClaims),
        new("Update Role Claims/Permissions", DestinationAction.Update, DestinationFeature.RoleClaims),

        new("View Apartments", DestinationAction.View, DestinationFeature.Apartments, true),
        new("Create Apartments", DestinationAction.Create, DestinationFeature.Apartments),
        new("Update Apartments", DestinationAction.Update, DestinationFeature.Apartments),
        new("Delete Apartments", DestinationAction.Delete, DestinationFeature.Apartments)
    ];

    // Return a read-only wrapper to prevent external modification
    public static IReadOnlyCollection<DestinationPermissionDetails> All { get; } = Array.AsReadOnly(AllPermissions);

    // Cache the filtered collections
    public static IReadOnlyCollection<DestinationPermissionDetails> Admin { get; } =
        Array.AsReadOnly(AllPermissions.Where(x => !x.IsBasic).ToArray());

    public static IReadOnlyCollection<DestinationPermissionDetails> Basic { get; } =
        Array.AsReadOnly(AllPermissions.Where(x => x.IsBasic).ToArray());
}
