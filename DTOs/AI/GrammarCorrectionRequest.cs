using System.Text.Json.Serialization;

namespace SpeakingPractice.Api.DTOs.AI;

public class GrammarCorrectionRequest
{
    [JsonPropertyName("transcription")]
    public string Transcription { get; set; } = string.Empty;

    [JsonPropertyName("language")]
    public string Language { get; set; } = "en";

    [JsonPropertyName("questionText")]
    public string QuestionText { get; set; } = string.Empty;
}

