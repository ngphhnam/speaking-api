namespace SpeakingPractice.Api.DTOs.Refinement;

public class CompareVersionsRequest
{
    public string OriginalText { get; set; } = string.Empty;
    public string RefinedText { get; set; } = string.Empty;
}

