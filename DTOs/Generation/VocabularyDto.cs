namespace SpeakingPractice.Api.DTOs.Generation;

public class VocabularyDto
{
    public List<VocabularyItem> Vocabulary { get; set; } = new();
    public string[] Phrases { get; set; } = Array.Empty<string>();
    public string[] Collocations { get; set; } = Array.Empty<string>();
}

public class VocabularyItem
{
    public string Word { get; set; } = string.Empty;
    public string Definition { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
    public string? Pronunciation { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? Difficulty { get; set; }
}

