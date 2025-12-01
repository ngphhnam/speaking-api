namespace SpeakingPractice.Api.DTOs.Common;

public record UserDto(Guid Id, string Email, string FullName, IReadOnlyCollection<string> Roles);

