namespace SpeakingPractice.Api.DTOs.Refinement;

public class RefinementResult
{
    public string OriginalText { get; set; } = string.Empty;
    public string RefinedText { get; set; } = string.Empty;
    public List<Improvement> Improvements { get; set; } = new();
    public string[] Suggestions { get; set; } = Array.Empty<string>();
    public decimal BandScoreImprovement { get; set; }
}

public class Improvement
{
    public string Type { get; set; } = string.Empty; // grammar, vocabulary, fluency, etc.
    public string Original { get; set; } = string.Empty;
    public string Improved { get; set; } = string.Empty;
    public string Explanation { get; set; } = string.Empty;
}

