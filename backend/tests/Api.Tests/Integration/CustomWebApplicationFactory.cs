using Api.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Api.Tests.Integration;

/// <summary>
/// Custom web application factory for integration testing.
/// Configures the test server with testing environment settings and
/// replaces the runtime database with an isolated in-memory database.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    // Generated once per factory instance: the options lambda runs per scope,
    // so computing the name inside it would give every request a fresh empty database.
    private readonly string _isolatedDatabaseName = $"TestDb_{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseEnvironment("Testing");

        webHostBuilder.ConfigureServices(serviceCollection =>
        {
            // Replace the runtime DbContext with an isolated InMemory provider
            var existingDbDescriptor = serviceCollection.SingleOrDefault(
                descriptor => descriptor.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (existingDbDescriptor != null)
            {
                serviceCollection.Remove(existingDbDescriptor);
            }

            serviceCollection.AddDbContext<AppDbContext>(dbOptions =>
                dbOptions.UseInMemoryDatabase(_isolatedDatabaseName));
        });
    }
}
