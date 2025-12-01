namespace SpeakingPractice.Api.DTOs.Vocabulary;

public class UpdateVocabularyRequest
{
    public string? Word { get; set; }
    public string? Phonetic { get; set; }
    public string? PartOfSpeech { get; set; }
    public string? DefinitionEn { get; set; }
    public string? DefinitionVi { get; set; }
    public decimal? IeltsBandLevel { get; set; }
    public string[]? TopicCategories { get; set; }
    public string[]? ExampleSentences { get; set; }
    public string[]? Synonyms { get; set; }
    public string[]? Antonyms { get; set; }
    public string[]? Collocations { get; set; }
}

