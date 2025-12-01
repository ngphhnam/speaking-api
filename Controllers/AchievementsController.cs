using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Achievements;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AchievementsController(
    IAchievementRepository achievementRepository,
    ILogger<AchievementsController> logger) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(CancellationToken ct = default)
    {
        var achievements = await achievementRepository.GetAllAsync(ct);
        return Ok(achievements.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var achievement = await achievementRepository.GetByIdAsync(id, ct);
        if (achievement is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(achievement));
    }

    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActive(CancellationToken ct = default)
    {
        var achievements = await achievementRepository.GetActiveAsync(ct);
        return Ok(achievements.Select(MapToDto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] CreateAchievementRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return BadRequest("Title is required");
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
        return CreatedAtAction(nameof(GetById), new { id = achievement.Id }, MapToDto(achievement));
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

        return Ok(MapToDto(achievement));
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
        return NoContent();
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
