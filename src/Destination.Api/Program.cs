using Destination.Api;
using Destination.Api.Extensions;
using Destination.Application;
using Destination.Infrastructure;
using Destination.Infrastructure.Database.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApplication()
    .AddPresentation()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();

    await app.Services.ApplyMigrationsAsync();
    await app.Services.SeedDataAsync();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseInfrastructure();

await app.RunAsync();
