using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IAuditLogRepository
{
    Task AddAsync(AuditLog entity, CancellationToken ct);
    Task<IEnumerable<AuditLog>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityType, Guid entityId, CancellationToken ct);
    Task<IEnumerable<AuditLog>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

