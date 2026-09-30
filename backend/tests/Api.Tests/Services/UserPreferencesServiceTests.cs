using Api.Domain;
using Api.DTOs.Preferences;
using Api.Repositories;
using Api.Services;
using Api.Tests.Utils;
using Moq;
using Xunit;

namespace Api.Tests.Services;

/// <summary>
/// Unit tests for the UserPreferencesService business logic.
/// Repository interactions are mocked with Moq.
/// </summary>
public sealed class UserPreferencesServiceTests
{
    private readonly Mock<IUserPreferencesRepository> _mockRepository;
    private readonly UserPreferencesService _service;

    public UserPreferencesServiceTests()
    {
        _mockRepository = new Mock<IUserPreferencesRepository>();
        _service = new UserPreferencesService(_mockRepository.Object);
    }

    #region GetByUserIdAsync Tests

    [Fact]
    public async Task GetByUserIdAsync_WhenPreferenceExists_ReturnsMappedDto()
    {
        // Arrange
        var persistedPreference = TestDataBuilders.BuildUserPreference(
            userId: 5,
            theme: DisplayThemes.Dark,
            notificationsEnabled: false);
        _mockRepository
            .Setup(repository => repository.GetByUserIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(persistedPreference);

        // Act
        var resultDto = await _service.GetByUserIdAsync(5, CancellationToken.None);

        // Assert
        Assert.Equal(5, resultDto.UserId);
        Assert.Equal(DisplayThemes.Dark, resultDto.Theme);
        Assert.False(resultDto.NotificationsEnabledIndicator);
        Assert.Equal(persistedPreference.CreatedDate, resultDto.CreatedDate);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenNoPreference_ReturnsDefaults()
    {
        // Arrange
        _mockRepository
            .Setup(repository => repository.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference?)null);

        // Act
        var resultDto = await _service.GetByUserIdAsync(123, CancellationToken.None);

        // Assert
        Assert.Equal(123, resultDto.UserId);
        Assert.Equal(UserPreference.DefaultTheme, resultDto.Theme);
        Assert.True(resultDto.NotificationsEnabledIndicator);
    }

    #endregion

    #region SaveAsync Tests

    [Fact]
    public async Task SaveAsync_WhenNewPreference_SetsCreatedAndUpdatedDates()
    {
        // Arrange
        var saveRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = DisplayThemes.Dark,
            NotificationsEnabledIndicator = false
        };
        _mockRepository
            .Setup(repository => repository.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference?)null);
        _mockRepository
            .Setup(repository => repository.UpsertAsync(It.IsAny<UserPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference preference, CancellationToken _) => preference);

        // Act
        var resultDto = await _service.SaveAsync(9, saveRequest, CancellationToken.None);

        // Assert
        Assert.Equal(9, resultDto.UserId);
        Assert.Equal(DisplayThemes.Dark, resultDto.Theme);
        Assert.False(resultDto.NotificationsEnabledIndicator);
        Assert.True(resultDto.CreatedDate <= resultDto.UpdatedDate);
    }

    [Fact]
    public async Task SaveAsync_WhenExistingPreference_PreservesOriginalCreatedDate()
    {
        // Arrange
        var originalCreatedDate = DateTime.UtcNow.AddDays(-10);
        var existingPreference = TestDataBuilders.BuildUserPreference(userId: 11);
        existingPreference.CreatedDate = originalCreatedDate;

        _mockRepository
            .Setup(repository => repository.GetByUserIdAsync(11, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingPreference);
        _mockRepository
            .Setup(repository => repository.UpsertAsync(It.IsAny<UserPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference preference, CancellationToken _) => preference);

        var saveRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = DisplayThemes.Light,
            NotificationsEnabledIndicator = true
        };

        // Act
        var resultDto = await _service.SaveAsync(11, saveRequest, CancellationToken.None);

        // Assert
        Assert.Equal(originalCreatedDate, resultDto.CreatedDate);
        Assert.True(resultDto.UpdatedDate > originalCreatedDate);
    }

    [Fact]
    public async Task SaveAsync_WhenThemeIsMixedCase_NormalizesToLowercase()
    {
        // Arrange
        var saveRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = "DARK",
            NotificationsEnabledIndicator = true
        };
        _mockRepository
            .Setup(repository => repository.GetByUserIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference?)null);
        _mockRepository
            .Setup(repository => repository.UpsertAsync(It.IsAny<UserPreference>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((UserPreference preference, CancellationToken _) => preference);

        // Act
        var resultDto = await _service.SaveAsync(13, saveRequest, CancellationToken.None);

        // Assert
        Assert.Equal(DisplayThemes.Dark, resultDto.Theme);
    }

    #endregion
}
