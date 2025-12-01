namespace SpeakingPractice.Api.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? UserId { get; set; }
    public string Action { get; set; } = string.Empty; // 'user_login', 'recording_created', etc.
    public string? EntityType { get; set; } // 'user', 'recording', 'session'
    public Guid? EntityId { get; set; }
    public string? OldValues { get; set; } // JSONB
    public string? NewValues { get; set; } // JSONB
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser? User { get; set; }
}

