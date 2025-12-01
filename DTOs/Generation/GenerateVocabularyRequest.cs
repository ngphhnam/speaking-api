namespace SpeakingPractice.Api.DTOs.Generation;

public class GenerateVocabularyRequest
{
    public string? UserLevel { get; set; }
    public bool IncludeAdvanced { get; set; } = false;
}

