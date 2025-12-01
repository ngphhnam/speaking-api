namespace SpeakingPractice.Api.Domain.Entities;

public class ApiUsageLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string ServiceName { get; set; } = string.Empty; // 'whisper', 'llama', 'languagetool'
    public string? Endpoint { get; set; }
    public long? RequestSizeBytes { get; set; }
    public long? ResponseSizeBytes { get; set; }
    public int? ProcessingTimeMs { get; set; }
    public decimal? EstimatedCost { get; set; }
    public int? StatusCode { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
}

