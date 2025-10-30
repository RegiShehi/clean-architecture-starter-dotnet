namespace Destination.Infrastructure;

using Application.Abstractions.Persistence;
using Application.Abstractions.Time;
using Common.Time;
using Database;
using Domain.Features.Apartments;
using Domain.Features.Bookings;
using DomainEvents;
using Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddInfrastructureServices(configuration)
            .AddIdentityServices()
            .AddPermissions()
            .AddJwtAuthentication();

    private static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        services.AddTransient<IDomainEventsDispatcher, DomainEventsDispatcher>();

        var connectionString = configuration.GetConnectionString("Database") ??
                               throw new ArgumentNullException(nameof(configuration));

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString).UseSnakeCaseNamingConvention());

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IApartmentRepository, ApartmentRepository>();

        return services;
    }

    public static IApplicationBuilder UseInfrastructure(this IApplicationBuilder app) =>
        app
            .UseCurrentUser();
}
