using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UniversityEvents.Infrastructure.Data;
using UniversityEvents.Infrastructure.Healper.Acls;
using static UniversityEvents.Core.Entities.Auth.IdentityModel;

namespace UniversityEvents.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Ensure logging services exist
        services.AddLogging();

        // Register DbContext
        services.AddDbContext<UniversityDbContext>((sp, builder) =>
        {
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                   .ConfigureWarnings(warnings =>
                       warnings.Ignore(RelationalEventId.PendingModelChangesWarning));

            var loggerFactory = sp.GetRequiredService<ILoggerFactory>();
            builder.UseLoggerFactory(loggerFactory);

            // Log SQL queries to console (Info+ only)
            builder.LogTo(Console.WriteLine, LogLevel.Information);
        });

        // Register Identity
        services.AddIdentity<User, Role>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = false;
        })
        .AddEntityFrameworkStores<UniversityDbContext>()
        .AddDefaultTokenProviders();

        // Register helpers
        services.AddTransient<ISignInHelper, SignInHelper>();

        // Needed for accessing HttpContext in services (e.g., for logging RouteLogs)
        services.AddHttpContextAccessor();

        // Example authorization policy
        services.AddAuthorizationCore(options =>
        {
            options.AddPolicy("CanPurge", policy => policy.RequireRole("Administrator"));
        });

        return services;
    }
}
