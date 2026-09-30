using System.Text.Json.Serialization;

namespace Api.DTOs.Preferences;

/// <summary>
/// Represents a user's saved display and notification preferences.
/// </summary>
public sealed class UserPreferenceDto
{
    /// <summary>
    /// The identifier of the user who owns these preferences.
    /// </summary>
    [JsonPropertyName("userId")]
    public int UserId { get; set; }

    /// <summary>
    /// The display theme selected by the user ("light" or "dark").
    /// </summary>
    [JsonPropertyName("theme")]
    public required string Theme { get; set; }

    /// <summary>
    /// Indicates whether the user receives notifications.
    /// </summary>
    [JsonPropertyName("notificationsEnabledIndicator")]
    public bool NotificationsEnabledIndicator { get; set; }

    /// <summary>
    /// UTC timestamp when the preferences were first saved.
    /// </summary>
    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// UTC timestamp when the preferences were last saved.
    /// </summary>
    [JsonPropertyName("updatedDate")]
    public DateTime UpdatedDate { get; set; }
}
