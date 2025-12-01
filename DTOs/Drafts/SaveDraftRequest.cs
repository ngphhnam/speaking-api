namespace SpeakingPractice.Api.DTOs.Drafts;

public class SaveDraftRequest
{
    public string DraftContent { get; set; } = string.Empty;
    public string? OutlineStructure { get; set; }
}

