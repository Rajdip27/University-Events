using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Services;

namespace UniversityEvents.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        
        services.AddDbContext<UniversityDbContext>((s, builder) =>
        {
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            builder.UseLoggerFactory(s.GetRequiredService<ILoggerFactory>());
            builder.LogTo(Console.WriteLine, LogLevel.Debug);
        }, ServiceLifetime.Scoped);
       services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
       services.AddScoped<AuditHelper>();

        return services;
    }
}
