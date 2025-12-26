namespace SpeakingPractice.Api.Services.Interfaces;

/// <summary>
/// Service để kiểm tra và trao achievements cho người dùng
/// </summary>
public interface IAchievementService
{
    /// <summary>
    /// Kiểm tra và trao achievements khi user hoàn thành một câu hỏi
    /// </summary>
    Task<List<AchievementAwardedDto>> CheckAndAwardAchievementsAsync(
        Guid userId, 
        decimal? bandScore = null,
        CancellationToken ct = default);
    
    /// <summary>
    /// Tính toán và cập nhật level cho user dựa trên XP
    /// </summary>
    Task<LevelUpdateResult> UpdateUserLevelAsync(Guid userId, int pointsEarned, CancellationToken ct = default);
    
    /// <summary>
    /// Lấy thông tin level hiện tại của user
    /// </summary>
    Task<UserLevelInfo?> GetUserLevelInfoAsync(Guid userId, CancellationToken ct = default);
}

public class AchievementAwardedDto
{
    public Guid AchievementId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? BadgeIconUrl { get; set; }
    public int Points { get; set; }
    public bool IsNewLevel { get; set; }
    public int? NewLevel { get; set; }
}

public class LevelUpdateResult
{
    public int OldLevel { get; set; }
    public int NewLevel { get; set; }
    public int OldExperiencePoints { get; set; }
    public int NewExperiencePoints { get; set; }
    public bool LeveledUp { get; set; }
    public int PointsToNextLevel { get; set; }
}

public class UserLevelInfo
{
    public int Level { get; set; }
    public int ExperiencePoints { get; set; }
    public int TotalPoints { get; set; }
    public int PointsToNextLevel { get; set; }
    public int PointsForCurrentLevel { get; set; }
}



