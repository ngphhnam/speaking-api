namespace SpeakingPractice.Api.DTOs.UserVocabulary;

public class AddUserVocabularyRequest
{
    public Guid VocabularyId { get; set; }
    public string? PersonalNotes { get; set; }
}

