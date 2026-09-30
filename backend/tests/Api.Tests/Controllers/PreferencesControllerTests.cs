using System.Net;
using System.Net.Http.Json;
using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Preferences;
using Api.Tests.Integration;
using Xunit;

namespace Api.Tests.Controllers;

/// <summary>
/// Integration tests for the PreferencesController endpoints.
/// Validates envelope structure, validation errors, and persistence round-trips.
/// </summary>
public sealed class PreferencesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _testClient;

    public PreferencesControllerTests(CustomWebApplicationFactory applicationFactory)
    {
        _testClient = applicationFactory.CreateClient();
    }

    private static string PreferencesPath(int userId) => $"/v1/users/{userId}/preferences";

    #region GetPreferences Tests

    [Fact]
    public async Task GetPreferencesAsync_WhenNoPreferences_ReturnsDefaultsInEnvelope()
    {
        // Arrange
        var preferencesPath = PreferencesPath(9001);

        // Act
        var envelopeResponse = await _testClient.GetFromJsonAsync<ItemResponseDto<UserPreferenceDto>>(preferencesPath);

        // Assert
        Assert.NotNull(envelopeResponse);
        Assert.NotNull(envelopeResponse.Item);
        Assert.Equal(9001, envelopeResponse.Item.UserId);
        Assert.Equal(DisplayThemes.Light, envelopeResponse.Item.Theme);
        Assert.True(envelopeResponse.Item.NotificationsEnabledIndicator);
        Assert.False(string.IsNullOrWhiteSpace(envelopeResponse.Metadata.TransactionId));
        Assert.Contains(preferencesPath, envelopeResponse.Links.Self);
    }

    [Fact]
    public async Task GetPreferencesAsync_WhenUserIdIsZero_ReturnsBadRequestWithErrorCode()
    {
        // Arrange
        var preferencesPath = PreferencesPath(0);

        // Act
        var httpResponse = await _testClient.GetAsync(preferencesPath);
        var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(errorResponse);
        Assert.Equal("ORG-VAL-001", errorResponse.Code);
    }

    #endregion

    #region SavePreferences Tests

    [Fact]
    public async Task SavePreferencesAsync_WhenValid_ReturnsSavedPreferencesInEnvelope()
    {
        // Arrange
        var preferencesPath = PreferencesPath(9002);
        var saveRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = DisplayThemes.Dark,
            NotificationsEnabledIndicator = false
        };

        // Act
        var httpResponse = await _testClient.PutAsJsonAsync(preferencesPath, saveRequest);
        var envelopeResponse = await httpResponse.Content.ReadFromJsonAsync<ItemResponseDto<UserPreferenceDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, httpResponse.StatusCode);
        Assert.NotNull(envelopeResponse);
        Assert.Equal(9002, envelopeResponse.Item.UserId);
        Assert.Equal(DisplayThemes.Dark, envelopeResponse.Item.Theme);
        Assert.False(envelopeResponse.Item.NotificationsEnabledIndicator);
    }

    [Fact]
    public async Task SavePreferencesAsync_ThenGet_ReturnsPersistedValues()
    {
        // Arrange
        var preferencesPath = PreferencesPath(9003);
        var saveRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = DisplayThemes.Dark,
            NotificationsEnabledIndicator = true
        };
        var saveHttpResponse = await _testClient.PutAsJsonAsync(preferencesPath, saveRequest);
        var saveResponseBody = await saveHttpResponse.Content.ReadAsStringAsync();

        // Act
        var envelopeResponse = await _testClient.GetFromJsonAsync<ItemResponseDto<UserPreferenceDto>>(preferencesPath);

        // Assert
        Assert.True(saveHttpResponse.StatusCode == HttpStatusCode.OK, $"PUT failed: {saveHttpResponse.StatusCode} - {saveResponseBody}");
        Assert.NotNull(envelopeResponse);
        Assert.Equal(DisplayThemes.Dark, envelopeResponse.Item.Theme);
        Assert.True(envelopeResponse.Item.NotificationsEnabledIndicator);
    }

    [Fact]
    public async Task SavePreferencesAsync_WhenInvalidTheme_ReturnsBadRequestWithFieldDetail()
    {
        // Arrange
        var preferencesPath = PreferencesPath(9004);
        var invalidRequest = new UpdateUserPreferenceRequestDto
        {
            Theme = "neon",
            NotificationsEnabledIndicator = true
        };

        // Act
        var httpResponse = await _testClient.PutAsJsonAsync(preferencesPath, invalidRequest);
        var errorResponse = await httpResponse.Content.ReadFromJsonAsync<ErrorResponseDto>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, httpResponse.StatusCode);
        Assert.NotNull(errorResponse);
        Assert.Equal("ORG-VAL-001", errorResponse.Code);
        Assert.NotNull(errorResponse.Details);
        Assert.Contains(errorResponse.Details, detail => detail.Field == "theme");
    }

    #endregion
}
