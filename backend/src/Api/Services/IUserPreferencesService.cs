using Api.DTOs.Preferences;

namespace Api.Services;

/// <summary>
/// Business logic contract for user preferences.
/// </summary>
public interface IUserPreferencesService
{
    /// <summary>
    /// Retrieves the preferences for a user, returning defaults when none have been saved.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user's preferences, or system defaults for first-run users.</returns>
    Task<UserPreferenceDto> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Saves the preferences for a user, creating or updating as needed.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="request">The preference values to save.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The persisted preferences.</returns>
    Task<UserPreferenceDto> SaveAsync(int userId, UpdateUserPreferenceRequestDto request, CancellationToken cancellationToken);
}
