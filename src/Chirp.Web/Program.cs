using Microsoft.EntityFrameworkCore;
using Chirp.Repositories;
using Chirp.Services;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);

/// <summary>
/// Application startup and configuration file.
/// </summary>
/// <remarks>
/// This file configures dependency injection, database access,
/// authentication, authorization, middleware, and environment-specific
/// behavior for the Chirp web application.
/// </remarks>

// -----------------------------------------------------------------------------
// Database configuration
// -----------------------------------------------------------------------------

// Use an in-memory SQLite database when running in the testing environment
if (builder.Environment.IsEnvironment("testing"))
{
    var connection = new SqliteConnection("Data Source=:memory:");
    connection.Open();

    builder.Services.AddSingleton(connection);

    builder.Services.AddDbContext<CheepDBContext>((sp, options) =>
    {
        var conn = sp.GetRequiredService<SqliteConnection>();
        options.UseSqlite(conn);
    });
}
else
{
    // Use PostgreSQL for development and production
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<CheepDBContext>(options =>
        options.UseNpgsql(connectionString, b => b.MigrationsAssembly("Chirp.Web")));
}

// Adds detailed database exception pages during development
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// -----------------------------------------------------------------------------
// Identity and authentication configuration
// -----------------------------------------------------------------------------

/// <summary>
/// Configure ASP.NET Core Identity using Author as the user entity.
/// </summary>
builder.Services.AddDefaultIdentity<Author>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    options.User.AllowedUserNameCharacters += " ";

    // Add these lines to relax password requirements
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
})
.AddEntityFrameworkStores<CheepDBContext>();

// -----------------------------------------------------------------------------
// Dependency injection (Repositories & Services)
// -----------------------------------------------------------------------------

// Razor Pages support
builder.Services.AddRazorPages();
builder.Services.AddControllers();

// Repository layer (data access)
builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();

// Service layer (business logic)
builder.Services.AddScoped<ICheepService, CheepService>();
builder.Services.AddScoped<IAuthorService, AuthorService>();

// -----------------------------------------------------------------------------
// CORS configuration
// -----------------------------------------------------------------------------

/// <summary>
/// Configure CORS policy for the application.
/// </summary>
/// <remarks>
/// Allowed origins are read from configuration.
/// Credentials are enabled to support OAuth authentication.
/// </remarks>
/*var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>()!;

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowChirp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
*/
// -----------------------------------------------------------------------------
// Cookie and session configuration
// -----------------------------------------------------------------------------

/// <summary>
/// Configure cookie behavior to protect against CSRF attacks.
/// </summary>
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.SameSite = SameSiteMode.Strict;
});

// -----------------------------------------------------------------------------
// Authentication providers
// -----------------------------------------------------------------------------

// Use simple cookie authentication in testing environment
builder.Services.AddAuthentication().AddCookie();

// -----------------------------------------------------------------------------
// Build application
// -----------------------------------------------------------------------------

var app = builder.Build();

var requestLogger = app.Services.GetRequiredService<ILoggerFactory>()
    .CreateLogger("Chirp.Web.Request");

var cpuGauge = Metrics.CreateGauge("minitwit_cpu_load_percent", "Current load of the CPU in percent.");
var memoryGauge = Metrics.CreateGauge("minitwit_memory_working_set_mb", "Process working set memory in MB.");
var responseCounter = Metrics.CreateCounter("minitwit_http_responses_total", "The count of HTTP responses sent.");
var reqDurationSummary = Metrics.CreateHistogram(
    "minitwit_request_duration_milliseconds",
    "Request duration distribution in milliseconds.",
    new HistogramConfiguration
    {
        Buckets = new[] { 1.0, 5, 10, 25, 50, 100, 250, 500, 1000, 2500, 5000, 10000 }
    });

var cpuSampleLock = new object();
var lastCpuSampleAt = DateTime.UtcNow;
var lastCpuTime = System.Diagnostics.Process.GetCurrentProcess().TotalProcessorTime;

// -----------------------------------------------------------------------------
// Database initialization and seeding
// -----------------------------------------------------------------------------

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<CheepDBContext>();

    if (app.Environment.IsEnvironment("testing"))
    {
        context.Database.EnsureCreated();
    }
    else
    {
        // Apply migrations
        context.Database.Migrate();
    }
}
// -----------------------------------------------------------------------------
// HTTP request pipeline configuration
// -----------------------------------------------------------------------------

if (app.Environment.IsProduction())
{
    // Production error handling
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// Test-only endpoint for resetting the database
if (app.Environment.IsEnvironment("testing"))
{
    app.MapGet("/reset-test-db", async (CheepDBContext db) =>
    {
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
        DbInitializer.SeedDatabase(db);
        return Results.Ok("reset");
    });
}

// -----------------------------------------------------------------------------
// Middleware
// -----------------------------------------------------------------------------

// Den er er gemini den her til at teste om lortet virker ):
app.UseMetricServer();

app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();
    try
    {
        await next(context);
    }
    finally
    {
        watch.Stop();
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var status = context.Response.StatusCode;
        var ms = watch.Elapsed.TotalMilliseconds;
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        if (method != "GET" || status >= 400 || ms > 1000)
        {
            requestLogger.LogInformation(
                "REQUEST {Method} {Path} {StatusCode} {DurationMs:F4}ms from {RemoteIp}",
                method, path, status, ms, ip);
        }
    }
});

app.UseStaticFiles();
app.UseRouting();

// Track default HTTP metrics
app.UseHttpMetrics();

// Custom metrics tracker
app.Use(async (context, next) =>
{
    var watch = System.Diagnostics.Stopwatch.StartNew();

    lock (cpuSampleLock)
    {
        var now = DateTime.UtcNow;
        var wallMs = (now - lastCpuSampleAt).TotalMilliseconds;
        if (wallMs >= 1000)
        {
            var process = System.Diagnostics.Process.GetCurrentProcess();
            var currentCpu = process.TotalProcessorTime;
            var cpuMs = (currentCpu - lastCpuTime).TotalMilliseconds;
            var percent = cpuMs / (wallMs * Environment.ProcessorCount) * 100.0;
            cpuGauge.Set(Math.Clamp(percent, 0.0, 100.0));
            memoryGauge.Set(process.WorkingSet64 / 1024.0 / 1024.0);
            lastCpuTime = currentCpu;
            lastCpuSampleAt = now;
        }
    }

    try
    {
        await next(context);
    }
    finally
    {
        watch.Stop();
        responseCounter.Inc();
        reqDurationSummary.Observe(watch.Elapsed.TotalMilliseconds);
    }
});

// app.UseCors("AllowChirp");
app.UseAuthentication();
app.UseAuthorization();

// Razor Pages routing
app.MapControllers();
app.MapRazorPages();

// Start the application
app.Run();

// -----------------------------------------------------------------------------
// Program class exposed for integration testing
// -----------------------------------------------------------------------------

/// <summary>
/// Partial Program class used to enable integration testing.
/// </summary>
public partial class Program { }