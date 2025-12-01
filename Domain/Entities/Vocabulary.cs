namespace SpeakingPractice.Api.Domain.Entities;

public class Vocabulary
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Word { get; set; } = string.Empty;
    public string? Phonetic { get; set; }
    public string? PartOfSpeech { get; set; }
    public string DefinitionEn { get; set; } = string.Empty;
    public string? DefinitionVi { get; set; }
    public decimal? IeltsBandLevel { get; set; }
    public string[]? TopicCategories { get; set; }
    public string[]? ExampleSentences { get; set; } // JSONB[]
    public string[]? Synonyms { get; set; }
    public string[]? Antonyms { get; set; }
    public string[]? Collocations { get; set; }
    public int UsageFrequency { get; set; } = 0;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ICollection<UserVocabulary> UserVocabularies { get; set; } = new List<UserVocabulary>();
}

