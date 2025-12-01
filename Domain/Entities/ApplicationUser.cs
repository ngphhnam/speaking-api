using Microsoft.AspNetCore.Identity;

namespace SpeakingPractice.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public decimal? TargetBandScore { get; set; }
    public string? CurrentLevel { get; set; } // 'beginner', 'intermediate', 'advanced'
    public DateTime? ExamDate { get; set; }
    public string SubscriptionType { get; set; } = "free";
    public DateTime? SubscriptionExpiresAt { get; set; }
    public bool EmailVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
    public virtual ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
}

