namespace SpeakingPractice.Api.Domain.Entities;

/// <summary>
/// Lưu lịch sử streak của người dùng để có thể xem lại các streak cũ
/// </summary>
public class StreakHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public int StreakLength { get; set; } // Độ dài streak
    public DateOnly StartDate { get; set; } // Ngày bắt đầu streak
    public DateOnly? EndDate { get; set; } // Ngày kết thúc streak (null nếu đang active)
    public bool IsActive { get; set; } = false; // Streak hiện tại
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset UpdatedAt { get; set; } = DateTimeOffset.UtcNow;
    
    // Navigation properties
    public virtual ApplicationUser User { get; set; } = default!;
}

