using System.Text.Json;
using SpeakingPractice.Api.DTOs.Generation;
using SpeakingPractice.Api.DTOs.Questions;
using SpeakingPractice.Api.DTOs.Topics;
using SpeakingPractice.Api.Infrastructure.Clients;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Services;

public class ContentGenerationService(
    ILlamaClient llamaClient,
    ITopicRepository topicRepository,
    IQuestionRepository questionRepository,
    ILogger<ContentGenerationService> logger) : IContentGenerationService
{
    public async Task<IEnumerable<TopicDto>> GenerateTopicsAsync(GenerateTopicsRequest request, CancellationToken ct)
    {
        var prompt = BuildTopicsPrompt(request);
        var context = new Dictionary<string, object>();
        if (request.PartNumber.HasValue) context["partNumber"] = request.PartNumber.Value;
        if (!string.IsNullOrEmpty(request.DifficultyLevel)) context["difficultyLevel"] = request.DifficultyLevel;
        if (!string.IsNullOrEmpty(request.TopicCategory)) context["topicCategory"] = request.TopicCategory;
        if (request.Keywords != null && request.Keywords.Length > 0) context["keywords"] = string.Join(", ", request.Keywords);

        var resultJson = await llamaClient.GenerateAsync<JsonElement>(prompt, "topics", context, ct);
        
        var topics = new List<TopicDto>();
        if (resultJson.ValueKind == JsonValueKind.Object && resultJson.TryGetProperty("topics", out var topicsArray))
        {
            foreach (var topicElement in topicsArray.EnumerateArray())
            {
                var topic = JsonSerializer.Deserialize<TopicDto>(topicElement.GetRawText());
                if (topic != null) topics.Add(topic);
            }
        }

        return topics;
    }

    public async Task<IEnumerable<QuestionDto>> GenerateQuestionsAsync(GenerateQuestionsRequest request, CancellationToken ct)
    {
        var topic = await topicRepository.GetByIdAsync(request.TopicId, ct);
        if (topic == null)
            throw new InvalidOperationException($"Topic with id {request.TopicId} not found");

        var prompt = BuildQuestionsPrompt(request, topic);
        var context = new Dictionary<string, object>
        {
            ["topicId"] = request.TopicId.ToString(),
            ["topicTitle"] = topic.Title
        };

        var resultJson = await llamaClient.GenerateAsync<JsonElement>(prompt, "questions", context, ct);
        
        var questions = new List<QuestionDto>();
        if (resultJson.ValueKind == JsonValueKind.Object && resultJson.TryGetProperty("questions", out var questionsArray))
        {
            foreach (var questionElement in questionsArray.EnumerateArray())
            {
                var question = JsonSerializer.Deserialize<QuestionDto>(questionElement.GetRawText());
                if (question != null)
                {
                    question.TopicId = request.TopicId;
                    questions.Add(question);
                }
            }
        }

        return questions;
    }

    public async Task<TopicDto> GenerateTopicWithQuestionsAsync(GenerateTopicWithQuestionsRequest request, CancellationToken ct)
    {
        var prompt = BuildTopicWithQuestionsPrompt(request);
        var context = new Dictionary<string, object>
        {
            ["topicTitle"] = request.TopicTitle,
            ["questionCount"] = request.QuestionCount
        };
        if (request.PartNumber.HasValue) context["partNumber"] = request.PartNumber.Value;
        if (!string.IsNullOrEmpty(request.DifficultyLevel)) context["difficultyLevel"] = request.DifficultyLevel;

        var resultJson = await llamaClient.GenerateAsync<JsonElement>(prompt, "topics", context, ct);
        
        var topic = JsonSerializer.Deserialize<TopicDto>(resultJson.GetRawText());
        
        if (topic == null)
            throw new InvalidOperationException("Failed to generate topic");

        return topic;
    }

    public async Task<OutlineDto> GenerateOutlineAsync(Guid questionId, GenerateOutlineRequest request, CancellationToken ct)
    {
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question == null)
            throw new InvalidOperationException($"Question with id {questionId} not found");

        var prompt = BuildOutlinePrompt(question, request);
        var context = new Dictionary<string, object>
        {
            ["questionId"] = questionId.ToString(),
            ["questionText"] = question.QuestionText
        };
        if (!string.IsNullOrEmpty(request.UserLevel)) context["userLevel"] = request.UserLevel;

        var resultJson = await llamaClient.GenerateAsync<JsonElement>(prompt, "outline", context, ct);
        
        // Parse the JSON flexibly, handling cases where introduction might be an object or string
        var outline = new OutlineDto();
        
        if (resultJson.ValueKind == JsonValueKind.Object)
        {
            // Parse outline
            if (resultJson.TryGetProperty("outline", out var outlineElement) && outlineElement.ValueKind == JsonValueKind.Object)
            {
                var outlineContent = new OutlineContent();
                
                // Handle introduction - could be string or object
                if (outlineElement.TryGetProperty("introduction", out var introElement))
                {
                    if (introElement.ValueKind == JsonValueKind.String)
                    {
                        outlineContent.Introduction = introElement.GetString() ?? string.Empty;
                    }
                    else if (introElement.ValueKind == JsonValueKind.Object)
                    {
                        // Try to extract text from common properties
                        if (introElement.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                            outlineContent.Introduction = textProp.GetString() ?? string.Empty;
                        else if (introElement.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.String)
                            outlineContent.Introduction = contentProp.GetString() ?? string.Empty;
                        else
                            outlineContent.Introduction = introElement.GetRawText(); // Fallback: serialize object to JSON string
                    }
                }
                
                // Parse mainPoints
                if (outlineElement.TryGetProperty("mainPoints", out var mainPointsElement) && mainPointsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var pointElement in mainPointsElement.EnumerateArray())
                    {
                        var point = JsonSerializer.Deserialize<OutlinePoint>(pointElement.GetRawText());
                        if (point != null)
                            outlineContent.MainPoints.Add(point);
                    }
                }
                
                // Parse conclusion - could be string or object
                if (outlineElement.TryGetProperty("conclusion", out var conclusionElement))
                {
                    if (conclusionElement.ValueKind == JsonValueKind.String)
                    {
                        outlineContent.Conclusion = conclusionElement.GetString() ?? string.Empty;
                    }
                    else if (conclusionElement.ValueKind == JsonValueKind.Object)
                    {
                        // Try to extract text from common properties
                        if (conclusionElement.TryGetProperty("text", out var textProp) && textProp.ValueKind == JsonValueKind.String)
                            outlineContent.Conclusion = textProp.GetString() ?? string.Empty;
                        else if (conclusionElement.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.String)
                            outlineContent.Conclusion = contentProp.GetString() ?? string.Empty;
                        else
                            outlineContent.Conclusion = conclusionElement.GetRawText(); // Fallback: serialize object to JSON string
                    }
                }
                
                outline.Outline = outlineContent;
            }
            
            // Parse estimatedDuration
            if (resultJson.TryGetProperty("estimatedDuration", out var durationElement))
            {
                if (durationElement.ValueKind == JsonValueKind.Number)
                    outline.EstimatedDuration = durationElement.GetInt32();
            }
            
            // Parse keyPhrases
            if (resultJson.TryGetProperty("keyPhrases", out var phrasesElement) && phrasesElement.ValueKind == JsonValueKind.Array)
            {
                var phrases = new List<string>();
                foreach (var phraseElement in phrasesElement.EnumerateArray())
                {
                    if (phraseElement.ValueKind == JsonValueKind.String)
                        phrases.Add(phraseElement.GetString() ?? string.Empty);
                }
                outline.KeyPhrases = phrases.ToArray();
            }
        }
        
        return outline;
    }

    public async Task<VocabularyDto> GenerateVocabularyAsync(Guid questionId, GenerateVocabularyRequest request, CancellationToken ct)
    {
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question == null)
            throw new InvalidOperationException($"Question with id {questionId} not found");

        var prompt = BuildVocabularyPrompt(question, request);
        var context = new Dictionary<string, object>
        {
            ["questionId"] = questionId.ToString(),
            ["questionText"] = question.QuestionText
        };
        if (!string.IsNullOrEmpty(request.UserLevel)) context["userLevel"] = request.UserLevel;
        context["includeAdvanced"] = request.IncludeAdvanced;

        var result = await llamaClient.GenerateAsync<VocabularyDto>(prompt, "vocabulary", context, ct);
        return result;
    }

    public async Task<StructuresDto> GenerateStructuresAsync(Guid questionId, GenerateStructuresRequest request, CancellationToken ct)
    {
        var question = await questionRepository.GetByIdAsync(questionId, ct);
        if (question == null)
            throw new InvalidOperationException($"Question with id {questionId} not found");

        var prompt = BuildStructuresPrompt(question, request);
        var context = new Dictionary<string, object>
        {
            ["questionId"] = questionId.ToString(),
            ["questionText"] = question.QuestionText
        };
        if (!string.IsNullOrEmpty(request.UserLevel)) context["userLevel"] = request.UserLevel;
        if (request.FocusAreas != null && request.FocusAreas.Length > 0) context["focusAreas"] = string.Join(", ", request.FocusAreas);

        var result = await llamaClient.GenerateAsync<StructuresDto>(prompt, "structures", context, ct);
        return result;
    }

    private static string BuildTopicsPrompt(GenerateTopicsRequest request)
    {
        var parts = new List<string>
        {
            $"Generate {request.Count} IELTS speaking topics.",
            "Return a JSON object with a 'topics' array."
        };

        if (request.PartNumber.HasValue)
            parts.Add($"Part Number: {request.PartNumber.Value}");
        if (!string.IsNullOrEmpty(request.DifficultyLevel))
            parts.Add($"Difficulty Level: {request.DifficultyLevel}");
        if (!string.IsNullOrEmpty(request.TopicCategory))
            parts.Add($"Category: {request.TopicCategory}");
        if (request.Keywords != null && request.Keywords.Length > 0)
            parts.Add($"Keywords: {string.Join(", ", request.Keywords)}");

        parts.Add("Each topic should have: title, description, partNumber, difficultyLevel, topicCategory, keywords");
        parts.Add("Return JSON format: {\"topics\": [{\"title\": \"...\", \"description\": \"...\", ...}]}");

        return string.Join("\n", parts);
    }

    private static string BuildQuestionsPrompt(GenerateQuestionsRequest request, Domain.Entities.Topic topic)
    {
        return $"Generate {request.Count} IELTS speaking questions for topic: {topic.Title}\n" +
               $"Topic Description: {topic.Description ?? "N/A"}\n" +
               "Each question should include: questionText, suggestedStructure, sampleAnswers (array), keyVocabulary (array)\n" +
               "Return JSON format: {\"questions\": [{...}]}";
    }

    private static string BuildTopicWithQuestionsPrompt(GenerateTopicWithQuestionsRequest request)
    {
        return $"Generate an IELTS speaking topic with title '{request.TopicTitle}' and {request.QuestionCount} questions.\n" +
               (request.PartNumber.HasValue ? $"Part Number: {request.PartNumber.Value}\n" : "") +
               (request.DifficultyLevel != null ? $"Difficulty Level: {request.DifficultyLevel}\n" : "") +
               "Return JSON with topic details and a 'questions' array containing all questions.";
    }

    private static string BuildOutlinePrompt(Domain.Entities.Question question, GenerateOutlineRequest request)
    {
        var prompt = $"Generate a speaking outline for this IELTS question: {question.QuestionText}\n";
        if (request.UserLevel != null)
            prompt += $"User Level: {request.UserLevel}\n";
        if (request.Preferences != null)
        {
            prompt += $"Include Examples: {request.Preferences.IncludeExamples}\n";
            prompt += $"Detail Level: {request.Preferences.DetailLevel}\n";
        }
        prompt += "Return JSON with: outline (introduction, mainPoints array, conclusion), estimatedDuration, keyPhrases array";
        return prompt;
    }

    private static string BuildVocabularyPrompt(Domain.Entities.Question question, GenerateVocabularyRequest request)
    {
        var prompt = $"Generate key vocabulary for this IELTS question: {question.QuestionText}\n";
        if (request.UserLevel != null)
            prompt += $"User Level: {request.UserLevel}\n";
        prompt += $"Include Advanced: {request.IncludeAdvanced}\n";
        prompt += "Return JSON with: vocabulary array (word, definition, example, pronunciation, partOfSpeech, difficulty), phrases array, collocations array";
        return prompt;
    }

    private static string BuildStructuresPrompt(Domain.Entities.Question question, GenerateStructuresRequest request)
    {
        var prompt = $"Generate sample sentence structures for this IELTS question: {question.QuestionText}\n";
        if (request.UserLevel != null)
            prompt += $"User Level: {request.UserLevel}\n";
        if (request.FocusAreas != null && request.FocusAreas.Length > 0)
            prompt += $"Focus Areas: {string.Join(", ", request.FocusAreas)}\n";
        prompt += "Return JSON with: structures array (pattern, examples array, usage), transitionPhrases array, idioms array";
        return prompt;
    }
}

