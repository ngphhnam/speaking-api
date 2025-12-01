using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Polly;
using Polly.Retry;
using SpeakingPractice.Api.DTOs.AI;
using SpeakingPractice.Api.Infrastructure.Exceptions;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public class LlamaClient(HttpClient httpClient, ILogger<LlamaClient> logger) : ILlamaClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public Task<LlamaScoreResult> ScoreAsync(string transcription, string questionText, string topic, string level, CancellationToken ct)
        => _retryPolicy.ExecuteAsync(async token =>
        {
            var payload = new
            {
                transcription,
                questionText,
                topic,
                level
            };

            var response = await httpClient.PostAsJsonAsync("/api/score", payload, SerializerOptions, token);
            if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                logger.LogWarning("Llama scoring service returned 503");
                throw new ExternalServiceUnavailableException("Llama scoring service");
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<LlamaScoreResult>(SerializerOptions, token);
            logger.LogInformation("Llama scoring completed");
            return result ?? throw new InvalidOperationException("Received empty scoring result from Llama service");
        }, ct);

    public Task<T> GenerateAsync<T>(string prompt, string taskType, Dictionary<string, object>? context = null, CancellationToken ct = default)
        => _retryPolicy.ExecuteAsync(async token =>
        {
            var payload = new
            {
                prompt = prompt,
                task_type = taskType,
                context = context,
                format = (object?)null
            };

            var response = await httpClient.PostAsJsonAsync("/api/generate", payload, SerializerOptions, token);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<T>(SerializerOptions, token);
            logger.LogInformation("Llama generation completed for task type: {TaskType}", taskType);
            return result ?? throw new InvalidOperationException("Failed to deserialize generation result");
        }, ct);
}

