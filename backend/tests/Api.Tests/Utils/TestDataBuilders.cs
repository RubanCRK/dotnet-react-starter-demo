namespace Api.Tests.Utils;

/// <summary>
/// Factory methods for creating test data objects.
/// Provides consistent, configurable test fixtures with sensible defaults.
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Creates a sample user preference entity for testing purposes.
    /// </summary>
    /// <param name="userId">Owning user identifier (default: 1).</param>
    /// <param name="theme">Display theme value (default: "light").</param>
    /// <param name="notificationsEnabled">Notification flag (default: true).</param>
    /// <returns>A configured user preference entity.</returns>
    public static Api.Domain.UserPreference BuildUserPreference(
        int userId = 1,
        string theme = Api.Domain.DisplayThemes.Light,
        bool notificationsEnabled = true)
    {
        return new Api.Domain.UserPreference
        {
            UserId = userId,
            Theme = theme,
            NotificationsEnabledIndicator = notificationsEnabled,
            CreatedDate = DateTime.UtcNow.AddDays(-1),
            UpdatedDate = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a sample health response for testing purposes.
    /// </summary>
    /// <param name="status">Health status value (default: "healthy").</param>
    /// <param name="version">Application version (default: "1.0.0.0").</param>
    /// <param name="environment">Runtime environment (default: "Testing").</param>
    /// <returns>A configured health response DTO.</returns>
    public static Api.DTOs.Health.HealthResponseDto BuildHealthResponse(
        string status = "healthy",
        string version = "1.0.0.0",
        string environment = "Testing")
    {
        return new Api.DTOs.Health.HealthResponseDto
        {
            Status = status,
            Version = version,
            CheckedAtDate = DateTime.UtcNow,
            Environment = environment
        };
    }

    /// <summary>
    /// Creates sample metadata for envelope responses.
    /// </summary>
    /// <param name="transactionId">Correlation identifier (default: new GUID).</param>
    /// <param name="totalCount">Optional total count for collections.</param>
    /// <returns>A configured metadata DTO.</returns>
    public static Api.DTOs.Common.MetadataDto BuildMetadata(
        string? transactionId = null,
        int? totalCount = null)
    {
        return new Api.DTOs.Common.MetadataDto
        {
            Timestamp = DateTime.UtcNow,
            TransactionId = transactionId ?? Guid.NewGuid().ToString("D"),
            TotalCount = totalCount
        };
    }

    /// <summary>
    /// Creates sample links for envelope responses.
    /// </summary>
    /// <param name="selfUrl">Canonical URL of the resource.</param>
    /// <param name="nextUrl">Optional next page URL.</param>
    /// <param name="prevUrl">Optional previous page URL.</param>
    /// <returns>A configured links DTO.</returns>
    public static Api.DTOs.Common.LinksDto BuildLinks(
        string selfUrl = "http://localhost/v1/resource",
        string? nextUrl = null,
        string? prevUrl = null)
    {
        return new Api.DTOs.Common.LinksDto
        {
            Self = selfUrl,
            Next = nextUrl,
            Prev = prevUrl
        };
    }
}
