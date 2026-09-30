using Api.Domain;
using Api.DTOs.Common;
using Api.DTOs.Preferences;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

/// <summary>
/// User preferences endpoints for display theme and notification settings.
/// Preferences are scoped to a user identifier until authentication is implemented.
/// </summary>
[Route("v1/users/{userId:int}/preferences")]
[ApiController]
[Produces("application/json")]
public class PreferencesController : ControllerBase
{
    private const string ValidationErrorCode = "ORG-VAL-001";

    private readonly IUserPreferencesService _preferencesService;

    /// <summary>
    /// Initializes a new instance of the preferences controller.
    /// </summary>
    /// <param name="preferencesService">The user preferences service.</param>
    public PreferencesController(IUserPreferencesService preferencesService)
    {
        _preferencesService = preferencesService;
    }

    /// <summary>
    /// Retrieves the preferences for a user.
    /// Returns system defaults when the user has not saved preferences yet.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The user's preferences wrapped in the standard envelope.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(ItemResponseDto<UserPreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<UserPreferenceDto>>> GetPreferences(
        int userId,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return BuildValidationError("userId", "The user identifier must be a positive number.");
        }

        var preferences = await _preferencesService.GetByUserIdAsync(userId, cancellationToken);
        return Ok(BuildEnvelope(preferences));
    }

    /// <summary>
    /// Saves the preferences for a user, creating or updating them as needed.
    /// </summary>
    /// <param name="userId">The identifier of the user.</param>
    /// <param name="updateRequest">The preference values to save.</param>
    /// <param name="cancellationToken">Token to cancel the asynchronous operation.</param>
    /// <returns>The persisted preferences wrapped in the standard envelope.</returns>
    [HttpPut]
    [ProducesResponseType(typeof(ItemResponseDto<UserPreferenceDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ItemResponseDto<UserPreferenceDto>>> SavePreferences(
        int userId,
        [FromBody] UpdateUserPreferenceRequestDto updateRequest,
        CancellationToken cancellationToken)
    {
        if (userId <= 0)
        {
            return BuildValidationError("userId", "The user identifier must be a positive number.");
        }

        if (!DisplayThemes.IsSupported(updateRequest.Theme))
        {
            return BuildValidationError(
                "theme",
                $"The theme must be one of: {DisplayThemes.Light}, {DisplayThemes.Dark}.");
        }

        var savedPreferences = await _preferencesService.SaveAsync(userId, updateRequest, cancellationToken);
        return Ok(BuildEnvelope(savedPreferences));
    }

    private ItemResponseDto<UserPreferenceDto> BuildEnvelope(UserPreferenceDto preferences)
    {
        return new ItemResponseDto<UserPreferenceDto>
        {
            Item = preferences,
            Metadata = new MetadataDto
            {
                Timestamp = DateTime.UtcNow,
                TransactionId = HttpContext.Items["TransactionId"]?.ToString() ?? Guid.NewGuid().ToString(),
                TotalCount = null
            },
            Links = new LinksDto
            {
                Self = $"{Request.Scheme}://{Request.Host}{Request.PathBase}{Request.Path}",
                Next = null,
                Prev = null
            }
        };
    }

    private BadRequestObjectResult BuildValidationError(string invalidField, string validationMessage)
    {
        return BadRequest(new ErrorResponseDto
        {
            Code = ValidationErrorCode,
            Message = "The request contains invalid values.",
            Details = new[]
            {
                new ErrorDetailDto
                {
                    Field = invalidField,
                    Message = validationMessage
                }
            }
        });
    }
}
