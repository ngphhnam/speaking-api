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
    
    // Separate retry policy for grammar correction - don't retry on 500 errors (backend issues)
    private readonly AsyncRetryPolicy _grammarRetryPolicy = Policy
        .Handle<HttpRequestException>(ex => 
            !ex.Data.Contains("SkipRetry") && // Don't retry if SkipRetry flag is set
            !ex.Message.Contains("500") && 
            !ex.Message.Contains("InternalServerError"))
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(1, _ => TimeSpan.FromSeconds(1)); // Only 1 retry for network issues, not backend errors

    public Task<LlamaScoreResult> ScoreAsync(string transcription, string questionText, string language, string feedbackLanguage, CancellationToken ct)
        => _retryPolicy.ExecuteAsync(async token =>
        {
            var payload = new
            {
                transcription,
                questionText,
                language,
                feedbackLanguage
            };

                var response = await httpClient.PostAsJsonAsync("/api/v2/score", payload, SerializerOptions, token);
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
            
            // For JsonElement, read as string first then parse to avoid deserialization issues
            // This allows flexible parsing of responses that may have varying structures
            if (typeof(T) == typeof(JsonElement))
            {
                try
                {
                    var jsonString = await response.Content.ReadAsStringAsync(token);
                    if (string.IsNullOrWhiteSpace(jsonString))
                        throw new InvalidOperationException("Received empty JSON response from Llama service");
                    
                    var jsonElement = JsonSerializer.Deserialize<JsonElement>(jsonString, SerializerOptions);
                    logger.LogInformation("Llama generation completed for task type: {TaskType}", taskType);
                    return (T)(object)jsonElement;
                }
                catch (JsonException ex)
                {
                    logger.LogError(ex, "Failed to deserialize JSON response for task type: {TaskType}", taskType);
                    throw new InvalidOperationException($"Failed to deserialize generation result: {ex.Message}", ex);
                }
            }
            
            var result = await response.Content.ReadFromJsonAsync<T>(SerializerOptions, token);
            logger.LogInformation("Llama generation completed for task type: {TaskType}", taskType);
            return result ?? throw new InvalidOperationException("Failed to deserialize generation result");
        }, ct);

    public Task<GrammarCorrectionResult> CorrectGrammarAsync(string transcription, string language, string questionText, CancellationToken ct)
        => _grammarRetryPolicy.ExecuteAsync(async token =>
        {
            var payload = new GrammarCorrectionRequest
            {
                Transcription = transcription,
                Language = language,
                QuestionText = questionText
            };

            // Log request payload for debugging
            var payloadJson = JsonSerializer.Serialize(payload, SerializerOptions);
            logger.LogInformation("Sending grammar correction request to /api/grammar/correct");
            logger.LogDebug("Request payload: {Payload}", payloadJson);
            logger.LogDebug("Transcription length: {Length}, Language: {Language}, QuestionText: {QuestionText}", 
                transcription.Length, language, questionText);

            var requestStartTime = DateTime.UtcNow;
            var response = await httpClient.PostAsJsonAsync("/api/v2/grammar/correct", payload, SerializerOptions, token);
            var requestDuration = (DateTime.UtcNow - requestStartTime).TotalSeconds;
            logger.LogInformation("Grammar correction request completed in {Duration}s with status {StatusCode}", 
                requestDuration, response.StatusCode);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(token);
                logger.LogError("Llama grammar correction service returned {StatusCode}: {Body}", response.StatusCode, errorContent);
                
                if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    throw new ExternalServiceUnavailableException("Llama grammar correction service");
                }
                
                // For 500 errors, throw a specific exception that won't be retried
                if (response.StatusCode == HttpStatusCode.InternalServerError)
                {
                    throw new HttpRequestException($"Grammar correction service returned 500 Internal Server Error: {errorContent}")
                    {
                        Data = { ["StatusCode"] = 500, ["SkipRetry"] = true }
                    };
                }
                
                throw new HttpRequestException($"Grammar correction service returned {response.StatusCode}: {errorContent}");
            }

            var result = await response.Content.ReadFromJsonAsync<GrammarCorrectionResult>(SerializerOptions, token);
            logger.LogInformation("Llama grammar correction completed");
            return result ?? throw new InvalidOperationException("Received empty grammar correction result from Llama service");
        }, ct);
}

