namespace SpeakingPractice.Api.DTOs.Generation;

public class StructuresDto
{
    public List<StructureItem> Structures { get; set; } = new();
    public string[] TransitionPhrases { get; set; } = Array.Empty<string>();
    public string[] Idioms { get; set; } = Array.Empty<string>();
}

public class StructureItem
{
    public string Pattern { get; set; } = string.Empty;
    public string[] Examples { get; set; } = Array.Empty<string>();
    public string Usage { get; set; } = string.Empty;
}

