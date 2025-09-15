using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using UniversityEvents.Application;
using UniversityEvents.Application.Mappings;
using UniversityEvents.Infrastructure;
using UniversityEvents.Web.Logging;

var builder = WebApplication.CreateBuilder(args);

// =======================
// 1️⃣ Configure Serilog
// =======================
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .Enrich.With<TraceIdEnricher>()   // Custom enricher adds TraceId
    .WriteTo.Console()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Seq("http://localhost:5341") // Replace with your Seq URL
    .CreateLogger();

builder.Host.UseSerilog();

// =======================
// 2️⃣ Add Infrastructure & Application Services
// =======================
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);

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

var app = builder.Build();

// =======================
// 7️⃣ Middleware Pipeline
// =======================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // ✅ Must come before Authorization
app.UseAuthorization();

app.MapStaticAssets();
app.MapPrometheusScrapingEndpoint("/metrics");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
