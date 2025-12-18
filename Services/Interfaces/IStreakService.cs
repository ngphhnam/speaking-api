namespace SpeakingPractice.Api.Services.Interfaces;

/// <summary>
/// Service for managing user practice streaks
/// </summary>
public interface IStreakService
{
    /// <summary>
    /// Updates user streak when they complete a practice session
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="practiceDate">The date of practice (defaults to today)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Updated streak information</returns>
    Task<StreakUpdateResult> UpdateStreakAsync(Guid userId, DateOnly? practiceDate = null, CancellationToken ct = default);
    
    /// <summary>
    /// Gets current streak information for a user
    /// </summary>
    /// <param name="userId">The user ID</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Streak information</returns>
    Task<StreakInfo?> GetStreakInfoAsync(Guid userId, CancellationToken ct = default);
    
    /// <summary>
    /// Checks and resets expired streaks (for scheduled job)
    /// </summary>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Number of streaks reset</returns>
    Task<int> ResetExpiredStreaksAsync(CancellationToken ct = default);
}

public class StreakUpdateResult
{
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public int TotalPracticeDays { get; set; }
    public bool IsNewRecord { get; set; }
    public bool StreakContinued { get; set; }
    public bool StreakBroken { get; set; }
}

public class StreakInfo
{
    public Guid UserId { get; set; }
    public int CurrentStreak { get; set; }
    public int LongestStreak { get; set; }
    public DateOnly? LastPracticeDate { get; set; }
    public int TotalPracticeDays { get; set; }
}




