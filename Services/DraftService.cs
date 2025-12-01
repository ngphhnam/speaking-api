using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Drafts;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class DraftService(
    IUserDraftRepository draftRepository,
    IQuestionRepository questionRepository,
    ILogger<DraftService> logger) : IDraftService
{
    public async Task<UserDraftDto> SaveDraftAsync(Guid userId, Guid questionId, SaveDraftRequest request, CancellationToken ct)
    {
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question == null)
            throw new InvalidOperationException($"Question with id {questionId} not found");

        var existingDraft = await draftRepository.GetByUserAndQuestionAsync(userId, questionId, ct);
        
        if (existingDraft != null)
        {
            existingDraft.DraftContent = request.DraftContent;
            existingDraft.OutlineStructure = request.OutlineStructure;
            existingDraft.UpdatedAt = DateTimeOffset.UtcNow;
            await draftRepository.UpdateAsync(existingDraft, ct);
            await draftRepository.SaveChangesAsync(ct);
            return MapToDto(existingDraft);
        }

        var draft = new UserDraft
        {
            UserId = userId,
            QuestionId = questionId,
            DraftContent = request.DraftContent,
            OutlineStructure = request.OutlineStructure,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await draftRepository.AddAsync(draft, ct);
        await draftRepository.SaveChangesAsync(ct);

        logger.LogInformation("Saved draft for user {UserId} and question {QuestionId}", userId, questionId);
        return MapToDto(draft);
    }

    public async Task<UserDraftDto?> GetDraftAsync(Guid userId, Guid questionId, CancellationToken ct)
    {
        var draft = await draftRepository.GetByUserAndQuestionAsync(userId, questionId, ct);
        return draft != null ? MapToDto(draft) : null;
    }

    public async Task DeleteDraftAsync(Guid userId, Guid questionId, CancellationToken ct)
    {
        var draft = await draftRepository.GetByUserAndQuestionAsync(userId, questionId, ct);
        if (draft != null)
        {
            await draftRepository.DeleteAsync(draft, ct);
            await draftRepository.SaveChangesAsync(ct);
            logger.LogInformation("Deleted draft for user {UserId} and question {QuestionId}", userId, questionId);
        }
    }

    private static UserDraftDto MapToDto(UserDraft draft)
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

