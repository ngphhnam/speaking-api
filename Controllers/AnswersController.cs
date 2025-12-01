using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.SpeakingSessions;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Infrastructure.Exceptions;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnswersController(
    IQuestionRepository questionRepository,
    IMinioClientWrapper minioClient,
    IWhisperClient whisperClient,
    ILlamaClient llamaClient,
    ILanguageToolClient languageToolClient,
    IRecordingRepository recordingRepository,
    IAnalysisResultRepository analysisResultRepository,
    ISpeakingSessionRepository speakingSessionRepository,
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
            return BadRequest("Audio file is required.");
        }

        // Get question
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question is null)
        {
            return NotFound($"Question with id {questionId} not found");
        }

        if (!question.IsActive)
        {
            return BadRequest("Question is not active");
        }

        // Get user ID (allow anonymous with Guid.Empty)
        var userId = GetUserId() ?? Guid.Empty;

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
            var topicTitle = question.Topic?.Title ?? "General";
            var level = question.EstimatedBandRequirement.HasValue && question.EstimatedBandRequirement.Value > 0
                ? GetLevelFromBand(question.EstimatedBandRequirement.Value) 
                : "intermediate";
            
            var llamaResult = await llamaClient.ScoreAsync(
                transcription.Text, 
                question.QuestionText,
                topicTitle, 
                level, 
                ct);

            // Check grammar
            var grammarReport = await languageToolClient.CheckGrammarAsync(transcription.Text, ct);

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
                Metrics = grammarReport.RawJson ?? "{}",
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

            logger.LogInformation("Submitted answer for question {QuestionId}, score: {Score}", questionId, llamaResult.BandScore);

            return Ok(new
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
                grammarReport = grammarReport.Summary,
                sampleAnswers = question.SampleAnswers,
                keyVocabulary = question.KeyVocabulary
            });
        }
        catch (ExternalServiceUnavailableException ex)
        {
            logger.LogWarning(ex, "Scoring service unavailable for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "Scoring service is temporarily unavailable. Please try again later."
            });
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            logger.LogWarning(ex, "Downstream service unavailable for question {QuestionId}", questionId);
            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                error = "Downstream service is temporarily unavailable. Please try again later."
            });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing answer for question {QuestionId}", questionId);
            return StatusCode(500, new { error = "An error occurred while processing your answer", message = ex.Message });
        }
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static string GetLevelFromBand(decimal bandScore)
    {
        return bandScore switch
        {
            < 4.0m => "beginner",
            < 6.0m => "intermediate",
            _ => "advanced"
        };
    }
}

