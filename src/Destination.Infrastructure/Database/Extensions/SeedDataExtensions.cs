namespace Destination.Infrastructure.Database.Extensions;

using System.Globalization;
using Bogus;
using Domain.Common;
using Domain.Features.Apartments;
using Domain.Features.Apartments.ValueObjects;
using Identity.Constants;
using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class SeedDataExtensions
{
    public static async Task SeedDataAsync(this IServiceProvider services,
        CancellationToken cancellationToken = default)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

        logger.LogInformation("Starting data seeding...");

        await using var transaction = await context.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            await SeedDefaultRolesAsync(context, roleManager, cancellationToken);

            await SeedAdminUserAsync(userManager);

            await SeedEntitiesAsync(context, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);

            logger.LogError(ex, "Seeding failed.");

            throw new InvalidOperationException("An error occurred during data seeding.", ex);
        }

        logger.LogInformation("Seeding completed");
    }

    private static async Task SeedDefaultRolesAsync(ApplicationDbContext context,
        RoleManager<ApplicationRole> roleManager,
        CancellationToken cancellationToken)
    {
        foreach (var roleName in RoleConstants.DefaultRoles)
        {
            var incomingRole = await roleManager.FindByNameAsync(roleName);

            if (incomingRole is not null)
            {
                continue;
            }

            incomingRole = new ApplicationRole
            {
                Name = roleName,
                Description = $"{roleName} Role"
            };

            await roleManager.CreateAsync(incomingRole);

            // assign permissions to newly added role
            switch (roleName)
            {
                case RoleConstants.Basic:
                    await AssignPermissionsToRole(context, DestinationPermissions.Basic, incomingRole, roleManager,
                        cancellationToken);
                    break;
                case RoleConstants.Admin:
                    await AssignPermissionsToRole(context, DestinationPermissions.Admin, incomingRole, roleManager,
                        cancellationToken);
                    break;
            }
        }
    }

    private static async Task SeedEntitiesAsync(ApplicationDbContext context, CancellationToken cancellationToken)
    {
        var data = await context.Set<Apartment>().AnyAsync(cancellationToken);

        if (data)
        {
            return;
        }

        var faker = new Faker();

        var apartments = new List<Apartment>(100);

        for (var i = 0; i < 100; i++)
        {
            var id = Guid.NewGuid();

            var name = new Name(faker.Company.CompanyName());
            var description = new Description("Amazing view");

            var address = new Address(
                faker.Address.Country(),
                faker.Address.State(),
                faker.Address.ZipCode(),
                faker.Address.City(),
                faker.Address.StreetAddress()
            );

            var price = new Money(faker.Random.Decimal(50, 1000), Currency.Usd);
            var cleaningFee = new Money(faker.Random.Decimal(25, 200), Currency.Usd);

            var amenities = new List<Amenity> { Amenity.Parking, Amenity.MountainView };

            var apartment = new Apartment(
                id,
                name,
                description,
                address,
                price,
                cleaningFee,
                amenities
            );

            apartments.Add(apartment);
        }

        await context.Set<Apartment>().AddRangeAsync(apartments, cancellationToken);
    }

    private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
    {
        var adminUser = await userManager.Users.FirstOrDefaultAsync(x => x.Email == AdminUserConstants.Email);

        if (adminUser is null)
        {
            adminUser = new ApplicationUser
            {
                FirstName = AdminUserConstants.FirstName,
                LastName = AdminUserConstants.LastName,
                Email = AdminUserConstants.Email,
                UserName = AdminUserConstants.Email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true,
                NormalizedEmail = AdminUserConstants.Email.ToUpperInvariant(),
                NormalizedUserName = AdminUserConstants.Email.ToUpper(CultureInfo.CurrentCulture),
                IsActive = true
            };

            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AdminUserConstants.DefaultPassword);

            await userManager.CreateAsync(adminUser);
        }

        // assign user to admin role
        if (!await userManager.IsInRoleAsync(adminUser, RoleConstants.Admin))
        {
            await userManager.AddToRoleAsync(adminUser, RoleConstants.Admin);
        }
    }

    private static async Task AssignPermissionsToRole(ApplicationDbContext context,
        IReadOnlyCollection<DestinationPermissionDetails> permissions,
        ApplicationRole role,
        RoleManager<ApplicationRole> roleManager,
        CancellationToken cancellationToken)
    {
        var currentClaims = await roleManager.GetClaimsAsync(role);

        // Identify permissions that are not present in currentClaims
        var newClaims = permissions
            .Where(p => !currentClaims.Any(c => c.Type == ClaimConstants.Permission && c.Value == p.Name))
            .Select(p => new IdentityRoleClaim<Guid>
            {
                RoleId = role.Id,
                ClaimType = ClaimConstants.Permission,
                ClaimValue = p.Name
            })
            .ToList();

        if (newClaims.Count > 0)
        {
            await context.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
        }
    }
}
