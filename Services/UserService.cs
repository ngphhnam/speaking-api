using Microsoft.AspNetCore.Identity;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class UserService(UserManager<ApplicationUser> userManager) : IUserService
{
    public async Task<UserDto> MapToDtoAsync(ApplicationUser user, CancellationToken ct)
    {
        var roles = await userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault() ?? string.Empty;
        return new UserDto(user.Id, user.Email ?? string.Empty, user.FullName, role);
    }
}

