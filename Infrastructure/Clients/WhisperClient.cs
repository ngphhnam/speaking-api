using System.Net.Http.Headers;
using System.Text.Json;
using Polly;
using Polly.Retry;
using SpeakingPractice.Api.DTOs.AI;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public class WhisperClient(HttpClient httpClient, ILogger<WhisperClient> logger) : IWhisperClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

    public async Task<TranscriptionResult> TranscribeAsync(Stream audioStream, string fileName, CancellationToken ct)
    {
        return await _retryPolicy.ExecuteAsync(async token =>
        {
            using var content = new MultipartFormDataContent();
            var streamContent = new StreamContent(audioStream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue("audio/wav");
            content.Add(streamContent, "file", fileName);

            var response = await httpClient.PostAsync("/transcribe", content, token);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync(token);
            logger.LogInformation("Whisper transcription completed");
            var result = JsonSerializer.Deserialize<TranscriptionResult>(json, SerializerOptions);
            return result ?? new TranscriptionResult();
        }, ct);
    }
}

