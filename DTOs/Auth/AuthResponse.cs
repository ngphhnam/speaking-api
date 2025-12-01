using SpeakingPractice.Api.DTOs.Common;

namespace SpeakingPractice.Api.DTOs.Auth;

public class AuthResponse
{
    public string AccessToken { get; set; } = default!;
    public DateTimeOffset ExpiresAt { get; set; }
    public string RefreshToken { get; set; } = default!;
    public UserDto User { get; set; } = default!;
}

