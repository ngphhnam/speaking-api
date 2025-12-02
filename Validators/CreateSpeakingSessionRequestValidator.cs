using FluentValidation;
using SpeakingPractice.Api.DTOs.SpeakingSessions;

namespace SpeakingPractice.Api.Validators;

public class CreateSpeakingSessionRequestValidator : AbstractValidator<CreateSpeakingSessionRequest>
{
    public CreateSpeakingSessionRequestValidator()
    {
        RuleFor(x => x.TopicId).NotEmpty();
    }
}

