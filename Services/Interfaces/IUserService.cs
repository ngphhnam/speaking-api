using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Common;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> MapToDtoAsync(ApplicationUser user, CancellationToken ct);
}

