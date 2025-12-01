using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.AnalysisResults;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class AnalysisResultsController(
    IAnalysisResultRepository analysisResultRepository,
    IRecordingRepository recordingRepository,
    ILogger<AnalysisResultsController> logger) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var analysis = await analysisResultRepository.GetByIdAsync(id, ct);
        if (analysis is null)
        {
            return NotFound();
        }

        var requesterId = GetUserId();
        if (!requesterId.HasValue)
        {
            return Unauthorized();
        }

        if (analysis.UserId != requesterId.Value && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return Ok(MapToDto(analysis));
    }

    [HttpGet("recording/{recordingId:guid}")]
    public async Task<IActionResult> GetByRecordingId(Guid recordingId, CancellationToken ct = default)
    {
        var recording = await recordingRepository.GetByIdAsync(recordingId, ct);
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

        var analysis = await analysisResultRepository.GetByRecordingIdAsync(recordingId, ct);
        if (analysis is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(analysis));
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

        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
        return Ok(analyses.Select(MapToDto));
    }

    [HttpGet("user/{userId:guid}/statistics")]
    public async Task<IActionResult> GetUserStatistics(Guid userId, CancellationToken ct = default)
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

        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
        var analysesList = analyses.ToList();

        if (!analysesList.Any())
        {
            return Ok(new UserStatisticsDto
            {
                UserId = userId,
                TotalRecordings = 0
            });
        }

        var scores = analysesList
            .Where(a => a.OverallBandScore.HasValue)
            .Select(a => a.OverallBandScore!.Value)
            .ToList();

        var weakAreas = new List<string>();
        var strengths = new List<string>();

        var avgFluency = analysesList.Where(a => a.FluencyScore.HasValue).Average(a => a.FluencyScore!.Value);
        var avgVocabulary = analysesList.Where(a => a.VocabularyScore.HasValue).Average(a => a.VocabularyScore!.Value);
        var avgGrammar = analysesList.Where(a => a.GrammarScore.HasValue).Average(a => a.GrammarScore!.Value);
        var avgPronunciation = analysesList.Where(a => a.PronunciationScore.HasValue).Average(a => a.PronunciationScore!.Value);

        var allScores = new Dictionary<string, decimal>
        {
            { "Fluency", avgFluency },
            { "Vocabulary", avgVocabulary },
            { "Grammar", avgGrammar },
            { "Pronunciation", avgPronunciation }
        };

        var sortedScores = allScores.OrderBy(s => s.Value).ToList();
        if (sortedScores.Any())
        {
            weakAreas.AddRange(sortedScores.Take(2).Select(s => s.Key));
            strengths.AddRange(sortedScores.TakeLast(2).Select(s => s.Key));
        }

        var statistics = new UserStatisticsDto
        {
            UserId = userId,
            TotalRecordings = analysesList.Count,
            AvgOverallScore = scores.Any() ? scores.Average() : null,
            AvgFluencyScore = avgFluency > 0 ? avgFluency : null,
            AvgVocabularyScore = avgVocabulary > 0 ? avgVocabulary : null,
            AvgGrammarScore = avgGrammar > 0 ? avgGrammar : null,
            AvgPronunciationScore = avgPronunciation > 0 ? avgPronunciation : null,
            HighestScore = scores.Any() ? scores.Max() : null,
            LowestScore = scores.Any() ? scores.Min() : null,
            WeakAreas = weakAreas.ToArray(),
            Strengths = strengths.ToArray()
        };

        return Ok(statistics);
    }

    [HttpGet("user/{userId:guid}/trends")]
    public async Task<IActionResult> GetTrends(
        Guid userId,
        [FromQuery] int days = 30,
        CancellationToken ct = default)
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

        var cutoffDate = DateTimeOffset.UtcNow.AddDays(-days);
        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
        var recentAnalyses = analyses
            .Where(a => a.AnalyzedAt >= cutoffDate)
            .OrderBy(a => a.AnalyzedAt)
            .ToList();

        var trends = recentAnalyses.Select(a => new ScoreTrendDto
        {
            Date = a.AnalyzedAt,
            OverallScore = a.OverallBandScore,
            FluencyScore = a.FluencyScore,
            VocabularyScore = a.VocabularyScore,
            GrammarScore = a.GrammarScore,
            PronunciationScore = a.PronunciationScore
        }).ToList();

        return Ok(trends);
    }

    [HttpGet("user/{userId:guid}/weak-areas")]
    public async Task<IActionResult> GetWeakAreas(Guid userId, CancellationToken ct = default)
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

        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
        var analysesList = analyses.ToList();

        if (!analysesList.Any())
        {
            return Ok(new { weakAreas = Array.Empty<string>() });
        }

        var avgFluency = analysesList.Where(a => a.FluencyScore.HasValue).Average(a => a.FluencyScore!.Value);
        var avgVocabulary = analysesList.Where(a => a.VocabularyScore.HasValue).Average(a => a.VocabularyScore!.Value);
        var avgGrammar = analysesList.Where(a => a.GrammarScore.HasValue).Average(a => a.GrammarScore!.Value);
        var avgPronunciation = analysesList.Where(a => a.PronunciationScore.HasValue).Average(a => a.PronunciationScore!.Value);

        var allScores = new Dictionary<string, decimal>
        {
            { "Fluency", avgFluency },
            { "Vocabulary", avgVocabulary },
            { "Grammar", avgGrammar },
            { "Pronunciation", avgPronunciation }
        };

        var weakAreas = allScores
            .OrderBy(s => s.Value)
            .Take(2)
            .Select(s => s.Key)
            .ToArray();

        return Ok(new { weakAreas });
    }

    [HttpGet("user/{userId:guid}/strengths")]
    public async Task<IActionResult> GetStrengths(Guid userId, CancellationToken ct = default)
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

        var analyses = await analysisResultRepository.GetByUserIdAsync(userId, ct);
        var analysesList = analyses.ToList();

        if (!analysesList.Any())
        {
            return Ok(new { strengths = Array.Empty<string>() });
        }

        var avgFluency = analysesList.Where(a => a.FluencyScore.HasValue).Average(a => a.FluencyScore!.Value);
        var avgVocabulary = analysesList.Where(a => a.VocabularyScore.HasValue).Average(a => a.VocabularyScore!.Value);
        var avgGrammar = analysesList.Where(a => a.GrammarScore.HasValue).Average(a => a.GrammarScore!.Value);
        var avgPronunciation = analysesList.Where(a => a.PronunciationScore.HasValue).Average(a => a.PronunciationScore!.Value);

        var allScores = new Dictionary<string, decimal>
        {
            { "Fluency", avgFluency },
            { "Vocabulary", avgVocabulary },
            { "Grammar", avgGrammar },
            { "Pronunciation", avgPronunciation }
        };

        var strengths = allScores
            .OrderByDescending(s => s.Value)
            .Take(2)
            .Select(s => s.Key)
            .ToArray();

        return Ok(new { strengths });
    }

    private Guid? GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(userId, out var guid) ? guid : null;
    }

    private static AnalysisResultDto MapToDto(Domain.Entities.AnalysisResult analysis)
    {
        return new AnalysisResultDto
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
        };
    }
}
