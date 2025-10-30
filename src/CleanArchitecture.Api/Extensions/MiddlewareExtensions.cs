namespace CleanArchitecture.Api.Extensions;

public static class MiddlewareExtensions
{
    // TODO: Add RequestContextLoggingMiddleware once Serilog has been added
    public static IApplicationBuilder UseRequestContextLogging(this IApplicationBuilder app) => app;
}
