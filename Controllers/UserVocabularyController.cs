using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.UserVocabulary;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class UserVocabularyController(
    IUserVocabularyRepository userVocabularyRepository,
    IVocabularyRepository vocabularyRepository,
    ApplicationDbContext context,
    ILogger<UserVocabularyController> logger) : ControllerBase
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserVocabulary(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await userVocabularyRepository.GetByUserIdAsync(userId, ct);
        var vocabularies = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId)
            .ToListAsync(ct);

        return this.ApiOk(vocabularies.Select(MapToDto), "User vocabularies retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/learning")]
    public async Task<IActionResult> GetLearning(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await userVocabularyRepository.GetByUserIdAndStatusAsync(userId, "learning", ct);
        var vocabularies = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId && uv.LearningStatus == "learning")
            .ToListAsync(ct);

        return this.ApiOk(vocabularies.Select(MapToDto), "User vocabularies retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/reviewing")]
    public async Task<IActionResult> GetReviewing(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await userVocabularyRepository.GetByUserIdAndStatusAsync(userId, "reviewing", ct);
        var vocabularies = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId && uv.LearningStatus == "reviewing")
            .ToListAsync(ct);

        return this.ApiOk(vocabularies.Select(MapToDto), "User vocabularies retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/mastered")]
    public async Task<IActionResult> GetMastered(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await userVocabularyRepository.GetByUserIdAndStatusAsync(userId, "mastered", ct);
        var vocabularies = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId && uv.LearningStatus == "mastered")
            .ToListAsync(ct);

        return this.ApiOk(vocabularies.Select(MapToDto), "User vocabularies retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/due-for-review")]
    public async Task<IActionResult> GetDueForReview(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await userVocabularyRepository.GetDueForReviewAsync(userId, ct);
        var vocabularies = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .Where(uv => uv.UserId == userId && uv.NextReviewAt.HasValue && uv.NextReviewAt <= DateTimeOffset.UtcNow)
            .ToListAsync(ct);

        return this.ApiOk(vocabularies.Select(MapToDto), "User vocabularies retrieved successfully");
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddUserVocabularyRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var vocabulary = await vocabularyRepository.GetByIdAsync(request.VocabularyId, ct);
        if (vocabulary is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Vocabulary not found");
        }

        var existing = await userVocabularyRepository.GetByUserAndVocabularyAsync(userId.Value, request.VocabularyId, ct);
        if (existing is not null)
        {
            return this.ApiStatusCode(409, ErrorCodes.UNIQUE_CONSTRAINT_VIOLATION, "Vocabulary already added to user's list");
        }

        var userVocabulary = new Domain.Entities.UserVocabulary
        {
            UserId = userId.Value,
            VocabularyId = request.VocabularyId,
            LearningStatus = "new",
            PersonalNotes = request.PersonalNotes,
            FirstEncounteredAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await userVocabularyRepository.AddAsync(userVocabulary, ct);
        await userVocabularyRepository.SaveChangesAsync(ct);

        var result = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .FirstOrDefaultAsync(uv => uv.Id == userVocabulary.Id, ct);

        logger.LogInformation("Added vocabulary {VocabularyId} to user {UserId}", request.VocabularyId, userId);
        return this.ApiCreated(nameof(GetUserVocabulary), new { userId = userId.Value }, MapToDto(result!), "Vocabulary added successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserVocabularyRequest request, CancellationToken ct = default)
    {
        var userVocabulary = await userVocabularyRepository.GetByIdAsync(id, ct);
        if (userVocabulary is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"User vocabulary with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userVocabulary.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        if (!string.IsNullOrWhiteSpace(request.LearningStatus))
        {
            var validStatuses = new[] { "new", "learning", "reviewing", "mastered" };
            if (!validStatuses.Contains(request.LearningStatus))
            {
                return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "Invalid learning status");
            }

            userVocabulary.LearningStatus = request.LearningStatus;
            
            if (request.LearningStatus == "mastered" && !userVocabulary.MasteredAt.HasValue)
            {
                userVocabulary.MasteredAt = DateTimeOffset.UtcNow;
            }

            // Set next review date based on status
            if (request.LearningStatus == "reviewing")
            {
                userVocabulary.NextReviewAt = DateTimeOffset.UtcNow.AddDays(1);
            }
            else if (request.LearningStatus == "learning")
            {
                userVocabulary.NextReviewAt = DateTimeOffset.UtcNow.AddDays(3);
            }
        }

        if (request.PersonalNotes is not null) userVocabulary.PersonalNotes = request.PersonalNotes;
        if (request.ExampleUsage is not null) userVocabulary.ExampleUsage = request.ExampleUsage;

        await userVocabularyRepository.UpdateAsync(userVocabulary, ct);
        await userVocabularyRepository.SaveChangesAsync(ct);

        var result = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .FirstOrDefaultAsync(uv => uv.Id == id, ct);

        return this.ApiOk(MapToDto(result!), "User vocabulary updated successfully");
    }

    [HttpPut("{id:guid}/review")]
    public async Task<IActionResult> MarkAsReviewed(Guid id, [FromBody] bool? success, CancellationToken ct = default)
    {
        var userVocabulary = await userVocabularyRepository.GetByIdAsync(id, ct);
        if (userVocabulary is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"User vocabulary with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userVocabulary.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        userVocabulary.ReviewCount++;
        userVocabulary.LastReviewedAt = DateTimeOffset.UtcNow;

        if (success == true)
        {
            userVocabulary.SuccessCount++;
            
            // Update next review date based on success count (spaced repetition)
            var daysUntilNextReview = userVocabulary.SuccessCount switch
            {
                1 => 1,
                2 => 3,
                3 => 7,
                4 => 14,
                >= 5 => 30,
                _ => 1
            };
            userVocabulary.NextReviewAt = DateTimeOffset.UtcNow.AddDays(daysUntilNextReview);

            // Auto-promote to mastered after 5 successful reviews
            if (userVocabulary.SuccessCount >= 5 && userVocabulary.LearningStatus != "mastered")
            {
                userVocabulary.LearningStatus = "mastered";
                userVocabulary.MasteredAt = DateTimeOffset.UtcNow;
            }
        }
        else
        {
            // Reset if failed
            userVocabulary.LearningStatus = "learning";
            userVocabulary.NextReviewAt = DateTimeOffset.UtcNow.AddDays(1);
        }

        await userVocabularyRepository.UpdateAsync(userVocabulary, ct);
        await userVocabularyRepository.SaveChangesAsync(ct);

        var result = await context.UserVocabularies
            .Include(uv => uv.Vocabulary)
            .FirstOrDefaultAsync(uv => uv.Id == id, ct);

        return this.ApiOk(MapToDto(result!), "User vocabulary updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var userVocabulary = await userVocabularyRepository.GetByIdAsync(id, ct);
        if (userVocabulary is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"User vocabulary with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userVocabulary.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        await userVocabularyRepository.DeleteAsync(userVocabulary, ct);
        await userVocabularyRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted user vocabulary {UserVocabularyId}", id);
        return this.ApiOk("User vocabulary deleted successfully");
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetStatistics(Guid userId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        var userVocabularies = await context.UserVocabularies
            .Where(uv => uv.UserId == userId)
            .ToListAsync(ct);

        var total = userVocabularies.Count;
        var mastered = userVocabularies.Count(uv => uv.LearningStatus == "mastered");
        var statistics = new VocabularyStatisticsDto
        {
            UserId = userId,
            TotalVocabulary = total,
            LearningCount = userVocabularies.Count(uv => uv.LearningStatus == "learning"),
            ReviewingCount = userVocabularies.Count(uv => uv.LearningStatus == "reviewing"),
            MasteredCount = mastered,
            DueForReviewCount = userVocabularies.Count(uv => uv.NextReviewAt.HasValue && uv.NextReviewAt <= DateTimeOffset.UtcNow),
            MasteryPercentage = total > 0 ? (decimal)mastered / total * 100 : 0
        };

        return this.ApiOk(statistics, "Statistics retrieved successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static UserVocabularyDto MapToDto(Domain.Entities.UserVocabulary userVocabulary)
    {
        return new UserVocabularyDto
        {
            Id = userVocabulary.Id,
            UserId = userVocabulary.UserId,
            VocabularyId = userVocabulary.VocabularyId,
            Vocabulary = new VocabularyDto
            {
                Id = userVocabulary.Vocabulary.Id,
                Word = userVocabulary.Vocabulary.Word,
                Phonetic = userVocabulary.Vocabulary.Phonetic,
                PartOfSpeech = userVocabulary.Vocabulary.PartOfSpeech,
                DefinitionEn = userVocabulary.Vocabulary.DefinitionEn,
                DefinitionVi = userVocabulary.Vocabulary.DefinitionVi,
                IeltsBandLevel = userVocabulary.Vocabulary.IeltsBandLevel,
                TopicCategories = userVocabulary.Vocabulary.TopicCategories,
                ExampleSentences = userVocabulary.Vocabulary.ExampleSentences,
                Synonyms = userVocabulary.Vocabulary.Synonyms,
                Antonyms = userVocabulary.Vocabulary.Antonyms,
                Collocations = userVocabulary.Vocabulary.Collocations
            },
            LearningStatus = userVocabulary.LearningStatus,
            NextReviewAt = userVocabulary.NextReviewAt,
            ReviewCount = userVocabulary.ReviewCount,
            SuccessCount = userVocabulary.SuccessCount,
            PersonalNotes = userVocabulary.PersonalNotes,
            ExampleUsage = userVocabulary.ExampleUsage,
            FirstEncounteredAt = userVocabulary.FirstEncounteredAt,
            LastReviewedAt = userVocabulary.LastReviewedAt,
            MasteredAt = userVocabulary.MasteredAt,
            CreatedAt = userVocabulary.CreatedAt
        };
    }
}
