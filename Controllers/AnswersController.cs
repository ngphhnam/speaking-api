using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.AI;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Exceptions;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;
using SpeakingPractice.Api.Services;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController(
    IQuestionRepository questionRepository,
    IMinioClientWrapper minioClient,
    IWhisperClient whisperClient,
    ILlamaClient llamaClient,
    IRecordingRepository recordingRepository,
    IAnalysisResultRepository analysisResultRepository,
    ISpeakingSessionRepository speakingSessionRepository,
    IStreakService streakService,
    IAchievementService achievementService,
    UserManager<ApplicationUser> userManager,
    ILogger<AnswersController> logger) : ControllerBase
{
    [HttpPost("submit")]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> SubmitAnswer(
        [FromForm] Guid questionId,
        [FromForm] IFormFile audio,
        CancellationToken ct = default)
    {
        if (audio is null || audio.Length == 0)
        {
            return this.ApiBadRequest(ErrorCodes.FILE_REQUIRED, "Audio file is required");
        }

        // Get question
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question is null)
        {
            return this.ApiNotFound(ErrorCodes.QUESTION_NOT_FOUND, $"Question with id {questionId} not found");
        }

        if (!question.IsActive)
        {
            return this.ApiBadRequest(ErrorCodes.QUESTION_NOT_ACTIVE, "Question is not active");
        }

        // Get user ID and verify user exists
        var userIdClaim = GetUserId();
        if (userIdClaim is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
        }

        var userId = userIdClaim.Value;
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not found");
        }

        // Check daily practice limit for free users before processing (24-hour rolling window)
        if (!IsPremiumUser(user))
        {
            const int freeUserDailyLimit = 5;
            var now = DateTimeOffset.UtcNow;
            var practiceCountInLast24Hours = await speakingSessionRepository.CountPracticeSessionsInLast24HoursAsync(userId, now, ct);

            if (practiceCountInLast24Hours >= freeUserDailyLimit)
            {
                // Find the oldest practice session in the last 24 hours to calculate when limit resets
                var oldestSession = await speakingSessionRepository.GetOldestSessionInLast24HoursAsync(userId, now, ct);
                var resetTime = oldestSession?.CreatedAt.AddHours(24) ?? now.AddHours(24);
                var hoursUntilReset = (resetTime - now).TotalHours;
                
                return this.ApiBadRequest(
                    ErrorCodes.DAILY_PRACTICE_LIMIT_REACHED,
                    $"Free users are limited to {freeUserDailyLimit} practice sessions per 24 hours. " +
                    $"You can practice again in approximately {Math.Ceiling(hoursUntilReset)} hours. Upgrade to Premium for unlimited access.");
            }
        }

        try
        {
            // Upload audio to MinIO
            await using var memoryStream = new MemoryStream();
            await audio.OpenReadStream().CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;

            var objectName = $"answers/{questionId}/{DateTimeOffset.UtcNow:yyyyMMdd}/{Guid.NewGuid()}_{audio.FileName}";
            var audioUrl = await minioClient.UploadAudioAsync(memoryStream, objectName, userId, ct);

            // Transcribe audio
            memoryStream.Position = 0;
            var transcription = await whisperClient.TranscribeAsync(memoryStream, audio.FileName, ct);

            // Score with Llama
            var llamaResult = await llamaClient.ScoreAsync(
                transcription.Text, 
                question.QuestionText,
                transcription.Language ?? "en",
                "vi",
                ct);

            // Correct grammar (optional - continue if it fails)
            GrammarCorrectionResult? grammarCorrection = null;
            try
            {
                grammarCorrection = await llamaClient.CorrectGrammarAsync(
                    transcription.Text,
                    transcription.Language ?? "en",
                    question.QuestionText,
                    ct);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Grammar correction failed for question {QuestionId}, continuing without correction", questionId);
                // Continue without grammar correction - use original transcription
            }

            // Create practice session for this attempt so recordings satisfy FK constraints
            var sessionTimestamp = DateTimeOffset.UtcNow;
            var session = new PracticeSession
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                TopicId = question.TopicId,
                SessionType = "question_attempt",
                QuestionsAttempted = 1,
                Status = "completed",
                StartedAt = sessionTimestamp,
                CompletedAt = sessionTimestamp,
                OverallBandScore = llamaResult.BandScore,
                FluencyScore = llamaResult.FluencyScore,
                VocabularyScore = llamaResult.VocabularyScore,
                GrammarScore = llamaResult.GrammarScore,
                PronunciationScore = llamaResult.PronunciationScore,
                CreatedAt = sessionTimestamp,
                UpdatedAt = sessionTimestamp
            };

            await speakingSessionRepository.AddAsync(session, ct);

            // Create Recording
            var recording = new Recording
            {
                Id = Guid.NewGuid(),
                SessionId = session.Id,
                UserId = userId,
                QuestionId = questionId,
                AudioUrl = audioUrl,
                AudioFormat = Path.GetExtension(audio.FileName).TrimStart('.'),
                FileSizeBytes = audio.Length,
                TranscriptionText = transcription.Text,
                TranscriptionLanguage = transcription.Language ?? "en",
                RefinedText = grammarCorrection?.Corrected,
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
                FeedbackSummary = llamaResult.OverallFeedback,
                Metrics = grammarCorrection != null 
                    ? System.Text.Json.JsonSerializer.Serialize(new
                    {
                        grammarCorrection = new
                        {
                            original = grammarCorrection.Original,
                            corrected = grammarCorrection.Corrected,
                            corrections = grammarCorrection.Corrections,
                            explanation = grammarCorrection.Explanation
                        }
                    }, new System.Text.Json.JsonSerializerOptions(System.Text.Json.JsonSerializerDefaults.Web)) // camelCase keys
                    : "{}",
                AnalyzedAt = DateTimeOffset.UtcNow,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await analysisResultRepository.AddAsync(analysisResult, ct);
            await recordingRepository.SaveChangesAsync(ct);

            // Update question statistics
            question.AttemptsCount++;
            var currentAvg = question.AvgScore ?? 0;
            var totalAttempts = question.AttemptsCount;
            question.AvgScore = ((currentAvg * (totalAttempts - 1)) + llamaResult.BandScore) / totalAttempts;
            question.UpdatedAt = DateTimeOffset.UtcNow;
            await questionRepository.UpdateAsync(question, ct);
            await questionRepository.SaveChangesAsync(ct);

            // Update streak when user completes a question (chỉ tăng khi hoàn thành câu hỏi)
            StreakUpdateResult? streakResult = null;
            try
            {
                streakResult = await streakService.UpdateStreakAsync(userId, null, ct);
                
                if (streakResult.IsNewRecord)
                {
                    logger.LogInformation(
                        "User {UserId} achieved new longest streak: {Streak} days!", 
                        userId, 
                        streakResult.CurrentStreak);
                }
                else if (streakResult.StreakRecovered)
                {
                    logger.LogInformation(
                        "User {UserId} recovered their streak: {Streak} days", 
                        userId, 
                        streakResult.CurrentStreak);
                }
                else if (streakResult.StreakContinued)
                {
                    logger.LogInformation(
                        "User {UserId} continued their streak: {Streak} days", 
                        userId, 
                        streakResult.CurrentStreak);
                }
            }
            catch (Exception ex)
            {
                // Don't fail the answer submission if streak update fails
                logger.LogError(ex, "Failed to update streak for user {UserId}", userId);
            }

            // Check and award achievements
            try
            {
                var awardedAchievements = await achievementService.CheckAndAwardAchievementsAsync(
                    userId, 
                    llamaResult.BandScore, 
                    ct);
                
                if (awardedAchievements.Count > 0)
                {
                    logger.LogInformation(
                        "User {UserId} earned {Count} achievement(s)",
                        userId,
                        awardedAchievements.Count);
                }
            }
            catch (Exception ex)
            {
                // Don't fail the answer submission if achievement check fails
                logger.LogError(ex, "Failed to check achievements for user {UserId}", userId);
            }

            logger.LogInformation("Submitted answer for question {QuestionId}, score: {Score}", questionId, llamaResult.BandScore);

            // Note: Translation removed from main flow to improve response time
            // Feedback is returned in English. Translation can be done client-side or via separate endpoint if needed.

            // Build streak info if streak was updated
            object? streakInfo = null;
            if (streakResult != null)
            {
                // Always return streak info so frontend can display current streak
                streakInfo = new
                {
                    currentStreak = streakResult.CurrentStreak,
                    longestStreak = streakResult.LongestStreak,
                    totalPracticeDays = streakResult.TotalPracticeDays,
                    isNewRecord = streakResult.IsNewRecord,
                    streakContinued = streakResult.StreakContinued,
                    streakRecovered = streakResult.StreakRecovered,
                    streakBroken = streakResult.StreakBroken
                };
            }

            var responseData = new
            {
                recordingId = recording.Id,
                analysisResultId = analysisResult.Id,
                transcription = transcription.Text,
                scores = new
                {
                    overallBandScore = llamaResult.BandScore,
                    fluencyScore = llamaResult.FluencyScore,
                    vocabularyScore = llamaResult.VocabularyScore,
                    grammarScore = llamaResult.GrammarScore,
                    pronunciationScore = llamaResult.PronunciationScore
                },
                feedback = llamaResult.OverallFeedback,
                grammarCorrection = grammarCorrection != null ? new
                {
                    original = grammarCorrection.Original,
                    corrected = grammarCorrection.Corrected,
                    corrections = grammarCorrection.Corrections,
                    explanation = grammarCorrection.Explanation
                } : null,
                sampleAnswers = question.SampleAnswers,
                keyVocabulary = question.KeyVocabulary,
                streak = streakInfo // Thêm thông tin streak vào response
            };

            return this.ApiOk(responseData, "Answer submitted successfully");
        }
        catch (ExternalServiceUnavailableException ex)
        {
            logger.LogWarning(ex, "Scoring service unavailable for question {QuestionId}", questionId);
            return this.ApiStatusCode(StatusCodes.Status503ServiceUnavailable, ErrorCodes.EXTERNAL_SERVICE_UNAVAILABLE, "Scoring service is temporarily unavailable. Please try again later.");
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            logger.LogWarning(ex, "Downstream service unavailable for question {QuestionId}", questionId);
            return this.ApiStatusCode(StatusCodes.Status503ServiceUnavailable, ErrorCodes.EXTERNAL_SERVICE_UNAVAILABLE, "Downstream service is temporarily unavailable. Please try again later.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing answer for question {QuestionId}", questionId);
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "An error occurred while processing your answer", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
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

