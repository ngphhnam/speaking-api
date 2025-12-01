using SpeakingPractice.Api.DTOs.Refinement;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IRefinementService
{
    Task<RefinementResult> RefineResponseAsync(Guid recordingId, RefinementRequest request, CancellationToken ct);
    Task<RefinementSuggestions> GetSuggestionsAsync(Guid recordingId, CancellationToken ct);
    Task<ComparisonResult> CompareVersionsAsync(string original, string refined, CancellationToken ct);
}

