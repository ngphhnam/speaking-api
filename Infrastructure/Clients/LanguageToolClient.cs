using System.Net.Http.Json;
using System.Text.Json;
using Polly;
using Polly.Retry;
using SpeakingPractice.Api.DTOs.AI;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public class LanguageToolClient(HttpClient httpClient, ILogger<LanguageToolClient> logger) : ILanguageToolClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<HttpRequestException>()
        .Or<TaskCanceledException>()
        .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));

    public Task<GrammarReportResult> CheckGrammarAsync(string text, CancellationToken ct)
        => _retryPolicy.ExecuteAsync(async token =>
        {
            var payload = new
            {
                text,
                language = "en-US"
            };

            var response = await httpClient.PostAsJsonAsync("/v2/check/json", payload, SerializerOptions, token);

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(token);
                logger.LogError("LanguageTool returned {StatusCode}: {Body}", response.StatusCode, content);
                response.EnsureSuccessStatusCode(); // throw with status code info
            }

            var json = await response.Content.ReadAsStringAsync(token);
            logger.LogInformation("LanguageTool check completed");

            using var doc = JsonDocument.Parse(json);
            var matches = doc.RootElement.GetProperty("matches");
            var summary = $"found {matches.GetArrayLength()} issues";

            return new GrammarReportResult
            {
                Summary = summary,
                RawJson = json
            };
        }, ct);
}

