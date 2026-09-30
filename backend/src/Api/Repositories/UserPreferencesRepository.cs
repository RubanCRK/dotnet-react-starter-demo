using Api.Data;
using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Repositories;

/// <summary>
/// Entity Framework Core implementation of the user preferences repository.
/// </summary>
public sealed class UserPreferencesRepository : IUserPreferencesRepository
{
    private readonly AppDbContext _dbContext;

    /// <summary>
    /// Initializes a new instance of the user preferences repository.
    /// </summary>
    /// <param name="dbContext">The application database context.</param>
    public UserPreferencesRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    /// <inheritdoc />
    public async Task<UserPreference?> GetByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.UserPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<UserPreference> UpsertAsync(UserPreference preference, CancellationToken cancellationToken)
    {
        var existingPreference = await _dbContext.UserPreferences
            .FirstOrDefaultAsync(record => record.UserId == preference.UserId, cancellationToken);

        if (existingPreference is null)
        {
            _dbContext.UserPreferences.Add(preference);
        }
        else
        {
            existingPreference.Theme = preference.Theme;
            existingPreference.NotificationsEnabledIndicator = preference.NotificationsEnabledIndicator;
            existingPreference.UpdatedDate = preference.UpdatedDate;
            preference = existingPreference;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
        return preference;
    }
}
