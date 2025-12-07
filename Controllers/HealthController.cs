using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Minio;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class HealthController(
    ApplicationDbContext dbContext,
    MinioClient minioClient,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration,
    ILogger<HealthController> logger) : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => this.ApiOk(new { status = "Healthy", timestamp = DateTimeOffset.UtcNow }, "Service is healthy");

    [HttpGet("dependencies")]
    public async Task<IActionResult> Dependencies(CancellationToken ct)
    {
        var results = new Dictionary<string, string>();

        results["database"] = await CheckAsync(async () =>
        {
            await dbContext.Database.CanConnectAsync(ct);
        });

        results["minio"] = await CheckAsync(async () =>
        {
            await minioClient.ListBucketsAsync(ct);
        });

        results["whisper"] = await CheckHttpAsync(configuration["Whisper:BaseUrl"], ct);
        results["llama"] = await CheckHttpAsync(configuration["Llama:BaseUrl"], ct);
        results["languageTool"] = await CheckHttpAsync(configuration["LanguageTool:BaseUrl"], ct);

        return this.ApiOk(results, "Dependencies checked successfully");
    }

    private async Task<string> CheckAsync(Func<Task> action)
    {
        try
        {
            await action();
            return "Healthy";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Dependency check failed");
            return "Unhealthy";
        }
    }

    private async Task<string> CheckHttpAsync(string? baseUrl, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            return "NotConfigured";
        }

        try
        {
            var client = httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(baseUrl);
            var response = await client.GetAsync(string.Empty, ct);
            return response.IsSuccessStatusCode ? "Healthy" : "Unhealthy";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Health check failed for {BaseUrl}", baseUrl);
            return "Unhealthy";
        }
    }
}

