using Microsoft.AspNetCore.Routing;
using UniversityEvents.Core.Entities.EntityLogs;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;

namespace UniversityEvents.Web.Middlewares;

public class RouteLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RouteLoggingMiddleware> _logger;
    private readonly IServiceProvider _serviceProvider;
    private readonly ISignInHelper _signInHelper;

    public RouteLoggingMiddleware(RequestDelegate next, ILogger<RouteLoggingMiddleware> logger, IServiceProvider serviceProvider, ISignInHelper signInHelper)
    {
        _next = next;
        _logger = logger;
        _serviceProvider = serviceProvider;
        _signInHelper = signInHelper;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            var endpoint = context.GetEndpoint() as RouteEndpoint;
            if (endpoint != null)
            {
                var routeData = context.GetRouteData();

                var log = new RouteLog
                {
                    Area = routeData.Values["area"]?.ToString(),
                    ControllerName = routeData.Values["controller"]?.ToString(),
                    ActionName = routeData.Values["action"]?.ToString(),
                    RoleId = _signInHelper.Roles.Any()
                            ? string.Join(",", _signInHelper.Roles)
                            : "Anonymous",
                    UserId = _signInHelper.UserId?.ToString(),
                    IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                    UrlReferrer = context.Request.Headers["Referer"].ToString(),
                    PageAccessed = context.Request.Path,
                    SessionId = context.Session?.Id,
                    LoginStatus = _signInHelper.IsAuthenticated ? "LoggedIn" : "Guest",
                    LoggedInDateTimeUtc = DateTime.UtcNow.ToString("u")
                };

                // Fire-and-forget DB save using scoped DbContext
                _ = Task.Run(async () =>
                {
                    try
                    {
                        using var scope = _serviceProvider.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<UniversityDbContext>();
                        db.RouteLogs.Add(log);
                        await db.SaveChangesAsync();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error saving RouteLog in background");
                    }
                });

                _logger.LogInformation("Route logged: {Controller}/{Action} by {User}",
                    log.ControllerName, log.ActionName, log.UserId ?? "Guest");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RouteLoggingMiddleware error");
        }

        await _next(context);
    }
}
