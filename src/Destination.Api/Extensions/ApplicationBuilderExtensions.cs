namespace Destination.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Destination API v1");
            c.RoutePrefix = "docs";
            c.DocumentTitle = "Destination API Docs";
        });

        return app;
    }
}
