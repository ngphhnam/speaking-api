using SpeakingPractice.Api.DTOs.SpeakingSessions;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface ISpeakingSessionService
{
    Task<SpeakingSessionDto> CreateSessionAsync(
        CreateSpeakingSessionRequest request,
        Stream audioStream,
        string fileName,
        Guid userId,
        CancellationToken ct);

    Task<IReadOnlyCollection<SpeakingSessionListItemDto>> GetUserSessionsAsync(Guid userId, CancellationToken ct);
    Task<SpeakingSessionDto?> GetByIdAsync(Guid id, Guid requesterId, bool isAdmin, CancellationToken ct);
}

