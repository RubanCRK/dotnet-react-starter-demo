using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>
/// Entity Framework Core database context for the application.
/// Currently backed by the InMemory provider; the model is kept
/// migration-ready for a future relational provider.
/// </summary>
public sealed class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the application database context.
    /// </summary>
    /// <param name="options">The options used to configure the context.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// Persisted user preference records, one per user.
    /// </summary>
    public DbSet<UserPreference> UserPreferences => Set<UserPreference>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserPreference>(entityBuilder =>
        {
            entityBuilder.HasKey(preference => preference.UserId);
            entityBuilder.Property(preference => preference.Theme)
                .HasMaxLength(10)
                .IsRequired();
        });
    }
}
