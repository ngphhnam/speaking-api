using FluentValidation;
using SpeakingPractice.Api.DTOs.SpeakingSessions;

namespace SpeakingPractice.Api.Validators;

public class CreateSpeakingSessionRequestValidator : AbstractValidator<CreateSpeakingSessionRequest>
{
    public CreateSpeakingSessionRequestValidator()
    {
        RuleFor(x => x.Topic).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Level).NotEmpty().MaximumLength(50);
    }
}

