using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UniversityDbContext>((s, builder) =>
        {
            //builder.UseSqlServer(configuration[ApplicationConstants.DefaultConnection]);
            builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection")).ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            builder.UseLoggerFactory(s.GetRequiredService<ILoggerFactory>());
            builder.LogTo(Console.WriteLine, LogLevel.Debug);
        }, ServiceLifetime.Scoped);
        return services;
    }
}
