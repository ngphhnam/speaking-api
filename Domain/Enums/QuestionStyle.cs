namespace SpeakingPractice.Api.Domain.Enums;

/// <summary>
/// Question style or format
/// </summary>
public enum QuestionStyle
{
    /// <summary>
    /// Open-ended question (most common)
    /// </summary>
    OpenEnded = 1,

    /// <summary>
    /// Yes/No question
    /// </summary>
    YesNo = 2,

    /// <summary>
    /// Multiple choice question
    /// </summary>
    MultipleChoice = 3,

    /// <summary>
    /// Cue card format (Part 2 only)
    /// </summary>
    CueCard = 4,

    /// <summary>
    /// Opinion-based question
    /// </summary>
    Opinion = 5,

    /// <summary>
    /// Comparison question
    /// </summary>
    Comparison = 6,

    /// <summary>
    /// Prediction question
    /// </summary>
    Prediction = 7,

    /// <summary>
    /// Cause/Effect question
    /// </summary>
    CauseEffect = 8
}





