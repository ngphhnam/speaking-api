using SpeakingPractice.Api.DTOs.Drafts;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IDraftService
{
    Task<UserDraftDto> SaveDraftAsync(Guid userId, Guid questionId, SaveDraftRequest request, CancellationToken ct);
    Task<UserDraftDto?> GetDraftAsync(Guid userId, Guid questionId, CancellationToken ct);
    Task DeleteDraftAsync(Guid userId, Guid questionId, CancellationToken ct);
}

