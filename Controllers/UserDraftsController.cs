using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.DTOs.Drafts;
using SpeakingPractice.Api.Infrastructure.Persistence;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
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
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var drafts = await context.UserDrafts
            .Where(d => d.UserId == userId)
            .OrderByDescending(d => d.UpdatedAt)
            .ToListAsync(ct);

        return Ok(drafts.Select(MapToDto));
    }

    [HttpGet("user/{userId:guid}/question/{questionId:guid}")]
    public async Task<IActionResult> GetDraft(Guid userId, Guid questionId, CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (userId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var draft = await userDraftRepository.GetByUserAndQuestionAsync(userId, questionId, ct);
        if (draft is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(draft));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var draft = await context.UserDrafts
            .FirstOrDefaultAsync(d => d.Id == id, ct);
        
        if (draft is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(MapToDto(draft));
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
            return NotFound("Question not found");
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
            return Ok(MapToDto(existing));
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
        return CreatedAtAction(nameof(GetById), new { id = draft.Id }, MapToDto(draft));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateDraft(Guid id, [FromBody] SaveDraftRequest request, CancellationToken ct = default)
    {
        var draft = await userDraftRepository.GetByIdAsync(id, ct);
        if (draft is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        draft.DraftContent = request.DraftContent;
        draft.OutlineStructure = request.OutlineStructure;
        draft.UpdatedAt = DateTimeOffset.UtcNow;

        await userDraftRepository.UpdateAsync(draft, ct);
        await userDraftRepository.SaveChangesAsync(ct);

        return Ok(MapToDto(draft));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteDraft(Guid id, CancellationToken ct = default)
    {
        var draft = await userDraftRepository.GetByIdAsync(id, ct);
        if (draft is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (draft.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        await userDraftRepository.DeleteAsync(draft, ct);
        await userDraftRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted draft {DraftId}", id);
        return NoContent();
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

