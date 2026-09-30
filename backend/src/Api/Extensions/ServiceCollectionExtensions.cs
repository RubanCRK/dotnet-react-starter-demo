using Api.Data;
using Api.Repositories;
using Api.Services;
using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

/// <summary>
/// Extension methods for configuring application services in the DI container.
/// Centralizes service registration following organization standards.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers all application services, repositories, and dependencies.
    /// Call this method in Program.cs to configure the service container.
    /// </summary>
    /// <param name="services">The service collection to configure.</param>
    /// <returns>The service collection for method chaining.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register the database context (InMemory until a relational provider is adopted)
        services.AddDbContext<AppDbContext>(dbContextOptions =>
            dbContextOptions.UseInMemoryDatabase("AppDatabase"));

        // Register repositories here using AddScoped
        services.AddScoped<IUserPreferencesRepository, UserPreferencesRepository>();

        // Register services here using AddScoped
        services.AddScoped<IUserPreferencesService, UserPreferencesService>();

        return services;
    }
}
