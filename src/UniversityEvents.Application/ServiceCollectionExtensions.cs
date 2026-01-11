using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityEvents.Application.CommonModel;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.FileServices;
using UniversityEvents.Application.Imports;
using UniversityEvents.Application.Logging;
using UniversityEvents.Application.Repositories;
using UniversityEvents.Application.Repositories.Auth;
using UniversityEvents.Application.Services;
using UniversityEvents.Application.Services.Pdf;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Application.SSLCommerz.Services;
using UniversityEvents.Application.ViewModel.Utilities;

namespace UniversityEvents.Application;

public static class ServiceCollectionExtensions
{
    public static void AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IEventRepository, EventRepository>();
        services.AddTransient<IStudentRegistrationRepository, StudentRegistrationRepository>();
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<IExternalAuthService, ExternalAuthService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IPaymentHistoryRepository, PaymentHistoryRepository>();
        services.AddHttpClient(); // register the factory
        services.AddTransient<ISSLCommerzService, SSLCommerzService>();
        services.AddScoped<IDashboardRepository, DashboardRepository>();


        services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
        // Register your PDF service
        services.AddScoped<IPdfService, PdfService>();
        // Register Razor view renderer
        services.AddScoped<IRazorViewToStringRenderer, RazorViewToStringRenderer>();

        //services.AddScoped<IRedisCacheHelper, RedisCacheHelper>();
        //// 🔹 Redis registration (✅ fixed)
        //services.AddSingleton<IConnectionMultiplexer>(sp =>
        //{
        //    var redisConnection = configuration.GetConnectionString("Redis");
        //    if (string.IsNullOrWhiteSpace(redisConnection))
        //        throw new InvalidOperationException("Redis connection string is missing in configuration.");
        //    return ConnectionMultiplexer.Connect(redisConnection);
        //});
        //services.AddScoped<IRedisCacheService, RedisCacheService>();
        services.AddScoped<IExcelImportService, ExcelImportService>();
        services.AddScoped<IEmailService, EmailService>();
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
        services.Configure<SMTPSettings>(configuration.GetSection("Email"));
        services.Configure<WhatsAppSettings>(
    configuration.GetSection("WhatsApp")
);


        services.AddDinkToPdf();

    }
}
