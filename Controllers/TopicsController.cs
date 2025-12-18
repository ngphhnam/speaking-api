using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Domain.Enums;
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.DTOs.Generation;
using SpeakingPractice.Api.DTOs.Topics;
using SpeakingPractice.Api.Infrastructure.Extensions;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class TopicsController(
    ITopicRepository topicRepository,
    IQuestionRepository questionRepository,
    IContentGenerationService contentGenerationService,
    ILogger<TopicsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? partNumber,
        [FromQuery] string? category,
        [FromQuery] bool includeInactive = false,
        [FromQuery(Name = "pageNum")] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken ct = default)
    {
        IEnumerable<Topic> topics;

        if (partNumber.HasValue)
        {
            topics = await topicRepository.GetByPartNumberAsync(partNumber.Value, ct);
        }
        else if (!string.IsNullOrWhiteSpace(category))
        {
            topics = await topicRepository.GetByCategoryAsync(category, ct);
        }
        else
        {
            topics = await topicRepository.GetAllAsync(includeInactive, ct);
        }

        var dtos = topics.Select(t => MapToDto(t)).ToList();

        // When getting all topics (no filter), also include Part 3 topics
        // Part 3 topics are the same as Part 2 topics but with partNumber = 3
        if (!partNumber.HasValue && string.IsNullOrWhiteSpace(category))
        {
            var part3Topics = await topicRepository.GetByPartNumberAsync(3, ct);
            var part3Dtos = part3Topics.Select(t => MapToDtoAsPart3(t));
            dtos.AddRange(part3Dtos);
        }

        // Normalize paging params
        if (page < 1) page = 1;
        if (pageSize <= 0) pageSize = 20;

        var totalCount = dtos.Count;
        var items = dtos
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var result = new PagedResult<TopicDto>(items, page, pageSize, totalCount);

        return this.ApiOk(result, "Topics retrieved successfully");
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(
        Guid id, 
        [FromQuery] int? partNumber = null,
        CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return this.ApiNotFound(ErrorCodes.TOPIC_NOT_FOUND, $"Topic with id {id} not found");
        }

        // If partNumber=3 is requested, return topic as Part 3 (override partNumber)
        if (partNumber == 3)
        {
            return this.ApiOk(MapToDtoAsPart3(topic), "Topic retrieved successfully");
        }

        return this.ApiOk(MapToDto(topic), "Topic retrieved successfully");
    }

    [HttpGet("slug/{slug}")]
    public async Task<IActionResult> GetBySlug(string slug, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetBySlugAsync(slug, ct);
        if (topic is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(topic));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTopicRequest request, CancellationToken ct = default)
    {
        var slug = GenerateSlug(request.Title);
        
        // Check if slug already exists
        var existing = await topicRepository.GetBySlugAsync(slug, ct);
        if (existing is not null)
        {
            return this.ApiStatusCode(409, ErrorCodes.UNIQUE_CONSTRAINT_VIOLATION, $"Topic with slug '{slug}' already exists");
        }

        var topic = new Topic
        {
            Title = request.Title,
            Slug = slug,
            Description = request.Description,
            PartNumber = request.PartNumber,
            DifficultyLevel = request.DifficultyLevel,
            TopicCategory = request.TopicCategory,
            Keywords = request.Keywords,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await topicRepository.AddAsync(topic, ct);
        await topicRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created topic {TopicId} with slug {Slug}", topic.Id, slug);
        return this.ApiCreated(nameof(GetById), new { id = topic.Id }, MapToDto(topic), "Topic created successfully");
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTopicRequest request, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Title) && request.Title != topic.Title)
        {
            topic.Title = request.Title;
            topic.Slug = GenerateSlug(request.Title);
        }

        if (request.Description is not null) topic.Description = request.Description;
        if (request.PartNumber.HasValue) topic.PartNumber = request.PartNumber;
        if (request.DifficultyLevel is not null) topic.DifficultyLevel = request.DifficultyLevel;
        if (request.TopicCategory is not null) topic.TopicCategory = request.TopicCategory;
        if (request.Keywords is not null) topic.Keywords = request.Keywords;
        if (request.IsActive.HasValue) topic.IsActive = request.IsActive.Value;

        topic.UpdatedAt = DateTimeOffset.UtcNow;

        await topicRepository.UpdateAsync(topic, ct);
        await topicRepository.SaveChangesAsync(ct);

        return this.ApiOk(MapToDto(topic), "Topic updated successfully");
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        await topicRepository.DeleteAsync(topic, ct);
        await topicRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted topic {TopicId}", id);
        return this.ApiOk("Topic deleted successfully");
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTopics([FromBody] GenerateTopicsRequest request, CancellationToken ct = default)
    {
        try
        {
            var topics = await contentGenerationService.GenerateTopicsAsync(request, ct);
            return this.ApiOk(topics, "Topics generated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating topics");
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to generate topics", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    [HttpPost("generate-with-questions")]
    public async Task<IActionResult> GenerateTopicWithQuestions([FromBody] GenerateTopicWithQuestionsRequest request, CancellationToken ct = default)
    {
        try
        {
            var topic = await contentGenerationService.GenerateTopicWithQuestionsAsync(request, ct);
            return this.ApiOk(topic, "Topic with questions generated successfully");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating topic with questions");
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to generate topic with questions", new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    /// <summary>
    /// Create a Part 2 topic with Part 2 and Part 3 questions in one request
    /// </summary>
    [HttpPost("with-questions")]
    [Authorize] // Require authentication for creating topics
    public async Task<IActionResult> CreateTopicWithQuestions(
        [FromBody] CreateTopicWithQuestionsRequest request,
        CancellationToken ct = default)
    {
        try
        {
            // Validate PartNumber
            if (request.PartNumber != 2)
            {
                return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, 
                    "This endpoint is for creating Part 2 topics only. Use POST /api/Topics for Part 1 topics.");
            }

            // Validate Part 2 question is provided
            if (request.Part2Question == null)
            {
                return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, 
                    "Part 2 question (cue card) is required for Part 2 topics.");
            }

            // Step 1: Create Topic
            var slug = GenerateSlug(request.Title);
            var existing = await topicRepository.GetBySlugAsync(slug, ct);
            if (existing is not null)
            {
                return this.ApiStatusCode(409, ErrorCodes.UNIQUE_CONSTRAINT_VIOLATION, 
                    $"Topic with slug '{slug}' already exists");
            }

            var topic = new Topic
            {
                Title = request.Title,
                Slug = slug,
                Description = request.Description,
                PartNumber = request.PartNumber,
                DifficultyLevel = request.DifficultyLevel,
                TopicCategory = request.TopicCategory,
                Keywords = request.Keywords,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await topicRepository.AddAsync(topic, ct);
            await topicRepository.SaveChangesAsync(ct);

            // Step 2: Create Part 2 Question (Cue Card)
            if (!Enum.TryParse<QuestionType>(request.Part2Question.QuestionType, ignoreCase: true, out var part2Type))
            {
                return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, 
                    "Invalid QuestionType for Part 2 question. Must be 'PART2'");
            }

            QuestionStyle? part2Style = null;
            if (!string.IsNullOrWhiteSpace(request.Part2Question.QuestionStyle))
            {
                if (!Enum.TryParse<QuestionStyle>(request.Part2Question.QuestionStyle, ignoreCase: true, out var parsedStyle))
                {
                    return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, 
                        "Invalid QuestionStyle. Must be 'CueCard' for Part 2 questions");
                }
                part2Style = parsedStyle;
            }

            var part2Question = new Question
            {
                TopicId = topic.Id,
                QuestionText = request.Part2Question.QuestionText,
                QuestionType = part2Type,
                QuestionStyle = part2Style ?? QuestionStyle.CueCard, // Default to CueCard
                SuggestedStructure = request.Part2Question.SuggestedStructure,
                SampleAnswers = request.Part2Question.SampleAnswers,
                KeyVocabulary = request.Part2Question.KeyVocabulary,
                EstimatedBandRequirement = request.Part2Question.EstimatedBandRequirement,
                TimeLimitSeconds = request.Part2Question.TimeLimitSeconds > 0 
                    ? request.Part2Question.TimeLimitSeconds 
                    : 120, // Default 120 seconds for Part 2
                AttemptsCount = 0,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };

            await questionRepository.AddAsync(part2Question, ct);

            // Step 3: Create Part 3 Questions (if provided)
            var part3Questions = new List<Question>();
            if (request.Part3Questions != null && request.Part3Questions.Any())
            {
                foreach (var part3Req in request.Part3Questions)
                {
                    if (!Enum.TryParse<QuestionType>(part3Req.QuestionType, ignoreCase: true, out var part3Type))
                    {
                        continue; // Skip invalid question
                    }

                    QuestionStyle? part3Style = null;
                    if (!string.IsNullOrWhiteSpace(part3Req.QuestionStyle))
                    {
                        Enum.TryParse<QuestionStyle>(part3Req.QuestionStyle, ignoreCase: true, out var parsedStyle);
                        part3Style = parsedStyle;
                    }

                    var part3Question = new Question
                    {
                        TopicId = topic.Id,
                        QuestionText = part3Req.QuestionText,
                        QuestionType = part3Type,
                        QuestionStyle = part3Style,
                        SuggestedStructure = part3Req.SuggestedStructure,
                        SampleAnswers = part3Req.SampleAnswers,
                        KeyVocabulary = part3Req.KeyVocabulary,
                        EstimatedBandRequirement = part3Req.EstimatedBandRequirement,
                        TimeLimitSeconds = part3Req.TimeLimitSeconds > 0 
                            ? part3Req.TimeLimitSeconds 
                            : 120, // Default 120 seconds for Part 3
                        AttemptsCount = 0,
                        IsActive = true,
                        CreatedAt = DateTimeOffset.UtcNow,
                        UpdatedAt = DateTimeOffset.UtcNow
                    };

                    part3Questions.Add(part3Question);
                    await questionRepository.AddAsync(part3Question, ct);
                }
            }

            await questionRepository.SaveChangesAsync(ct);

            // Reload topic with questions for response
            var createdTopic = await topicRepository.GetByIdAsync(topic.Id, ct);
            
            logger.LogInformation("Created topic {TopicId} with {Part2Count} Part 2 question and {Part3Count} Part 3 questions", 
                topic.Id, 1, part3Questions.Count);

            return this.ApiCreated(
                nameof(GetById), 
                new { id = topic.Id }, 
                MapToDto(createdTopic!), 
                $"Topic created successfully with {1} Part 2 question and {part3Questions.Count} Part 3 questions");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error creating topic with questions");
            return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, 
                "Failed to create topic with questions", 
                new Dictionary<string, object> { { "details", ex.Message } });
        }
    }

    [HttpGet("{id:guid}/vocabulary")]
    public async Task<IActionResult> GetTopicVocabulary(Guid id, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        // Get vocabulary from all questions in the topic
        var allVocabulary = topic.Questions?
            .SelectMany(q => q.KeyVocabulary ?? Array.Empty<string>())
            .Distinct()
            .ToList() ?? new List<string>();

        return this.ApiOk(new { vocabulary = allVocabulary }, "Vocabulary retrieved successfully");
    }

    [HttpGet("{id:guid}/statistics")]
    [AllowAnonymous]
    public async Task<IActionResult> GetStatistics(Guid id, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        var statistics = new
        {
            TopicId = id,
            UsageCount = topic.UsageCount,
            AvgUserRating = topic.AvgUserRating,
            TotalQuestions = topic.Questions?.Count ?? 0,
            ActiveQuestions = topic.Questions?.Count(q => q.IsActive) ?? 0,
            TotalAttempts = topic.Questions?.Sum(q => q.AttemptsCount) ?? 0,
            AvgScore = topic.Questions?.Where(q => q.AvgScore.HasValue).Average(q => q.AvgScore!.Value)
        };

        return this.ApiOk(statistics, "Statistics retrieved successfully");
    }

    [HttpGet("popular")]
    [AllowAnonymous]
    public async Task<IActionResult> GetPopular([FromQuery] int limit = 10, CancellationToken ct = default)
    {
        var topics = await topicRepository.GetAllAsync(false, ct);
        var popular = topics
            .OrderByDescending(t => t.UsageCount)
            .ThenByDescending(t => t.AvgUserRating ?? 0)
            .Take(limit)
            .Select(MapToDto);

        return this.ApiOk(popular, "Popular topics retrieved successfully");
    }

    [HttpGet("recommended")]
    [Authorize]
    public async Task<IActionResult> GetRecommended(CancellationToken ct = default)
    {
        // Simple recommendation: return topics with high ratings and low usage
        var topics = await topicRepository.GetAllAsync(false, ct);
        var recommended = topics
            .Where(t => t.AvgUserRating.HasValue && t.AvgUserRating >= 4.0m)
            .OrderByDescending(t => t.AvgUserRating)
            .ThenBy(t => t.UsageCount)
            .Take(10)
            .Select(MapToDto);

        return this.ApiOk(recommended, "Recommended topics retrieved successfully");
    }

    [HttpPost("{id:guid}/rate")]
    [Authorize]
    public async Task<IActionResult> RateTopic(Guid id, [FromBody] RateTopicRequest request, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        if (request.Rating < 1 || request.Rating > 5)
        {
            return this.ApiBadRequest(ErrorCodes.INVALID_VALUE, "Rating must be between 1 and 5");
        }

        // Update average rating
        var currentRating = topic.AvgUserRating ?? 0;
        var currentCount = topic.UsageCount;
        var newCount = currentCount + 1;
        topic.AvgUserRating = ((currentRating * currentCount) + request.Rating) / newCount;
        topic.UsageCount = newCount;
        topic.UpdatedAt = DateTimeOffset.UtcNow;

        await topicRepository.UpdateAsync(topic, ct);
        await topicRepository.SaveChangesAsync(ct);

        logger.LogInformation("Topic {TopicId} rated {Rating}", id, request.Rating);
        return this.ApiOk(new { avgRating = topic.AvgUserRating, totalRatings = topic.UsageCount }, "Topic rated successfully");
    }

    private static TopicDto MapToDto(Topic topic)
    {
        var part1Count = topic.Questions?.Count(q => q.QuestionType == QuestionType.PART1) ?? 0;
        var part2Count = topic.Questions?.Count(q => q.QuestionType == QuestionType.PART2) ?? 0;
        var part3Count = topic.Questions?.Count(q => q.QuestionType == QuestionType.PART3) ?? 0;

        return new TopicDto
        {
            Id = topic.Id,
            Title = topic.Title,
            Slug = topic.Slug,
            Description = topic.Description,
            PartNumber = topic.PartNumber,
            DifficultyLevel = topic.DifficultyLevel,
            TopicCategory = topic.TopicCategory,
            Keywords = topic.Keywords,
            UsageCount = topic.UsageCount,
            AvgUserRating = topic.AvgUserRating,
            IsActive = topic.IsActive,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt,
            TotalQuestion = topic.Questions?.Count ?? 0,
            Part1QuestionCount = part1Count,
            Part2QuestionCount = part2Count,
            Part3QuestionCount = part3Count
        };
    }

    private static TopicDto MapToDtoAsPart3(Topic topic)
    {
        // Map topic as Part 3 topic (partNumber = 3) but keep same data
        var part3Count = topic.Questions?.Count(q => q.QuestionType == QuestionType.PART3) ?? 0;

        return new TopicDto
        {
            Id = topic.Id,
            Title = topic.Title,
            Slug = topic.Slug,
            Description = topic.Description,
            PartNumber = 3, // Override to 3 for Part 3 view
            DifficultyLevel = topic.DifficultyLevel,
            TopicCategory = topic.TopicCategory,
            Keywords = topic.Keywords,
            UsageCount = topic.UsageCount,
            AvgUserRating = topic.AvgUserRating,
            IsActive = topic.IsActive,
            CreatedAt = topic.CreatedAt,
            UpdatedAt = topic.UpdatedAt,
            TotalQuestion = part3Count, // Only count Part 3 questions
            Part1QuestionCount = 0,
            Part2QuestionCount = 0,
            Part3QuestionCount = part3Count
        };
    }

    private static string GenerateSlug(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        slug = slug.Trim('-');
        return slug;
    }
}

public class RateTopicRequest
{
    public decimal Rating { get; set; }
}

