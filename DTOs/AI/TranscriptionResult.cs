namespace SpeakingPractice.Api.DTOs.AI;

public class TranscriptionResult
{
    public string Text { get; set; } = string.Empty;
    public string? Language { get; set; }
    public IReadOnlyCollection<TranscriptionSegment> Segments { get; set; } = Array.Empty<TranscriptionSegment>();
}

public class TranscriptionSegment
{
    public int Index { get; set; }
    public string Text { get; set; } = string.Empty;
    public double Start { get; set; }
    public double End { get; set; }
}

