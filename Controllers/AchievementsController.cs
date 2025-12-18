using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Achievements;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchievementsController(
    IAchievementRepository achievementRepository,
    IMinioClientWrapper minioClient,
    ILogger<AchievementsController> logger) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var achievements = await achievementRepository.GetAllAsync(ct);
        return this.ApiOk(achievements.Select(MapToDto), "Achievements retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var achievement = await achievementRepository.GetByIdAsync(id, ct);
        if (achievement is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"Achievement with id {id} not found");
        }

        return this.ApiOk(MapToDto(achievement), "Achievement retrieved successfully");
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive(CancellationToken ct = default)
    {
        var achievements = await achievementRepository.GetActiveAsync(ct);
        return this.ApiOk(achievements.Select(MapToDto), "Active achievements retrieved successfully");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAchievementRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return this.ApiBadRequest(ErrorCodes.REQUIRED_FIELD_MISSING, "Title is required");
        }

        var achievement = new Domain.Entities.Achievement
        {
            Title = request.Title,
            Description = request.Description,
            AchievementType = request.AchievementType ?? "general",
            RequirementCriteria = request.RequirementCriteria ?? "{}",
            Points = request.Points,
            BadgeIconUrl = request.BadgeIconUrl,
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await achievementRepository.AddAsync(achievement, ct);
        await achievementRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created achievement {AchievementId}", achievement.Id);
        return this.ApiCreated(nameof(GetById), new { id = achievement.Id }, MapToDto(achievement), "Achievement created successfully");
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAchievementRequest request, CancellationToken ct = default)
    {
        var achievement = await achievementRepository.GetByIdAsync(id, ct);
        if (achievement is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Title)) achievement.Title = request.Title;
        if (request.Description is not null) achievement.Description = request.Description;
        if (!string.IsNullOrWhiteSpace(request.AchievementType)) achievement.AchievementType = request.AchievementType;
        if (request.RequirementCriteria is not null) achievement.RequirementCriteria = request.RequirementCriteria;
        if (request.Points.HasValue) achievement.Points = request.Points.Value;
        if (request.BadgeIconUrl is not null) achievement.BadgeIconUrl = request.BadgeIconUrl;
        if (request.IsActive.HasValue) achievement.IsActive = request.IsActive.Value;

        await achievementRepository.UpdateAsync(achievement, ct);
        await achievementRepository.SaveChangesAsync(ct);

        return this.ApiOk(MapToDto(achievement), "Achievement updated successfully");
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var achievement = await achievementRepository.GetByIdAsync(id, ct);
        if (achievement is null)
        {
            return NotFound();
        }

        await achievementRepository.DeleteAsync(achievement, ct);
        await achievementRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted achievement {AchievementId}", id);
        return this.ApiOk("Achievement deleted successfully");
    }

    [HttpPost("upload-badge-icon")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(5_000_000)] // 5MB limit
    public async Task<IActionResult> UploadBadgeIcon([FromForm] IFormFile badgeIcon, CancellationToken ct = default)
    {
        if (badgeIcon is null || badgeIcon.Length == 0)
        {
            return this.ApiBadRequest(ErrorCodes.REQUIRED_FIELD_MISSING, "Badge icon file is required");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml" };
        if (!allowedTypes.Contains(badgeIcon.ContentType.ToLower()))
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "Invalid file type. Allowed types: JPEG, PNG, GIF, WEBP, SVG");
        }

        // Validate file size (max 5MB)
        if (badgeIcon.Length > 5_000_000)
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "File size exceeds 5MB limit");
        }

        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(badgeIcon.FileName);
            var objectName = $"badges/badge_{Guid.NewGuid()}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";

            // Upload to MinIO
            string badgeIconUrl;
            using (var stream = badgeIcon.OpenReadStream())
            {
                badgeIconUrl = await minioClient.UploadImageAsync(stream, objectName, Guid.Empty, ct);
            }

            logger.LogInformation("Uploaded badge icon: {BadgeIconUrl}", badgeIconUrl);
            
            return this.ApiOk(new { badgeIconUrl }, "Badge icon uploaded successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading badge icon");
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to upload badge icon", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    [HttpPost("{id:guid}/upload-badge-icon")]
    [Authorize(Roles = "Admin")]
    [RequestSizeLimit(5_000_000)] // 5MB limit
    public async Task<IActionResult> UploadAndUpdateBadgeIcon(Guid id, [FromForm] IFormFile badgeIcon, CancellationToken ct = default)
    {
        var achievement = await achievementRepository.GetByIdAsync(id, ct);
        if (achievement is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"Achievement with id {id} not found");
        }

        if (badgeIcon is null || badgeIcon.Length == 0)
        {
            return this.ApiBadRequest(ErrorCodes.REQUIRED_FIELD_MISSING, "Badge icon file is required");
        }

        // Validate file type
        var allowedTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp", "image/svg+xml" };
        if (!allowedTypes.Contains(badgeIcon.ContentType.ToLower()))
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "Invalid file type. Allowed types: JPEG, PNG, GIF, WEBP, SVG");
        }

        // Validate file size (max 5MB)
        if (badgeIcon.Length > 5_000_000)
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "File size exceeds 5MB limit");
        }

        try
        {
            // Generate unique filename
            var extension = Path.GetExtension(badgeIcon.FileName);
            var objectName = $"badges/badge_{id}_{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}{extension}";

            // Upload to MinIO
            string badgeIconUrl;
            using (var stream = badgeIcon.OpenReadStream())
            {
                badgeIconUrl = await minioClient.UploadImageAsync(stream, objectName, Guid.Empty, ct);
            }

            // Update achievement with new badge icon URL
            achievement.BadgeIconUrl = badgeIconUrl;
            await achievementRepository.UpdateAsync(achievement, ct);
            await achievementRepository.SaveChangesAsync(ct);

            logger.LogInformation("Updated badge icon for achievement {AchievementId}: {BadgeIconUrl}", id, badgeIconUrl);
            
            return this.ApiOk(new { badgeIconUrl, achievement = MapToDto(achievement) }, "Badge icon uploaded and updated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error uploading badge icon for achievement {AchievementId}", id);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to upload badge icon", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    private static AchievementDto MapToDto(Domain.Entities.Achievement achievement)
    {
        return new AchievementDto
        {
            Id = achievement.Id,
            Title = achievement.Title,
            Description = achievement.Description,
            AchievementType = achievement.AchievementType,
            Points = achievement.Points,
            BadgeIconUrl = achievement.BadgeIconUrl,
            IsActive = achievement.IsActive,
            CreatedAt = achievement.CreatedAt
        };
    }
}
