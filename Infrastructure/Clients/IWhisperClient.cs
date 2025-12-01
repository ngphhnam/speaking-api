using SpeakingPractice.Api.DTOs.AI;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public interface IWhisperClient
{
    Task<TranscriptionResult> TranscribeAsync(Stream audioStream, string fileName, CancellationToken ct);
}

