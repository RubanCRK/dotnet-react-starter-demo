using Api.Domain;

namespace Api.Repositories;

/// <summary>
/// Data access contract for user preference records.
/// </summary>
public interface IUserPreferencesRepository
{
    /// <summary>
    /// Retrieves the preferences for a user, or null when none have been saved.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user's preferences, or null when not found.</returns>
    Task<UserPreference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// Inserts new preferences or updates the existing record for the user.
    /// </summary>
    /// <param name="preference">The preference record to persist.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The persisted preference record.</returns>
    Task<UserPreference> UpsertAsync(UserPreference preference, CancellationToken cancellationToken);
}
