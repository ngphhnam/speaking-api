using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/api-usage-logs")]
public class ApiUsageLogController(
    IApiUsageLogRepository apiUsageLogRepository,
    ILogger<ApiUsageLogController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetApiUsageLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? serviceName = null,
        CancellationToken ct = default)
    {
        IEnumerable<Domain.Entities.ApiUsageLog> logs;

        if (userId.HasValue)
        {
            logs = await apiUsageLogRepository.GetByUserIdAsync(userId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(serviceName))
        {
            logs = await apiUsageLogRepository.GetByServiceAsync(serviceName, ct);
        }
        else
        {
            logs = await apiUsageLogRepository.GetAllAsync(page, pageSize, ct);
        }

        return Ok(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.ServiceName,
            log.Endpoint,
            log.RequestSizeBytes,
            log.ResponseSizeBytes,
            log.ProcessingTimeMs,
            log.EstimatedCost,
            log.StatusCode,
            log.ErrorMessage,
            log.CreatedAt
        }));
    }

    [HttpGet("service/{serviceName}")]
    public async Task<IActionResult> GetByService(string serviceName, CancellationToken ct = default)
    {
        var logs = await apiUsageLogRepository.GetByServiceAsync(serviceName, ct);
        return Ok(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.ServiceName,
            log.Endpoint,
            log.RequestSizeBytes,
            log.ResponseSizeBytes,
            log.ProcessingTimeMs,
            log.EstimatedCost,
            log.StatusCode,
            log.ErrorMessage,
            log.CreatedAt
        }));
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct = default)
    {
        var logs = await apiUsageLogRepository.GetByUserIdAsync(userId, ct);
        return Ok(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.ServiceName,
            log.Endpoint,
            log.RequestSizeBytes,
            log.ResponseSizeBytes,
            log.ProcessingTimeMs,
            log.EstimatedCost,
            log.StatusCode,
            log.ErrorMessage,
            log.CreatedAt
        }));
    }

    [HttpGet("statistics")]
    public async Task<IActionResult> GetStatistics(CancellationToken ct = default)
    {
        var allLogs = await apiUsageLogRepository.GetAllAsync(1, int.MaxValue, ct);
        var logsList = allLogs.ToList();

        var statistics = new
        {
            TotalRequests = logsList.Count,
            TotalCost = logsList.Where(l => l.EstimatedCost.HasValue).Sum(l => l.EstimatedCost!.Value),
            AvgProcessingTime = logsList.Where(l => l.ProcessingTimeMs.HasValue).Average(l => l.ProcessingTimeMs!.Value),
            TotalRequestSize = logsList.Where(l => l.RequestSizeBytes.HasValue).Sum(l => l.RequestSizeBytes!.Value),
            TotalResponseSize = logsList.Where(l => l.ResponseSizeBytes.HasValue).Sum(l => l.ResponseSizeBytes!.Value),
            ByService = logsList.GroupBy(l => l.ServiceName).Select(g => new
            {
                ServiceName = g.Key,
                Count = g.Count(),
                TotalCost = g.Where(l => l.EstimatedCost.HasValue).Sum(l => l.EstimatedCost!.Value),
                AvgProcessingTime = g.Where(l => l.ProcessingTimeMs.HasValue).Average(l => l.ProcessingTimeMs!.Value)
            })
        };

        return Ok(statistics);
    }

    [HttpGet("costs")]
    public async Task<IActionResult> GetCosts(CancellationToken ct = default)
    {
        var allLogs = await apiUsageLogRepository.GetAllAsync(1, int.MaxValue, ct);
        var logsList = allLogs.ToList();

        var costs = new
        {
            TotalCost = logsList.Where(l => l.EstimatedCost.HasValue).Sum(l => l.EstimatedCost!.Value),
            ByService = logsList
                .Where(l => l.EstimatedCost.HasValue)
                .GroupBy(l => l.ServiceName)
                .Select(g => new
                {
                    ServiceName = g.Key,
                    TotalCost = g.Sum(l => l.EstimatedCost!.Value),
                    RequestCount = g.Count()
                }),
            ByDate = logsList
                .Where(l => l.EstimatedCost.HasValue)
                .GroupBy(l => l.CreatedAt.Date)
                .Select(g => new
                {
                    Date = g.Key,
                    TotalCost = g.Sum(l => l.EstimatedCost!.Value),
                    RequestCount = g.Count()
                })
                .OrderByDescending(x => x.Date)
        };

        return Ok(costs);
    }
}






