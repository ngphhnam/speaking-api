namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateOutlineRequest
{
    public string? UserLevel { get; set; }
    public OutlinePreferences? Preferences { get; set; }
}

public class OutlinePreferences
{
    public bool IncludeExamples { get; set; } = true;
    public string DetailLevel { get; set; } = "medium"; // low, medium, high
}

