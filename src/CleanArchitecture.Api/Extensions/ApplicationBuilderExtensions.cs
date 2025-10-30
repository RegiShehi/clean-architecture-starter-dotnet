namespace CleanArchitecture.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Clean Architecture API v1");
            c.RoutePrefix = "docs";
            c.DocumentTitle = "Clean Architecture API Docs";
        });

        return app;
    }
}
