using UniversityEvents.Core.Entities.EntityLogs;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Web.Middlewares;

public class RouteLoggingMiddleware(RequestDelegate next, ILogger<RouteLoggingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<RouteLoggingMiddleware> _logger = logger;
    public async Task InvokeAsync(HttpContext context, UniversityDbContext db)
    {
        try
        {
            var routeData = context.GetRouteData();
            var log = new RouteLog
            {
                Area = routeData.Values["area"]?.ToString(),
                ControllerName = routeData.Values["controller"]?.ToString(),
                ActionName = routeData.Values["action"]?.ToString(),
                RoleId = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst("role")?.Value
                    : "Anonymous",
                UserId = context.User?.Identity?.IsAuthenticated == true
                    ? context.User.FindFirst("sub")?.Value
                    : null,
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UrlReferrer = context.Request.Headers["Referer"].ToString(),
                PageAccessed = context.Request.Path,
                SessionId = context.Session.Id, // Requires Session enabled
                LoginStatus = context.User?.Identity?.IsAuthenticated == true ? "LoggedIn" : "Guest",
                LoggedInDateTimeUtc = DateTime.UtcNow.ToString("u"),
            };

            db.RouteLogs.Add(log);
            await db.SaveChangesAsync();

            _logger.LogInformation("Route logged: {Controller}/{Action} by {User}",
                log.ControllerName, log.ActionName, log.UserId ?? "Guest");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving RouteLog");
        }

        await _next(context);
    }
}
