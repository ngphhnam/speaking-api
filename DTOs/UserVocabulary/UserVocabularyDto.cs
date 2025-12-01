namespace SpeakingPractice.Api.DTOs.UserVocabulary;

public class UserVocabularyDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid VocabularyId { get; set; }
    public VocabularyDto Vocabulary { get; set; } = null!;
    public string LearningStatus { get; set; } = "learning";
    public DateTimeOffset? NextReviewAt { get; set; }
    public int ReviewCount { get; set; }
    public int SuccessCount { get; set; }
    public string? PersonalNotes { get; set; }
    public string? ExampleUsage { get; set; }
    public DateTimeOffset FirstEncounteredAt { get; set; }
    public DateTimeOffset? LastReviewedAt { get; set; }
    public DateTimeOffset? MasteredAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}

public class VocabularyDto
{
    public Guid Id { get; set; }
    public string Word { get; set; } = string.Empty;
    public string? Phonetic { get; set; }
    public string? PartOfSpeech { get; set; }
    public string DefinitionEn { get; set; } = string.Empty;
    public string? DefinitionVi { get; set; }
    public decimal? IeltsBandLevel { get; set; }
    public string[]? TopicCategories { get; set; }
    public string[]? ExampleSentences { get; set; }
    public string[]? Synonyms { get; set; }
    public string[]? Antonyms { get; set; }
    public string[]? Collocations { get; set; }
}

