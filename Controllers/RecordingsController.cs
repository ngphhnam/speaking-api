using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Recordings;
using SpeakingPractice.Api.DTOs.Refinement;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class RecordingsController(
    IRecordingRepository recordingRepository,
    IAnalysisResultRepository analysisResultRepository,
    IRefinementService refinementService,
    ILogger<RecordingsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? questionId,
        [FromQuery] Guid? sessionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        var isAdmin = User.IsInRole("Admin");
        var targetUserId = userId ?? requesterId.Value;

        if (!isAdmin && targetUserId != requesterId.Value)
        {
            return Forbid();
        }

        IReadOnlyCollection<Domain.Entities.Recording> recordings;

        if (questionId.HasValue)
        {
            recordings = await recordingRepository.GetByQuestionIdAsync(questionId.Value, ct);
        }
        else if (sessionId.HasValue)
        {
            recordings = await recordingRepository.GetBySessionIdAsync(sessionId.Value, ct);
        }
        else if (isAdmin && userId.HasValue)
        {
            recordings = await recordingRepository.GetByUserIdAsync(targetUserId, ct);
        }
        else
        {
            recordings = await recordingRepository.GetByUserIdAsync(requesterId.Value, ct);
        }

        return Ok(recordings.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var recording = await recordingRepository.GetByIdAsync(id, ct);
        if (recording is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (recording.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(MapToDto(recording));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct = default)
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

        var recordings = await recordingRepository.GetByUserIdAsync(userId, ct);
        return Ok(recordings.Select(MapToDto));
    }

    [HttpGet("question/{questionId:guid}")]
    public async Task<IActionResult> GetByQuestionId(Guid questionId, CancellationToken ct = default)
    {
        var recordings = await recordingRepository.GetByQuestionIdAsync(questionId, ct);
        return Ok(recordings.Select(MapToDto));
    }

    [HttpGet("session/{sessionId:guid}")]
    public async Task<IActionResult> GetBySessionId(Guid sessionId, CancellationToken ct = default)
    {
        var recordings = await recordingRepository.GetBySessionIdAsync(sessionId, ct);
        return Ok(recordings.Select(MapToDto));
    }

    [HttpGet("{id:guid}/analysis")]
    public async Task<IActionResult> GetAnalysis(Guid id, CancellationToken ct = default)
    {
        var recording = await recordingRepository.GetByIdAsync(id, ct);
        if (recording is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (recording.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var analysis = await analysisResultRepository.GetByRecordingIdAsync(id, ct);
        if (analysis is null)
        {
            return NotFound();
        }

        return Ok(new DTOs.AnalysisResults.AnalysisResultDto
        {
            Id = analysis.Id,
            RecordingId = analysis.RecordingId,
            UserId = analysis.UserId,
            OverallBandScore = analysis.OverallBandScore,
            FluencyScore = analysis.FluencyScore,
            VocabularyScore = analysis.VocabularyScore,
            GrammarScore = analysis.GrammarScore,
            PronunciationScore = analysis.PronunciationScore,
            FeedbackSummary = analysis.FeedbackSummary,
            Strengths = analysis.Strengths,
            Improvements = analysis.Improvements,
            GrammarIssues = analysis.GrammarIssues,
            PronunciationIssues = analysis.PronunciationIssues,
            VocabularySuggestions = analysis.VocabularySuggestions,
            AnalyzedAt = analysis.AnalyzedAt,
            CreatedAt = analysis.CreatedAt
        });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var recording = await recordingRepository.GetByIdAsync(id, ct);
        if (recording is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (recording.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        await recordingRepository.DeleteAsync(recording, ct);
        await recordingRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted recording {RecordingId}", id);
        return NoContent();
    }
    [HttpPost("{id:guid}/refine")]
    public async Task<IActionResult> RefineResponse(Guid id, [FromBody] RefinementRequest? request, CancellationToken ct = default)
    {
        try
        {
            request ??= new RefinementRequest();
            var result = await refinementService.RefineResponseAsync(id, request, ct);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation for recording {RecordingId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refining response for recording {RecordingId}", id);
            return StatusCode(500, new { error = "Failed to refine response", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/refinement-suggestions")]
    public async Task<IActionResult> GetRefinementSuggestions(Guid id, CancellationToken ct = default)
    {
        try
        {
            var suggestions = await refinementService.GetSuggestionsAsync(id, ct);
            return Ok(suggestions);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogWarning(ex, "Invalid operation for recording {RecordingId}", id);
            return NotFound(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting refinement suggestions for recording {RecordingId}", id);
            return StatusCode(500, new { error = "Failed to get refinement suggestions", message = ex.Message });
        }
    }

    [HttpPost("compare")]
    public async Task<IActionResult> CompareVersions([FromBody] CompareVersionsRequest request, CancellationToken ct = default)
    {
        try
        {
            var result = await refinementService.CompareVersionsAsync(request.OriginalText, request.RefinedText, ct);
            return Ok(result);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error comparing versions");
            return StatusCode(500, new { error = "Failed to compare versions", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/audio")]
    public async Task<IActionResult> DownloadAudio(Guid id, CancellationToken ct = default)
    {
        var recording = await recordingRepository.GetByIdAsync(id, ct);
        if (recording is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (recording.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        // Return redirect to audio URL or implement file download
        // For now, return the URL
        return Ok(new { audioUrl = recording.AudioUrl });
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static RecordingDto MapToDto(Domain.Entities.Recording recording)
    {
        return new RecordingDto
        {
            Id = recording.Id,
            SessionId = recording.SessionId,
            UserId = recording.UserId,
            QuestionId = recording.QuestionId,
            AudioUrl = recording.AudioUrl,
            AudioFormat = recording.AudioFormat,
            FileSizeBytes = recording.FileSizeBytes,
            DurationSeconds = recording.DurationSeconds,
            TranscriptionText = recording.TranscriptionText,
            TranscriptionLanguage = recording.TranscriptionLanguage,
            ProcessingStatus = recording.ProcessingStatus,
            RefinedText = recording.RefinedText,
            RecordedAt = recording.RecordedAt,
            ProcessedAt = recording.ProcessedAt,
            CreatedAt = recording.CreatedAt
        };
    }
}

