﻿namespace Destination.Infrastructure.Identity.Constants;

using System.Collections.ObjectModel;

public static class RoleConstants
{
    public const string Admin = nameof(Admin);
    public const string Basic = nameof(Basic);

    public static IReadOnlyList<string> DefaultRoles { get; } = new ReadOnlyCollection<string>([Admin, Basic]);

    public static bool IsDefault(string roleName) => DefaultRoles.Contains(roleName);
}
