using Microsoft.AspNetCore.Identity;

namespace SpeakingPractice.Api.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = default!;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? TargetBandScore { get; set; }
    public string? CurrentLevel { get; set; } // 'beginner', 'intermediate', 'advanced'
    public DateOnly? ExamDate { get; set; }
    public string SubscriptionType { get; set; } = "free";
    public DateTime? SubscriptionExpiresAt { get; set; }
    public string? SubscriptionPlanCode { get; set; }
    public int? SubscriptionPlanDays { get; set; }
    public bool EmailVerified { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    
    // Streak tracking
    public int CurrentStreak { get; set; } = 0;
    public int LongestStreak { get; set; } = 0;
    public DateOnly? LastPracticeDate { get; set; }
    public int TotalPracticeDays { get; set; } = 0;
    
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;

    // Navigation properties
    public virtual ICollection<PracticeSession> PracticeSessions { get; set; } = new List<PracticeSession>();
    public virtual ICollection<Recording> Recordings { get; set; } = new List<Recording>();
    public virtual ICollection<AnalysisResult> AnalysisResults { get; set; } = new List<AnalysisResult>();
}

