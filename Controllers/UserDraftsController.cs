using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.Drafts;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/user-drafts")]
public class UserDraftsController(
    IUserDraftRepository userDraftRepository,
    IQuestionRepository questionRepository,
    ApplicationDbContext context,
    ILogger<UserDraftsController> logger) : ControllerBase
{
    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetUserDrafts(Guid userId, CancellationToken ct = default)
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

        var drafts = await context.UserDrafts
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(ct);

        return this.ApiOk(drafts.Select(MapToDto), "Drafts retrieved successfully");
    }

    [HttpGet("user/{userId:guid}/question/{questionId:guid}")]
    public async Task<IActionResult> GetDraft(Guid userId, Guid questionId, CancellationToken ct = default)
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

        var draft = await userDraftRepository.GetByUserAndQuestionAsync(userId, questionId, ct);
        if (draft is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, "Draft not found");
        }

        return this.ApiOk(MapToDto(draft), "Draft retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var draft = await context.UserDrafts
            .FirstOrDefaultAsync(d => d.Id == id, ct);
        
        if (draft is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"Draft with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        return this.ApiOk(MapToDto(draft), "Draft retrieved successfully");
    }

    [HttpPost]
    public async Task<IActionResult> SaveDraft([FromBody] SaveDraftWithQuestionRequest request, CancellationToken ct = default)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
        {
            return Unauthorized();
        }

        var question = await questionRepository.GetByIdAsync(request.QuestionId, ct);
        if (question is null)
        {
            return this.ApiNotFound(ErrorCodes.QUESTION_NOT_FOUND, "Question not found");
        }

        var existing = await userDraftRepository.GetByUserAndQuestionAsync(userId.Value, request.QuestionId, ct);
        
        if (existing is not null)
        {
            // Update existing draft
            existing.DraftContent = request.DraftContent;
            existing.OutlineStructure = request.OutlineStructure;
            existing.UpdatedAt = DateTimeOffset.UtcNow;

            await userDraftRepository.UpdateAsync(existing, ct);
            await userDraftRepository.SaveChangesAsync(ct);

            logger.LogInformation("Updated draft {DraftId} for user {UserId}", existing.Id, userId);
            return this.ApiOk(MapToDto(existing), "Draft updated successfully");
        }

        // Create new draft
        var draft = new Domain.Entities.UserDraft
        {
            UserId = userId.Value,
            QuestionId = request.QuestionId,
            DraftContent = request.DraftContent,
            OutlineStructure = request.OutlineStructure,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await userDraftRepository.AddAsync(draft, ct);
        await userDraftRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created draft {DraftId} for user {UserId}", draft.Id, userId);
        return this.ApiCreated(nameof(GetById), new { id = draft.Id }, MapToDto(draft), "Draft created successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDraft(Guid id, [FromBody] SaveDraftRequest request, CancellationToken ct = default)
    {
        var draft = await userDraftRepository.GetByIdAsync(id, ct);
        if (draft is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"Draft with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        draft.DraftContent = request.DraftContent;
        draft.OutlineStructure = request.OutlineStructure;
        draft.UpdatedAt = DateTimeOffset.UtcNow;

        await userDraftRepository.UpdateAsync(draft, ct);
        await userDraftRepository.SaveChangesAsync(ct);

        return this.ApiOk(MapToDto(draft), "Draft retrieved successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDraft(Guid id, CancellationToken ct = default)
    {
        var draft = await userDraftRepository.GetByIdAsync(id, ct);
        if (draft is null)
        {
            return this.ApiNotFound(ErrorCodes.NOT_FOUND, $"Draft with id {id} not found");
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return this.ApiForbid(ErrorCodes.FORBIDDEN, "You don't have permission to access this resource");
        }

        await userDraftRepository.DeleteAsync(draft, ct);
        await userDraftRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted draft {DraftId}", id);
        return this.ApiOk("Draft deleted successfully");
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static UserDraftDto MapToDto(Domain.Entities.UserDraft draft)
    {
        return new UserDraftDto
        {
            Id = draft.Id,
            UserId = draft.UserId,
            QuestionId = draft.QuestionId,
            DraftContent = draft.DraftContent,
            OutlineStructure = draft.OutlineStructure,
            CreatedAt = draft.CreatedAt,
            UpdatedAt = draft.UpdatedAt
        };
    }
}

public class SaveDraftWithQuestionRequest : SaveDraftRequest
{
    public Guid QuestionId { get; set; }
}

