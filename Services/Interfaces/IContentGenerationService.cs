using SpeakingPractice.Api.DTOs.Generation;
using SpeakingPractice.Api.DTOs.Questions;
using SpeakingPractice.Api.DTOs.Topics;

namespace SpeakingPractice.Api.Services.Interfaces;

public interface IContentGenerationService
{
    Task<IEnumerable<TopicDto>> GenerateTopicsAsync(GenerateTopicsRequest request, CancellationToken ct);
    Task<IEnumerable<QuestionDto>> GenerateQuestionsAsync(GenerateQuestionsRequest request, CancellationToken ct);
    Task<TopicDto> GenerateTopicWithQuestionsAsync(GenerateTopicWithQuestionsRequest request, CancellationToken ct);
    Task<OutlineDto> GenerateOutlineAsync(Guid questionId, GenerateOutlineRequest request, CancellationToken ct);
    Task<VocabularyDto> GenerateVocabularyAsync(Guid questionId, GenerateVocabularyRequest request, CancellationToken ct);
    Task<StructuresDto> GenerateStructuresAsync(Guid questionId, GenerateStructuresRequest request, CancellationToken ct);
}

