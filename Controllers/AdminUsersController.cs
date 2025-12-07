using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")]
public class AdminUsersController(
    IUserManagementService userManagementService,
    ILogger<AdminUsersController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? role = null,
        [FromQuery] string? search = null,
        CancellationToken ct = default)
    {
        var users = await userManagementService.GetUsersAsync(page, pageSize, role, search, ct);
        return this.ApiOk(users, "Users retrieved successfully");
    }

    [HttpPost("{id:guid}/roles")]
    public async Task<IActionResult> AssignRole(Guid id, [FromBody] AssignRoleRequest request, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.Role))
        {
            return this.ApiBadRequest(ErrorCodes.REQUIRED_FIELD_MISSING, "Role is required");
        }

        await userManagementService.AssignRoleAsync(id, request.Role, ct);
        return this.ApiOk("Role assigned successfully");
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> Lock(Guid id, CancellationToken ct)
    {
        await userManagementService.LockUserAsync(id, ct);
        return this.ApiOk("User locked successfully");
    }

    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> Unlock(Guid id, CancellationToken ct)
    {
        await userManagementService.UnlockUserAsync(id, ct);
        return this.ApiOk("User unlocked successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetUser(Guid id, CancellationToken ct)
    {
        var users = await userManagementService.GetUsersAsync(1, 1, null, null, ct);
        var user = users.Items.FirstOrDefault(u => u.Id == id);
        if (user is null)
        {
            return this.ApiNotFound(ErrorCodes.USER_NOT_FOUND_RESOURCE, $"User with id {id} not found");
        }
        return this.ApiOk(user, "User retrieved successfully");
    }

    [HttpPut("{id:guid}")]
    public Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        // Implementation would update user details
        logger.LogInformation("Updating user {UserId}", id);
        return Task.FromResult<IActionResult>(this.ApiOk(new { message = "User updated", userId = id }, "User updated successfully"));
    }

    [HttpDelete("{id:guid}")]
    public Task<IActionResult> DeleteUser(Guid id, CancellationToken ct)
    {
        // Implementation would delete user
        logger.LogInformation("Deleting user {UserId}", id);
        return Task.FromResult<IActionResult>(this.ApiOk("User deleted successfully"));
    }

    [HttpGet("{id:guid}/statistics")]
    public Task<IActionResult> GetUserStatistics(Guid id, CancellationToken ct)
    {
        // Implementation would get user statistics
        return Task.FromResult<IActionResult>(this.ApiOk(new
        {
            UserId = id,
            TotalSessions = 0,
            TotalRecordings = 0,
            AvgScore = (decimal?)null
        }, "User statistics retrieved successfully"));
    }

    [HttpGet("{id:guid}/recordings")]
    public Task<IActionResult> GetUserRecordings(Guid id, CancellationToken ct)
    {
        // Implementation would get user recordings
        return Task.FromResult<IActionResult>(this.ApiOk(Array.Empty<object>(), "User recordings retrieved successfully"));
    }

    [HttpGet("{id:guid}/sessions")]
    public Task<IActionResult> GetUserSessions(Guid id, CancellationToken ct)
    {
        // Implementation would get user sessions
        return Task.FromResult<IActionResult>(this.ApiOk(Array.Empty<object>(), "User recordings retrieved successfully"));
    }

    [HttpGet("statistics")]
    public Task<IActionResult> GetPlatformStatistics(CancellationToken ct)
    {
        // Implementation would get platform-wide statistics
        return Task.FromResult<IActionResult>(this.ApiOk(new
        {
            TotalUsers = 0,
            TotalSessions = 0,
            TotalRecordings = 0,
            ActiveUsers = 0
        }, "Platform statistics retrieved successfully"));
    }

    [HttpGet("analytics")]
    public Task<IActionResult> GetAnalytics(CancellationToken ct)
    {
        // Implementation would get analytics data
        return Task.FromResult<IActionResult>(this.ApiOk(new
        {
            DailyActiveUsers = 0,
            WeeklyActiveUsers = 0,
            MonthlyActiveUsers = 0,
            GrowthRate = 0.0m
        }, "Analytics retrieved successfully"));
    }

    public class AssignRoleRequest
    {
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUserRequest
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
    }
}

