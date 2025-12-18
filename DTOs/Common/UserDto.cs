namespace SpeakingPractice.Api.DTOs.Common;

public record UserDto(
    Guid Id, 
    string Email, 
    string FullName, 
    string Role, 
    string? Bio = null,
    string? AvatarUrl = null,
    string? Phone = null,
    DateOnly? DateOfBirth = null,
    decimal? TargetBandScore = null,
    string? CurrentLevel = null,
    DateOnly? ExamDate = null,
    int CurrentStreak = 0,
    int LongestStreak = 0,
    DateOnly? LastPracticeDate = null,
    int TotalPracticeDays = 0);
