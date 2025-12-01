namespace SpeakingPractice.Api.DTOs.SpeakingSessions;

public class CreateSpeakingSessionRequest
{
    public string Topic { get; set; } = default!;
    public string Level { get; set; } = default!;
}

