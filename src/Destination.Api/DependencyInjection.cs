namespace Destination.Api;

using System.Text.Json.Serialization;
using Application.Abstractions.Messaging;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

public static class DependencyInjection
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddScoped<ISender, Sender>();
        services.AddEndpointsApiExplorer();
        services.AddControllers()
            .AddMvcOptions(o => o.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
            .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        services.Configure<ApiBehaviorOptions>(o => o.SuppressModelStateInvalidFilter = true);

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Destination API",
                Version = "v1",
                Description = "API documentation for Destination service."
            });

            // Group related models together
            c.CustomSchemaIds(type =>
            {
                var ns = type.Namespace?.Split('.').LastOrDefault();
                return string.IsNullOrWhiteSpace(ns)
                    ? type.Name
                    : $"{ns}.{type.Name}";
            });

            var jwtSecurityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "Enter your JWT bearer token to access this API",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            };

            c.AddSecurityDefinition("Bearer", jwtSecurityScheme);

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    jwtSecurityScheme,
                    Array.Empty<string>()
                }
            });
        });

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        return services;
    }
}
