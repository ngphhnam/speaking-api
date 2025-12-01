using SpeakingPractice.Api.DTOs.AI;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public interface ILanguageToolClient
{
    Task<GrammarReportResult> CheckGrammarAsync(string text, CancellationToken ct);
}

