namespace SpeakingPractice.Api.DTOs.Refinement;

public class ComparisonResult
{
    public string OriginalText { get; set; } = string.Empty;
    public string RefinedText { get; set; } = string.Empty;
    public List<ComparisonHighlight> Highlights { get; set; } = new();
    public string Summary { get; set; } = string.Empty;
}

public class ComparisonHighlight
{
    public string Type { get; set; } = string.Empty;
    public string Original { get; set; } = string.Empty;
    public string Improved { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

