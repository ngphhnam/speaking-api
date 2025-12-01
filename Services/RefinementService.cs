using System.Text.Json;
using SpeakingPractice.Api.DTOs.Refinement;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class RefinementService(
    ILlamaClient llamaClient,
    IRecordingRepository recordingRepository,
    IAnalysisResultRepository analysisResultRepository,
    ILogger<RefinementService> logger) : IRefinementService
{
    public async Task<RefinementResult> RefineResponseAsync(Guid recordingId, RefinementRequest request, CancellationToken ct)
    {
        var recording = await recordingRepository.GetByIdAsync(recordingId, ct);
        if (recording == null)
            throw new InvalidOperationException($"Recording with id {recordingId} not found");

        if (string.IsNullOrEmpty(recording.TranscriptionText))
            throw new InvalidOperationException("Recording has no transcription text");

        var prompt = BuildRefinementPrompt(recording.TranscriptionText, request);
        var context = new Dictionary<string, object>
        {
            ["recordingId"] = recordingId.ToString(),
            ["originalText"] = recording.TranscriptionText
        };
        if (request.FocusAreas != null && request.FocusAreas.Length > 0)
            context["focusAreas"] = string.Join(", ", request.FocusAreas);
        if (request.TargetBandScore.HasValue)
            context["targetBandScore"] = request.TargetBandScore.Value;
        context["preserveOriginalStyle"] = request.PreserveOriginalStyle;

        var result = await llamaClient.GenerateAsync<RefinementResult>(prompt, "refine", context, ct);

        // Save refined text to recording
        recording.RefinedText = result.RefinedText;
        recording.RefinementMetadata = JsonSerializer.Serialize(new
        {
            improvements = result.Improvements,
            suggestions = result.Suggestions,
            bandScoreImprovement = result.BandScoreImprovement
        });
        await recordingRepository.UpdateAsync(recording, ct);
        await recordingRepository.SaveChangesAsync(ct);

        return result;
    }

    public async Task<RefinementSuggestions> GetSuggestionsAsync(Guid recordingId, CancellationToken ct)
    {
        var recording = await recordingRepository.GetByIdAsync(recordingId, ct);
        if (recording == null)
            throw new InvalidOperationException($"Recording with id {recordingId} not found");

        if (string.IsNullOrEmpty(recording.TranscriptionText))
            throw new InvalidOperationException("Recording has no transcription text");

        var analysisResult = await analysisResultRepository.GetByRecordingIdAsync(recordingId, ct);
        
        // If we have refinement suggestions in analysis result, return them
        if (analysisResult?.RefinementSuggestions != null)
        {
            try
            {
                var suggestions = JsonSerializer.Deserialize<RefinementSuggestions>(analysisResult.RefinementSuggestions);
                if (suggestions != null) return suggestions;
            }
            catch
            {
                // Fall through to generate new suggestions
            }
        }

        // Generate new suggestions
        var prompt = $"Analyze this IELTS speaking response and provide specific improvement suggestions:\n\n{recording.TranscriptionText}\n\n" +
                    "Return JSON with: grammarSuggestions, vocabularySuggestions, fluencySuggestions, pronunciationSuggestions (all as arrays of strings)";

        var context = new Dictionary<string, object>
        {
            ["recordingId"] = recordingId.ToString()
        };

        var result = await llamaClient.GenerateAsync<RefinementSuggestions>(prompt, "refine", context, ct);

        // Save to analysis result if it exists
        if (analysisResult != null)
        {
            analysisResult.RefinementSuggestions = JsonSerializer.Serialize(result);
            await analysisResultRepository.UpdateAsync(analysisResult, ct);
            await analysisResultRepository.SaveChangesAsync(ct);
        }

        return result;
    }

    public async Task<ComparisonResult> CompareVersionsAsync(string original, string refined, CancellationToken ct)
    {
        var prompt = $"Compare these two versions of an IELTS speaking response:\n\n" +
                    $"ORIGINAL:\n{original}\n\n" +
                    $"REFINED:\n{refined}\n\n" +
                    "Return JSON with: originalText, refinedText, highlights array (type, original, improved, explanation), summary";

        var context = new Dictionary<string, object>
        {
            ["originalText"] = original,
            ["refinedText"] = refined
        };

        var result = await llamaClient.GenerateAsync<ComparisonResult>(prompt, "compare", context, ct);
        return result;
    }

    private static string BuildRefinementPrompt(string originalText, RefinementRequest request)
    {
        var prompt = $"Refine and improve this IELTS speaking response while preserving the original style:\n\n{originalText}\n\n";
        
        if (request.FocusAreas != null && request.FocusAreas.Length > 0)
            prompt += $"Focus on: {string.Join(", ", request.FocusAreas)}\n";
        if (request.TargetBandScore.HasValue)
            prompt += $"Target Band Score: {request.TargetBandScore.Value}\n";
        prompt += $"Preserve Original Style: {request.PreserveOriginalStyle}\n\n";
        
        prompt += "Return JSON with: originalText, refinedText, improvements array (type, original, improved, explanation), suggestions array, bandScoreImprovement";
        
        return prompt;
    }
}

