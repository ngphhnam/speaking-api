namespace SpeakingPractice.Api.DTOs.AI;

public class GrammarCorrectionResult
{
    public string Original { get; set; } = string.Empty;
    public string Corrected { get; set; } = string.Empty;
    public List<GrammarCorrection> Corrections { get; set; } = new();
    public string Explanation { get; set; } = string.Empty;
}

public class GrammarCorrection
{
    public string Original { get; set; } = string.Empty;
    public string Corrected { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}


