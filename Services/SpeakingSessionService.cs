using Microsoft.AspNetCore.Identity;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class SpeakingSessionService(
    IMinioClientWrapper minioClient,
    IWhisperClient whisperClient,
    ILlamaClient llamaClient,
    ILanguageToolClient languageToolClient,
    ISpeakingSessionRepository sessionRepository,
    IRecordingRepository recordingRepository,
    IAnalysisResultRepository analysisResultRepository,
    UserManager<ApplicationUser> userManager,
    ILogger<SpeakingSessionService> logger) : ISpeakingSessionService
{
    public async Task<SpeakingSessionDto> CreateSessionAsync(
        CreateSpeakingSessionRequest request,
        Guid userId,
        CancellationToken ct)
    {
        // Verify user exists
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new UnauthorizedAccessException("User not found");
        }

        var now = DateTimeOffset.UtcNow;

        var session = new PracticeSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TopicId = request.TopicId,
            SessionType = "practice",
            Status = "in_progress",
            QuestionsAttempted = 0,
            StartedAt = now,
            CreatedAt = now,
            UpdatedAt = now
        };

        await sessionRepository.AddAsync(session, ct);
        await sessionRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created speaking practice session {SessionId} for user {UserId}", session.Id, userId);

        // Lúc mới tạo session chưa có recording/analysis, nên DTO trả về chủ yếu là meta
        return new SpeakingSessionDto
        {
            Id = session.Id,
            Topic = session.Topic?.Title ?? string.Empty,
            Level = session.Topic?.DifficultyLevel ?? string.Empty,
            AudioUrl = string.Empty,
            TranscriptionText = null,
            BandScore = null,
            PronunciationScore = null,
            GrammarScore = null,
            VocabularyScore = null,
            FluencyScore = null,
            OverallFeedback = null,
            GrammarReportJson = null,
            CreatedAt = session.CreatedAt,
            UpdatedAt = session.UpdatedAt
        };
    }

    public async Task<IReadOnlyCollection<SpeakingSessionListItemDto>> GetUserSessionsAsync(Guid userId, CancellationToken ct)
    {
        var sessions = await sessionRepository.GetByUserAsync(userId, ct);
        return sessions.Select(s => new SpeakingSessionListItemDto
        {
            Id = s.Id,
            Topic = s.Topic?.Title ?? "N/A",
            Level = s.Topic?.DifficultyLevel ?? "N/A",
            BandScore = s.OverallBandScore,
            Status = s.Status,
            TotalDurationSeconds = s.TotalDurationSeconds,
            OverallBandScore = s.OverallBandScore,
            CreatedAt = s.CreatedAt
        }).ToList();
    }

    public async Task<SpeakingSessionDto?> GetByIdAsync(Guid id, Guid requesterId, bool isAdmin, CancellationToken ct)
    {
        var session = await sessionRepository.GetByIdAsync(id, ct);
        if (session is null)
        {
            return null;
        }

        if (!isAdmin && session.UserId != requesterId)
        {
            return null;
        }

        var recording = session.Recordings.FirstOrDefault();
        var analysisResult = recording?.AnalysisResult;

        return MapToDto(session, recording, analysisResult);
    }

    private static SpeakingSessionDto MapToDto(PracticeSession session, Recording? recording, AnalysisResult? analysisResult) => new()
    {
        Id = session.Id,
        Topic = session.Topic?.Title ?? "N/A",
        Level = session.Topic?.DifficultyLevel ?? "N/A",
        AudioUrl = recording?.AudioUrl ?? string.Empty,
        TranscriptionText = recording?.TranscriptionText,
        BandScore = analysisResult?.OverallBandScore ?? session.OverallBandScore,
        PronunciationScore = analysisResult?.PronunciationScore ?? session.PronunciationScore,
        GrammarScore = analysisResult?.GrammarScore ?? session.GrammarScore,
        VocabularyScore = analysisResult?.VocabularyScore ?? session.VocabularyScore,
        FluencyScore = analysisResult?.FluencyScore ?? session.FluencyScore,
        OverallFeedback = analysisResult?.FeedbackSummary,
        GrammarReportJson = analysisResult?.Metrics,
        CreatedAt = session.CreatedAt,
        UpdatedAt = session.UpdatedAt
    };
}

