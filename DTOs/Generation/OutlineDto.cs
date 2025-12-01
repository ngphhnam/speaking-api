namespace SpeakingPractice.Api.DTOs.Generation;

public class OutlineDto
{
    public OutlineContent Outline { get; set; } = default!;
    public int EstimatedDuration { get; set; }
    public string[] KeyPhrases { get; set; } = Array.Empty<string>();
}

public class OutlineContent
{
    public string Introduction { get; set; } = string.Empty;
    public List<OutlinePoint> MainPoints { get; set; } = new();
    public string Conclusion { get; set; } = string.Empty;
}

public class OutlinePoint
{
    public string Point { get; set; } = string.Empty;
    public string Details { get; set; } = string.Empty;
}

