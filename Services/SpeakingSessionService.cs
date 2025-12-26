using Microsoft.AspNetCore.Identity;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Exceptions;
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
        // Check daily practice limit for free users
        await CheckDailyPracticeLimitAsync(userId, ct);

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

        // Note: Streak chỉ được update khi user hoàn thành câu hỏi (trong AnswersController),
        // không update khi tạo session

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

    private async Task CheckDailyPracticeLimitAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            throw new InvalidOperationException($"User with id {userId} not found");
        }

        // Premium users have unlimited access
        if (IsPremiumUser(user))
        {
            return;
        }

        // Free users are limited to 5 practice sessions per 24 hours (rolling window)
        const int freeUserDailyLimit = 5;
        var now = DateTimeOffset.UtcNow;
        var practiceCountInLast24Hours = await sessionRepository.CountPracticeSessionsInLast24HoursAsync(userId, now, ct);

        if (practiceCountInLast24Hours >= freeUserDailyLimit)
        {
            // Find the oldest practice session in the last 24 hours to calculate when limit resets
            var oldestSession = await sessionRepository.GetOldestSessionInLast24HoursAsync(userId, now, ct);
            var resetTime = oldestSession?.CreatedAt.AddHours(24) ?? now.AddHours(24);
            var hoursUntilReset = (resetTime - now).TotalHours;
            
            throw new BusinessRuleException(
                ErrorCodes.DAILY_PRACTICE_LIMIT_REACHED,
                $"Free users are limited to {freeUserDailyLimit} practice sessions per 24 hours. " +
                $"You can practice again in approximately {Math.Ceiling(hoursUntilReset)} hours. Upgrade to Premium for unlimited access.");
        }
    }

    private static bool IsPremiumUser(ApplicationUser user)
    {
        // User is premium if subscription type is "premium" and subscription hasn't expired
        if (user.SubscriptionType?.ToLowerInvariant() != "premium")
        {
            return false;
        }

        // If no expiration date, subscription is permanent
        if (!user.SubscriptionExpiresAt.HasValue)
        {
            return true;
        }

        // Check if subscription is still valid
        return user.SubscriptionExpiresAt.Value > DateTime.UtcNow;
    }
}

