using SpeakingPractice.Api.DTOs.Questions;

namespace SpeakingPractice.Api.DTOs.Topics;

/// <summary>
/// Request DTO for creating a Part 2 topic with Part 2 and Part 3 questions together
/// </summary>
public class CreateTopicWithQuestionsRequest
{
    // Topic information
    public string Title { get; set; } = default!;
    public string? Description { get; set; }
    public int PartNumber { get; set; } = 2; // Default to Part 2
    public string? DifficultyLevel { get; set; }
    public string? TopicCategory { get; set; }
    public string[]? Keywords { get; set; }

    // Part 2 Question (Cue Card) - Required for Part 2 topics
    public CreateQuestionRequest? Part2Question { get; set; }

    // Part 3 Questions - Optional, can add multiple
    public List<CreateQuestionRequest>? Part3Questions { get; set; }
}




