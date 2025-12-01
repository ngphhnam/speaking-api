using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Drafts;
using SpeakingPractice.Api.DTOs.Generation;
using SpeakingPractice.Api.DTOs.Questions;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class QuestionsController(
    IQuestionRepository questionRepository,
    ITopicRepository topicRepository,
    IContentGenerationService contentGenerationService,
    IDraftService draftService,
    ILogger<QuestionsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? topicId,
        [FromQuery] bool includeInactive = false,
        CancellationToken ct = default)
    {
        IEnumerable<Question> questions;

        if (topicId.HasValue)
        {
            questions = await questionRepository.GetByTopicIdAsync(topicId.Value, ct);
        }
        else
        {
            questions = await questionRepository.GetAllAsync(includeInactive, ct);
        }

        var dtos = questions.Select(q => MapToDto(q));
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(question));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateQuestionRequest request, CancellationToken ct = default)
    {
        try
        {
            // Validate topic exists if provided
            if (request.TopicId.HasValue)
            {
                var topic = await topicRepository.GetByIdAsync(request.TopicId.Value, ct);
                if (topic is null)
                {
                    return BadRequest($"Topic with id {request.TopicId} not found");
                }
            }

            var question = new Question
            {
                TopicId = request.TopicId,
                QuestionText = request.QuestionText,
                QuestionType = request.QuestionType,
                SuggestedStructure = request.SuggestedStructure,
                SampleAnswers = request.SampleAnswers,
                KeyVocabulary = request.KeyVocabulary,
                EstimatedBandRequirement = request.EstimatedBandRequirement,
                TimeLimitSeconds = request.TimeLimitSeconds,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await questionRepository.AddAsync(question, ct);
            await questionRepository.SaveChangesAsync(ct);

            logger.LogInformation("Created question {QuestionId}", question.Id);
            return CreatedAtAction(nameof(GetById), new { id = question.Id }, MapToDto(question));
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            logger.LogError(ex, "Database error creating question. Inner exception: {InnerException}", ex.InnerException?.Message);
            return StatusCode(500, new
            {
                error = "An error occurred while saving the question.",
                details = ex.InnerException?.Message ?? ex.Message,
                traceId = HttpContext.TraceIdentifier
            });
        }
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateQuestionRequest request, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        // Validate topic exists if provided
        if (request.TopicId.HasValue)
        {
            var topic = await topicRepository.GetByIdAsync(request.TopicId.Value, ct);
            if (topic is null)
            {
                return BadRequest($"Topic with id {request.TopicId} not found");
            }
        }

        if (request.TopicId.HasValue) question.TopicId = request.TopicId;
        if (!string.IsNullOrWhiteSpace(request.QuestionText)) question.QuestionText = request.QuestionText;
        if (request.QuestionType is not null) question.QuestionType = request.QuestionType;
        if (request.SuggestedStructure is not null) question.SuggestedStructure = request.SuggestedStructure;
        if (request.SampleAnswers is not null) question.SampleAnswers = request.SampleAnswers;
        if (request.KeyVocabulary is not null) question.KeyVocabulary = request.KeyVocabulary;
        if (request.EstimatedBandRequirement.HasValue) question.EstimatedBandRequirement = request.EstimatedBandRequirement;
        if (request.TimeLimitSeconds.HasValue) question.TimeLimitSeconds = request.TimeLimitSeconds.Value;
        if (request.IsActive.HasValue) question.IsActive = request.IsActive.Value;

        question.UpdatedAt = DateTimeOffset.UtcNow;

        await questionRepository.UpdateAsync(question, ct);
        await questionRepository.SaveChangesAsync(ct);

        return Ok(MapToDto(question));
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        await questionRepository.DeleteAsync(question, ct);
        await questionRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted question {QuestionId}", id);
        return NoContent();
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateQuestions([FromBody] GenerateQuestionsRequest request, CancellationToken ct = default)
    {
        try
        {
            var questions = await contentGenerationService.GenerateQuestionsAsync(request, ct);
            return Ok(questions);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating questions");
            return StatusCode(500, new { error = "Failed to generate questions", message = ex.Message });
        }
    }

    [HttpPost("{id:guid}/outline/generate")]
    public async Task<IActionResult> GenerateOutline(Guid id, [FromBody] GenerateOutlineRequest? request, CancellationToken ct = default)
    {
        try
        {
            request ??= new GenerateOutlineRequest();
            var outline = await contentGenerationService.GenerateOutlineAsync(id, request, ct);
            return Ok(outline);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating outline for question {QuestionId}", id);
            return StatusCode(500, new { error = "Failed to generate outline", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/vocabulary")]
    public async Task<IActionResult> GetVocabulary(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        // Return existing vocabulary or generate enhanced version
        var vocabulary = new VocabularyDto
        {
            Vocabulary = question.KeyVocabulary?
                .Select(v => new VocabularyItem
                {
                    Word = v,
                    Definition = "",
                    Example = "",
                    Difficulty = question.EstimatedBandRequirement?.ToString() ?? "intermediate"
                })
                .ToList() ?? new List<VocabularyItem>(),
            Phrases = Array.Empty<string>(),
            Collocations = Array.Empty<string>()
        };

        return Ok(vocabulary);
    }

    [HttpPost("{id:guid}/vocabulary/enhance")]
    public async Task<IActionResult> EnhanceVocabulary(Guid id, [FromBody] GenerateVocabularyRequest? request, CancellationToken ct = default)
    {
        try
        {
            request ??= new GenerateVocabularyRequest();
            var vocabulary = await contentGenerationService.GenerateVocabularyAsync(id, request, ct);
            return Ok(vocabulary);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error enhancing vocabulary for question {QuestionId}", id);
            return StatusCode(500, new { error = "Failed to enhance vocabulary", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/structures")]
    public async Task<IActionResult> GetStructures(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        // Parse suggested structure if available
        var structures = new StructuresDto
        {
            Structures = new List<StructureItem>(),
            TransitionPhrases = Array.Empty<string>(),
            Idioms = Array.Empty<string>()
        };

        if (!string.IsNullOrEmpty(question.SuggestedStructure))
        {
            // Try to parse as JSON or return as-is
            structures.Structures.Add(new StructureItem
            {
                Pattern = question.SuggestedStructure,
                Examples = Array.Empty<string>(),
                Usage = "Suggested structure for this question"
            });
        }

        return Ok(structures);
    }

    [HttpPost("{id:guid}/structures/generate")]
    public async Task<IActionResult> GenerateStructures(Guid id, [FromBody] GenerateStructuresRequest? request, CancellationToken ct = default)
    {
        try
        {
            request ??= new GenerateStructuresRequest();
            var structures = await contentGenerationService.GenerateStructuresAsync(id, request, ct);
            return Ok(structures);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating structures for question {QuestionId}", id);
            return StatusCode(500, new { error = "Failed to generate structures", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/sample-answers")]
    public async Task<IActionResult> GetSampleAnswers(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        var sampleAnswers = new SampleAnswersDto
        {
            SampleAnswers = question.SampleAnswers?
                .Select((text, index) => new SampleAnswer
                {
                    Text = text,
                    BandScore = question.EstimatedBandRequirement ?? 6.0m,
                    Highlights = new AnswerHighlights
                    {
                        Vocabulary = question.KeyVocabulary ?? Array.Empty<string>(),
                        Grammar = Array.Empty<string>(),
                        Structure = question.SuggestedStructure ?? ""
                    }
                })
                .ToList() ?? new List<SampleAnswer>()
        };

        return Ok(sampleAnswers);
    }

    [HttpPost("{id:guid}/drafts")]
    public async Task<IActionResult> SaveDraft(Guid id, [FromBody] SaveDraftRequest request, CancellationToken ct = default)
    {
        try
        {
            // For now, use Guid.Empty as userId (anonymous user)
            // In production, get from authenticated user
            var userId = Guid.Empty; // TODO: Get from authenticated user
            var draft = await draftService.SaveDraftAsync(userId, id, request, ct);
            return Ok(draft);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving draft for question {QuestionId}", id);
            return StatusCode(500, new { error = "Failed to save draft", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/drafts")]
    public async Task<IActionResult> GetDraft(Guid id, CancellationToken ct = default)
    {
        try
        {
            // For now, use Guid.Empty as userId (anonymous user)
            var userId = Guid.Empty; // TODO: Get from authenticated user
            var draft = await draftService.GetDraftAsync(userId, id, ct);
            if (draft == null)
            {
                return NotFound();
            }
            return Ok(draft);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting draft for question {QuestionId}", id);
            return StatusCode(500, new { error = "Failed to get draft", message = ex.Message });
        }
    }

    [HttpGet("{id:guid}/statistics")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatistics(Guid id, CancellationToken ct = default)
    {
        var question = await questionRepository.GetByIdAsync(id, ct);
        if (question is null)
        {
            return NotFound();
        }

        var statistics = new
        {
            QuestionId = id,
            AttemptsCount = question.AttemptsCount,
            AvgScore = question.AvgScore,
            IsActive = question.IsActive
        };

        return Ok(statistics);
    }

    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopular([FromQuery] int limit = 10, CancellationToken ct = default)
    {
        var questions = await questionRepository.GetAllAsync(false, ct);
        var popular = questions
            .OrderByDescending(q => q.AttemptsCount)
            .ThenByDescending(q => q.AvgScore ?? 0)
            .Take(limit)
            .Select(MapToDto);

        return Ok(popular);
    }

    [HttpGet("recommended")]
    [Authorize]
    public async Task<IActionResult> GetRecommended(CancellationToken ct = default)
    {
        // Simple recommendation: return questions with good scores and moderate attempts
        var questions = await questionRepository.GetAllAsync(false, ct);
        var recommended = questions
            .Where(q => q.AvgScore.HasValue && q.AvgScore >= 6.0m && q.AttemptsCount > 0)
            .OrderByDescending(q => q.AvgScore)
            .Take(10)
            .Select(MapToDto);

        return Ok(recommended);
    }

    [HttpGet("topic/{topicId:guid}/random")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRandomByTopic(Guid topicId, CancellationToken ct = default)
    {
        var questions = await questionRepository.GetByTopicIdAsync(topicId, ct);
        var activeQuestions = questions.Where(q => q.IsActive).ToList();

        if (!activeQuestions.Any())
        {
            return NotFound("No active questions found for this topic");
        }

        var random = new Random();
        var randomQuestion = activeQuestions[random.Next(activeQuestions.Count)];

        return Ok(MapToDto(randomQuestion));
    }

    private static QuestionDto MapToDto(Question question)
    {
        return new QuestionDto
        {
            Id = question.Id,
            TopicId = question.TopicId,
            TopicTitle = question.Topic?.Title,
            QuestionText = question.QuestionText,
            QuestionType = question.QuestionType,
            SuggestedStructure = question.SuggestedStructure,
            SampleAnswers = question.SampleAnswers,
            KeyVocabulary = question.KeyVocabulary,
            EstimatedBandRequirement = question.EstimatedBandRequirement,
            TimeLimitSeconds = question.TimeLimitSeconds,
            AttemptsCount = question.AttemptsCount,
            AvgScore = question.AvgScore,
            IsActive = question.IsActive,
            CreatedAt = question.CreatedAt,
            UpdatedAt = question.UpdatedAt
        };
    }
}

