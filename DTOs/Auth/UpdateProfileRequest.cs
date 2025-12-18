namespace SpeakingPractice.Api.DTOs.Auth;

public class UpdateProfileRequest
{
    public string? FullName { get; set; }
    public string? Bio { get; set; }
    public string? Phone { get; set; }
    public DateOnly? DateOfBirth { get; set; }
    public decimal? TargetBandScore { get; set; }
    public string? CurrentLevel { get; set; }
    public DateOnly? ExamDate { get; set; }
}

