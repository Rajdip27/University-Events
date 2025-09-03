using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using UniversityEvents.Infrastructure.Data;

namespace UniversityEvents.Infrastructure.DesignTime;

public class UniversityDbContextFactory: IDesignTimeDbContextFactory<UniversityDbContext>
{
    public UniversityDbContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<UniversityDbContext>();
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

        // Create context without AuditHelper (design-time)
        return new UniversityDbContext(optionsBuilder.Options);
    }
}
