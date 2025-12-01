namespace SpeakingPractice.Api.DTOs.Generation;

public class SampleAnswersDto
{
    public List<SampleAnswer> SampleAnswers { get; set; } = new();
}

public class SampleAnswer
{
    public string Text { get; set; } = string.Empty;
    public decimal BandScore { get; set; }
    public AnswerHighlights Highlights { get; set; } = new();
}

public class AnswerHighlights
{
    public string[] Vocabulary { get; set; } = Array.Empty<string>();
    public string[] Grammar { get; set; } = Array.Empty<string>();
    public string Structure { get; set; } = string.Empty;
}

