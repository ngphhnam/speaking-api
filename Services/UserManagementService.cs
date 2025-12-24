using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class UserManagementService(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole<Guid>> roleManager) : IUserManagementService
{
    public async Task<PagedResult<UserDto>> GetUsersAsync(int page, int pageSize, string? role, string? search, CancellationToken ct)
    {
        var query = userManager.Users.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(u =>
                (u.Email != null && EF.Functions.ILike(u.Email, $"%{search}%")) ||
                EF.Functions.ILike(u.FullName, $"%{search}%"));
        }

        if (!string.IsNullOrWhiteSpace(role))
        {
            var inRole = await userManager.GetUsersInRoleAsync(role);
            var ids = inRole.Select(u => u.Id).ToHashSet();
            query = query.Where(u => ids.Contains(u.Id));
        }

        var total = await query.CountAsync(ct);
        var users = await query.OrderByDescending(u => u.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        var items = new List<UserDto>();
        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            var primaryRole = roles.FirstOrDefault() ?? string.Empty;
            items.Add(new UserDto(
                Id: user.Id,
                Email: user.Email ?? string.Empty,
                FullName: user.FullName,
                Role: primaryRole,
                Bio: user.Bio,
                AvatarUrl: user.AvatarUrl,
                Phone: user.Phone,
                DateOfBirth: user.DateOfBirth,
                TargetBandScore: user.TargetBandScore,
                CurrentLevel: user.CurrentLevel,
                ExamDate: user.ExamDate,
                CurrentStreak: user.CurrentStreak,
                LongestStreak: user.LongestStreak,
                LastPracticeDate: user.LastPracticeDate,
                TotalPracticeDays: user.TotalPracticeDays,
                SubscriptionType: user.SubscriptionType,
                SubscriptionExpiresAt: user.SubscriptionExpiresAt,
                SubscriptionPlanCode: user.SubscriptionPlanCode,
                SubscriptionPlanDays: user.SubscriptionPlanDays));
        }

        return new PagedResult<UserDto>(items, page, pageSize, total);
    }

    public async Task AssignRoleAsync(Guid userId, string role, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("User not found");

        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<Guid>(role));
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }

    public async Task LockUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("User not found");
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow.AddYears(100));
    }

    public async Task UnlockUserAsync(Guid userId, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ??
                   throw new InvalidOperationException("User not found");
        await userManager.SetLockoutEndDateAsync(user, DateTimeOffset.UtcNow);
    }
}

