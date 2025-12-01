using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class AuditLogRepository(ApplicationDbContext context) : IAuditLogRepository
{
    public async Task AddAsync(AuditLog entity, CancellationToken ct)
        => await context.AuditLogs.AddAsync(entity, ct);

    public async Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(al => al.UserId == userId)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct)
        => await context.AuditLogs
            .AsNoTracking()
            .Where(al => al.EntityType == entityType && al.EntityId == entityId)
            .OrderByDescending(al => al.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        => await context.AuditLogs
            .AsNoTracking()
            .OrderByDescending(al => al.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

