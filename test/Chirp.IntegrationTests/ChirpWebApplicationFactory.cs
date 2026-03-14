using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Chirp.Core;
using Chirp.Repositories;

namespace Chirp.IntegrationTests;

/// Custom WebApplicationFactory that configures the app for testing.
/// This sets the environment to "testing" which triggers in-memory database usage 
/// default for WebApplicationFactory is "Development" - and if not changed, it will use the real DB
public class ChirpWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("testing");

        builder.ConfigureServices(services =>
        {
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<CheepDBContext>();
            context.Database.EnsureCreated();
            DbInitializer.SeedDatabase(context);
        });
    }
}