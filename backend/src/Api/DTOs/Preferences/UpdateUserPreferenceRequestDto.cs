using System.Text.Json.Serialization;

namespace Api.DTOs.Preferences;

/// <summary>
/// Request payload for saving a user's preferences.
/// </summary>
public sealed class UpdateUserPreferenceRequestDto
{
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
}
