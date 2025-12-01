using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Repositories;

public interface IApiUsageLogRepository
{
    Task AddAsync(ApiUsageLog entity, CancellationToken ct);
    Task<IEnumerable<ApiUsageLog>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<IEnumerable<ApiUsageLog>> GetByServiceAsync(string serviceName, CancellationToken ct);
    Task<IEnumerable<ApiUsageLog>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}

