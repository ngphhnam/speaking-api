namespace SpeakingPractice.Api.DTOs.Refinement;

public class RefinementRequest
{
    public string[]? FocusAreas { get; set; }
    public decimal? TargetBandScore { get; set; }
    public bool PreserveOriginalStyle { get; set; } = true;
}

