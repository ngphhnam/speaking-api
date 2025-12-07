using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/admin/audit-logs")]
public class AuditLogController(
    IAuditLogRepository auditLogRepository,
    ILogger<AuditLogController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        [FromQuery] Guid? userId = null,
        [FromQuery] string? entityType = null,
        [FromQuery] Guid? entityId = null,
        CancellationToken ct = default)
    {
        IEnumerable<Domain.Entities.AuditLog> logs;

        if (userId.HasValue)
        {
            logs = await auditLogRepository.GetByUserIdAsync(userId.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(entityType) && entityId.HasValue)
        {
            logs = await auditLogRepository.GetByEntityAsync(entityType, entityId.Value, ct);
        }
        else
        {
            logs = await auditLogRepository.GetAllAsync(page, pageSize, ct);
        }

        return this.ApiOk(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.OldValues,
            log.NewValues,
            log.IpAddress,
            log.UserAgent,
            log.CreatedAt
        }), "Audit logs retrieved successfully");
    }

    [HttpGet("user/{userId:guid}")]
    public async Task<IActionResult> GetByUserId(Guid userId, CancellationToken ct = default)
    {
        var logs = await auditLogRepository.GetByUserIdAsync(userId, ct);
        return this.ApiOk(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.OldValues,
            log.NewValues,
            log.IpAddress,
            log.UserAgent,
            log.CreatedAt
        }), "Audit logs retrieved successfully");
    }

    [HttpGet("entity/{entityType}/{entityId:guid}")]
    public async Task<IActionResult> GetByEntity(string entityType, Guid entityId, CancellationToken ct = default)
    {
        var logs = await auditLogRepository.GetByEntityAsync(entityType, entityId, ct);
        return this.ApiOk(logs.Select(log => new
        {
            log.Id,
            log.UserId,
            log.Action,
            log.EntityType,
            log.EntityId,
            log.OldValues,
            log.NewValues,
            log.IpAddress,
            log.UserAgent,
            log.CreatedAt
        }), "Audit logs retrieved successfully");
    }
}






