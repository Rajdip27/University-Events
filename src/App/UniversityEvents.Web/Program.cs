using Microsoft.AspNetCore.Http;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UniversityEvents.Application;
using UniversityEvents.Application.Extensions;
using UniversityEvents.Application.Mappings;
using UniversityEvents.Infrastructure;
using UniversityEvents.Web.Logging;
using UniversityEvents.Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1️⃣ Configure Serilog
// =======================
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<TraceIdEnricher>()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// =======================
// 2️⃣ Add Services
// =======================
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// MVC Controllers
builder.Services.AddControllersWithViews();

// =======================
// 3️⃣ Add OpenTelemetry
// =======================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(
                OpenTelemetry.Resources.ResourceBuilder
                    .CreateDefault()
                    .AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .SetResourceBuilder(
                OpenTelemetry.Resources.ResourceBuilder
                    .CreateDefault()
                    .AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
    });

// Mapster
MapsterConfig.RegisterMappings();

// ✅ Distributed Cache + Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ HttpContextAccessor for extension
builder.Services.AddHttpContextAccessor();

// Authentication / Authorization
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

var app = builder.Build();

// =======================
// 4️⃣ Middleware pipeline
// =======================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// ✅ Use session BEFORE accessing session in extension
app.UseSession();

// Configure the DateTimeExtensions with IHttpContextAccessor
DateTimeExtensions.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseAuthentication();
app.UseAuthorization();

// Custom middlewares
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<BlockDirectLoginMiddleware>();
app.UseMiddleware<RouteLoggingMiddleware>();

// Prometheus metrics
app.MapStaticAssets();
app.MapPrometheusScrapingEndpoint("/metrics");

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
