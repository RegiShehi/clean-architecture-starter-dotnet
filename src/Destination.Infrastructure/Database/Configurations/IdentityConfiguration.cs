namespace Destination.Infrastructure.Database.Configurations;

using Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal sealed class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder
            .ToTable("Users", SchemaNames.Identity);

        builder.Property(r => r.FirstName).HasMaxLength(100);
        builder.Property(r => r.LastName).HasMaxLength(100);
        builder.Property(r => r.RefreshToken).HasMaxLength(2048);
    }
}

internal sealed class ApplicationRoleConfig : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        builder
            .ToTable("Roles", SchemaNames.Identity);

        builder.Property(r => r.Description).HasMaxLength(200);
    }
}

internal sealed class ApplicationUserClaimConfig : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder) =>
        builder
            .ToTable("RoleClaims", SchemaNames.Identity);
}

internal sealed class IdentityUserRoleConfig : IEntityTypeConfiguration<IdentityUserRole<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserRole<Guid>> builder) =>
        builder
            .ToTable("UserRoles", SchemaNames.Identity);
}

internal sealed class IdentityUserClaimConfig : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder) =>
        builder
            .ToTable("UserClaims", SchemaNames.Identity);
}

internal sealed class IdentityUserTokenConfig : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder) =>
        builder
            .ToTable("UserTokens", SchemaNames.Identity);
}

internal sealed class IdentityUserLoginConfig : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder) =>
        builder
            .ToTable("UserLogins", SchemaNames.Identity);
}
