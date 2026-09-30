namespace Api.Domain;

/// <summary>
/// Represents persisted user preferences for display theme and notifications.
/// One row per user; the user identifier is the primary key.
/// </summary>
public sealed class UserPreference
{
    /// <summary>
    /// Default theme applied when a user has not saved preferences.
    /// </summary>
    public const string DefaultTheme = DisplayThemes.Light;

    /// <summary>
    /// The identifier of the user who owns these preferences.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The display theme selected by the user. Must be a value from <see cref="DisplayThemes"/>.
    /// </summary>
    public string Theme { get; set; } = DefaultTheme;

    /// <summary>
    /// Indicates whether the user receives notifications.
    /// </summary>
    public bool NotificationsEnabledIndicator { get; set; } = true;

    /// <summary>
    /// UTC timestamp when the preferences were first saved.
    /// </summary>
    public DateTime CreatedDate { get; set; }

    /// <summary>
    /// UTC timestamp when the preferences were last saved.
    /// </summary>
    public DateTime UpdatedDate { get; set; }
}

/// <summary>
/// Allowed values for the display theme preference.
/// </summary>
public static class DisplayThemes
{
    /// <summary>Light display theme.</summary>
    public const string Light = "light";

    /// <summary>Dark display theme.</summary>
    public const string Dark = "dark";

    /// <summary>
    /// Determines whether the provided value is a supported display theme.
    /// </summary>
    /// <param name="theme">The theme value to validate.</param>
    /// <returns>True when the value is a supported theme; otherwise false.</returns>
    public static bool IsSupported(string? theme)
    {
        return string.Equals(theme, Light, StringComparison.OrdinalIgnoreCase)
            || string.Equals(theme, Dark, StringComparison.OrdinalIgnoreCase);
    }
}
