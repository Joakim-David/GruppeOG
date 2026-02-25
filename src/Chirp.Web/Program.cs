using Microsoft.EntityFrameworkCore;
using Chirp.Repositories;
using Chirp.Services;
using Chirp.Core;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;

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
    // Use persistent SQLite database for non-testing environments
    string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<CheepDBContext>(options =>
        options.UseSqlite(connectionString, b => b.MigrationsAssembly("Chirp.Web")));
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
    options.SignIn.RequireConfirmedAccount = false;
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
        var dbPath = "/app/data/chirp.db";
        if (!File.Exists(dbPath))
        {
            context.Database.EnsureCreated(); 
            DbInitializer.SeedDatabase(context);
        }
        else
        {
            context.Database.Migrate(); 
        }
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

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowChirp");
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
