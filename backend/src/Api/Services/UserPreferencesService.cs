using Api.Domain;
using Api.DTOs.Preferences;
using Api.Repositories;

namespace Api.Services;

/// <summary>
/// Business logic for user preferences, including default values for first-run users.
/// </summary>
public sealed class UserPreferencesService : IUserPreferencesService
{
    private readonly IUserPreferencesRepository _preferencesRepository;

    /// <summary>
    /// Initializes a new instance of the user preferences service.
    /// </summary>
    /// <param name="preferencesRepository">The user preferences repository.</param>
    public UserPreferencesService(IUserPreferencesRepository preferencesRepository)
    {
        _preferencesRepository = preferencesRepository;
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        var persistedPreference = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        if (persistedPreference is null)
        {
            return BuildDefaultPreferences(userId);
        }

        return MapToDto(persistedPreference);
    }

    /// <inheritdoc />
    public async Task<UserPreferenceDto> SaveAsync(
        int userId,
        UpdateUserPreferenceRequestDto request,
        CancellationToken cancellationToken)
    {
        var currentTimestamp = DateTime.UtcNow;
        var existingPreference = await _preferencesRepository.GetByUserIdAsync(userId, cancellationToken);

        var preferenceToPersist = new UserPreference
        {
            UserId = userId,
            Theme = request.Theme.ToLowerInvariant(),
            NotificationsEnabledIndicator = request.NotificationsEnabledIndicator,
            CreatedDate = existingPreference?.CreatedDate ?? currentTimestamp,
            UpdatedDate = currentTimestamp
        };

        var persistedPreference = await _preferencesRepository.UpsertAsync(preferenceToPersist, cancellationToken);
        return MapToDto(persistedPreference);
    }

    private static UserPreferenceDto BuildDefaultPreferences(int userId)
    {
        var currentTimestamp = DateTime.UtcNow;
        return new UserPreferenceDto
        {
            UserId = userId,
            Theme = UserPreference.DefaultTheme,
            NotificationsEnabledIndicator = true,
            CreatedDate = currentTimestamp,
            UpdatedDate = currentTimestamp
        };
    }

    private static UserPreferenceDto MapToDto(UserPreference preference)
    {
        return new UserPreferenceDto
        {
            UserId = preference.UserId,
            Theme = preference.Theme,
            NotificationsEnabledIndicator = preference.NotificationsEnabledIndicator,
            CreatedDate = preference.CreatedDate,
            UpdatedDate = preference.UpdatedDate
        };
    }
}
