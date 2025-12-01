using SpeakingPractice.Api.DTOs.Common;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IUserManagementService
{
    Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? role, string? search, CancellationToken ct);
    Task AssignRoleAsync(Guid userId, string role, CancellationToken ct);
    Task LockUserAsync(Guid userId, CancellationToken ct);
    Task UnlockUserAsync(Guid userId, CancellationToken ct);
}

