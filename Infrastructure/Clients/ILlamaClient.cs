using SpeakingPractice.Api.DTOs.AI;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public interface ILlamaClient
{
    Task<LlamaScoreResult> ScoreAsync(string transcription, string questionText, string topic, string level, CancellationToken ct);
    Task<T> GenerateAsync<T>(string prompt, string taskType, Dictionary<string, object>? context = null, CancellationToken ct = default);
    Task<GrammarCorrectionResult> CorrectGrammarAsync(string transcription, string language, string questionText, CancellationToken ct);
}

