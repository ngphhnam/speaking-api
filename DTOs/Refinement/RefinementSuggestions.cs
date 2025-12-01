namespace SpeakingPractice.Api.DTOs.Refinement;

public class RefinementSuggestions
{
    public List<string> GrammarSuggestions { get; set; } = new();
    public List<string> VocabularySuggestions { get; set; } = new();
    public List<string> FluencySuggestions { get; set; } = new();
    public List<string> PronunciationSuggestions { get; set; } = new();
}

