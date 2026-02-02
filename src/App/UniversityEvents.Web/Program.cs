using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UniversityEvents.Application;
using UniversityEvents.Application.Mappings;
using UniversityEvents.Application.SSLCommerz.Models;
using UniversityEvents.Infrastructure;
using UniversityEvents.Web.Logging;
using UniversityEvents.Web.Middlewares;

// --------------------
// 1️⃣ Create builder with options
// --------------------
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    // Lock environment from the start
    EnvironmentName = Environments.Development
});

// --------------------
// 2️⃣ Configure Serilog
// --------------------
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<TraceIdEnricher>()
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog(); // ✅ OK

// --------------------
// 3️⃣ Configure app settings
// --------------------
builder.Services.Configure<SSLCommerzSettings>(
    builder.Configuration.GetSection("SSLCommerz"));

// --------------------
// 4️⃣ Add services
// --------------------
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

// MVC
builder.Services.AddControllersWithViews();

// --------------------
// 5️⃣ Add OpenTelemetry Tracing & Metrics
// --------------------
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault().AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter();
    });

// Mapster
MapsterConfig.RegisterMappings();

// Distributed cache + session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// HttpContextAccessor
builder.Services.AddHttpContextAccessor();

// Auth
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// --------------------
// 6️⃣ Build app
// --------------------
var app = builder.Build();

// --------------------
// 7️⃣ Middleware pipeline
// --------------------
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    app.UseMiddleware<ErrorHandlingMiddleware>(); // prod only
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // session must come before accessing it

// Configure DateTimeExtensions
DateTimeExtensions.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());

app.UseAuthentication();
app.UseAuthorization();

// Custom middlewares
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

// --------------------
// 8️⃣ Run app
// --------------------
app.Run();
