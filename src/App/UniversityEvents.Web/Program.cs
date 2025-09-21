using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UniversityEvents.Application;
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
    .Enrich.With<TraceIdEnricher>()   // Custom enricher adds TraceId
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger();

builder.Host.UseSerilog();

// =======================
// 2️⃣ Add Infrastructure & Application Services
// =======================
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);


// =======================
// 3️⃣ Add MVC Controllers
// =======================
builder.Services.AddControllersWithViews();

// =======================
// 4️⃣ OpenTelemetry Tracing
// =======================
builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
    {
        tracerProviderBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddConsoleExporter();
    });

// =======================
// 5️⃣ OpenTelemetry Metrics (Prometheus)
// =======================
builder.Services.AddOpenTelemetry()
    .WithMetrics(metricsBuilder =>
    {
        metricsBuilder
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("UniversityEventsMVC"))
            .AddAspNetCoreInstrumentation()
            .AddHttpClientInstrumentation()
            .AddPrometheusExporter(); // Expose /metrics endpoint
    });

// =======================
// 6️⃣ Mapster Mappings
// =======================
MapsterConfig.RegisterMappings();
builder.Services.AddDistributedMemoryCache();

// ✅ Session Services
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =======================
// 7️⃣ Authentication / Authorization
// =======================
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();

// =======================
// 8️⃣ Build app
// =======================
var app = builder.Build();

// =======================
// 9️⃣ Middleware Pipeline
// =======================

// Exception handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Session first
app.UseSession();

// Authentication must come before Authorization and any middleware using user info
app.UseAuthentication();
app.UseAuthorization();

// Middlewares that rely on scoped services (SignInHelper, etc.)
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseMiddleware<BlockDirectLoginMiddleware>();
app.UseMiddleware<RouteLoggingMiddleware>();

// Static assets & Prometheus
app.MapStaticAssets();
app.MapPrometheusScrapingEndpoint("/metrics");

// Default controller route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
