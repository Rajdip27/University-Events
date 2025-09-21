using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityEvents.Application.Caching;
using UniversityEvents.Application.Imports;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.Repositories.Auth;
using static UniversityEvents.Application.Repositories.ICategoryRepository;

namespace UniversityEvents.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = configuration["Authentication:Google:ClientId"];
                options.ClientSecret = configuration["Authentication:Google:ClientSecret"];
            })
            .AddFacebook(options =>
            {
                options.AppId = configuration["Authentication:Facebook:AppId"];
                options.AppSecret = configuration["Authentication:Facebook:AppSecret"];
            });
    }
}
