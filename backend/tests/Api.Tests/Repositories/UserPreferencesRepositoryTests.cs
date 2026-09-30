using Api.Data;
using Api.Domain;
using Api.Repositories;
using Api.Tests.Utils;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Api.Tests.Repositories;

/// <summary>
/// Unit tests for the UserPreferencesRepository using an in-memory database.
/// Each test runs against an isolated database instance.
/// </summary>
public sealed class UserPreferencesRepositoryTests : IDisposable
{
    private readonly AppDbContext _dbContext;
    private readonly UserPreferencesRepository _repository;

    public UserPreferencesRepositoryTests()
    {
        var contextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(contextOptions);
        _repository = new UserPreferencesRepository(_dbContext);
    }

    public void Dispose() => _dbContext.Dispose();

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_WhenExists_ReturnsPreference()
    {
        // Arrange
        var seededPreference = TestDataBuilders.BuildUserPreference(userId: 42, theme: DisplayThemes.Dark);
        _dbContext.UserPreferences.Add(seededPreference);
        await _dbContext.SaveChangesAsync();

        // Act
        var retrievedPreference = await _repository.GetByUserIdAsync(42, CancellationToken.None);

        // Assert
        Assert.NotNull(retrievedPreference);
        Assert.Equal(42, retrievedPreference.UserId);
        Assert.Equal(DisplayThemes.Dark, retrievedPreference.Theme);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenNotExists_ReturnsNull()
    {
        // Arrange
        var unknownUserId = 999;

        // Act
        var retrievedPreference = await _repository.GetByUserIdAsync(unknownUserId, CancellationToken.None);

        // Assert
        Assert.Null(retrievedPreference);
    }

    #endregion

    #region UpsertAsync Tests

    [Fact]
    public async Task UpsertAsync_WhenNewPreference_AddsRecord()
    {
        // Arrange
        var newPreference = TestDataBuilders.BuildUserPreference(userId: 7);

        // Act
        var persistedPreference = await _repository.UpsertAsync(newPreference, CancellationToken.None);

        // Assert
        Assert.Equal(7, persistedPreference.UserId);
        var storedRecord = await _dbContext.UserPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(record => record.UserId == 7);
        Assert.NotNull(storedRecord);
        Assert.Equal(newPreference.Theme, storedRecord.Theme);
    }

    [Fact]
    public async Task UpsertAsync_WhenExistingPreference_UpdatesRecordAndPreservesCreatedDate()
    {
        // Arrange
        var originalCreatedDate = DateTime.UtcNow.AddDays(-5);
        var existingPreference = TestDataBuilders.BuildUserPreference(userId: 8, theme: DisplayThemes.Light);
        existingPreference.CreatedDate = originalCreatedDate;
        _dbContext.UserPreferences.Add(existingPreference);
        await _dbContext.SaveChangesAsync();

        var updatedPreference = TestDataBuilders.BuildUserPreference(
            userId: 8,
            theme: DisplayThemes.Dark,
            notificationsEnabled: false);
        updatedPreference.CreatedDate = originalCreatedDate;

        // Act
        var persistedPreference = await _repository.UpsertAsync(updatedPreference, CancellationToken.None);

        // Assert
        Assert.Equal(DisplayThemes.Dark, persistedPreference.Theme);
        Assert.False(persistedPreference.NotificationsEnabledIndicator);
        Assert.Equal(originalCreatedDate, persistedPreference.CreatedDate);
        Assert.Equal(1, await _dbContext.UserPreferences.CountAsync(record => record.UserId == 8));
    }

    #endregion
}
