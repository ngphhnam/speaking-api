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
    ILogger<SpeakingSessionService> logger) : ISpeakingSessionService
{
    public async Task<SpeakingSessionDto> CreateSessionAsync(
        CreateSpeakingSessionRequest request,
        Stream audioStream,
        string fileName,
        Guid userId,
        CancellationToken ct)
    {
        await using var memoryStream = new MemoryStream();
        await audioStream.CopyToAsync(memoryStream, ct);
        memoryStream.Position = 0;

        var objectName = $"audio/{DateTimeOffset.UtcNow:yyyyMMdd}/{Guid.NewGuid()}_{fileName}";
        var audioUrl = await minioClient.UploadAudioAsync(memoryStream, objectName, userId, ct);

        memoryStream.Position = 0;
        var transcription = await whisperClient.TranscribeAsync(memoryStream, fileName, ct);

        var llamaResult = await llamaClient.ScoreAsync(transcription.Text, "", request.Topic, request.Level, ct);
        var grammarReport = await languageToolClient.CheckGrammarAsync(transcription.Text, ct);

        // Create PracticeSession
        var session = new PracticeSession
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            SessionType = "practice",
            Status = "completed",
            StartedAt = DateTimeOffset.UtcNow,
            CompletedAt = DateTimeOffset.UtcNow,
            QuestionsAttempted = 1,
            OverallBandScore = llamaResult.BandScore,
            FluencyScore = llamaResult.FluencyScore,
            VocabularyScore = llamaResult.VocabularyScore,
            GrammarScore = llamaResult.GrammarScore,
            PronunciationScore = llamaResult.PronunciationScore,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await sessionRepository.AddAsync(session, ct);

        // Create Recording
        var recording = new Recording
        {
            Id = Guid.NewGuid(),
            SessionId = session.Id,
            UserId = userId,
            AudioUrl = audioUrl,
            AudioFormat = Path.GetExtension(fileName).TrimStart('.'),
            TranscriptionText = transcription.Text,
            TranscriptionLanguage = transcription.Language ?? "en",
            ProcessingStatus = "completed",
            RecordedAt = DateTimeOffset.UtcNow,
            ProcessedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await recordingRepository.AddAsync(recording, ct);

        // Create AnalysisResult
        var analysisResult = new AnalysisResult
        {
            Id = Guid.NewGuid(),
            RecordingId = recording.Id,
            UserId = userId,
            OverallBandScore = llamaResult.BandScore,
            FluencyScore = llamaResult.FluencyScore,
            VocabularyScore = llamaResult.VocabularyScore,
            GrammarScore = llamaResult.GrammarScore,
            PronunciationScore = llamaResult.PronunciationScore,
            Metrics = grammarReport.RawJson ?? "{}",
            FeedbackSummary = llamaResult.OverallFeedback,
            AnalyzedAt = DateTimeOffset.UtcNow,
            CreatedAt = DateTimeOffset.UtcNow
        };

        await analysisResultRepository.AddAsync(analysisResult, ct);
        await sessionRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created practice session {SessionId} with recording {RecordingId}", session.Id, recording.Id);

        return MapToDto(session, recording, analysisResult);
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

