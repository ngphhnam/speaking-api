using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Infrastructure.Persistence;

namespace SpeakingPractice.Api.Repositories;

public class ApiUsageLogRepository(ApplicationDbContext context) : IApiUsageLogRepository
{
    public async Task AddAsync(ApiUsageLog entity, CancellationToken ct)
        => await context.ApiUsageLogs.AddAsync(entity, ct);

    public async Task<IEnumerable<ApiUsageLog>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        => await context.ApiUsageLogs
            .AsNoTracking()
            .Where(log => log.UserId == userId)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<ApiUsageLog>> GetByServiceAsync(string serviceName, CancellationToken ct)
        => await context.ApiUsageLogs
            .AsNoTracking()
            .Where(log => log.ServiceName == serviceName)
            .OrderByDescending(log => log.CreatedAt)
            .ToListAsync(ct);

    public async Task<IEnumerable<ApiUsageLog>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        => await context.ApiUsageLogs
            .AsNoTracking()
            .OrderByDescending(log => log.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public Task SaveChangesAsync(CancellationToken ct) => context.SaveChangesAsync(ct);
}

