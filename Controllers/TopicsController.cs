using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.DTOs.Generation;
using SpeakingPractice.Api.DTOs.Topics;
using SpeakingPractice.Api.Repositories;
using SpeakingPractice.Api.Services.Interfaces;
using System.Text.RegularExpressions;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
public class TopicsController(
    ITopicRepository topicRepository,
    IContentGenerationService contentGenerationService,
    ILogger<TopicsController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int? partNumber,
        [FromQuery] string? category,
        [FromQuery] bool includeInactive = false,
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

        var dtos = topics.Select(t => MapToDto(t));
        return Ok(dtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(id, ct);
        if (topic is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(topic));
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
            return Conflict($"Topic with slug '{slug}' already exists");
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
        return CreatedAtAction(nameof(GetById), new { id = topic.Id }, MapToDto(topic));
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

        return Ok(MapToDto(topic));
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
        return NoContent();
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateTopics([FromBody] GenerateTopicsRequest request, CancellationToken ct = default)
    {
        try
        {
            var topics = await contentGenerationService.GenerateTopicsAsync(request, ct);
            return Ok(topics);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating topics");
            return StatusCode(500, new { error = "Failed to generate topics", message = ex.Message });
        }
    }

    [HttpPost("generate-with-questions")]
    public async Task<IActionResult> GenerateTopicWithQuestions([FromBody] GenerateTopicWithQuestionsRequest request, CancellationToken ct = default)
    {
        try
        {
            var topic = await contentGenerationService.GenerateTopicWithQuestionsAsync(request, ct);
            return Ok(topic);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating topic with questions");
            return StatusCode(500, new { error = "Failed to generate topic with questions", message = ex.Message });
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

        return Ok(new { vocabulary = allVocabulary });
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

        return Ok(statistics);
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

        return Ok(popular);
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

        return Ok(recommended);
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
            return BadRequest("Rating must be between 1 and 5");
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
        return Ok(new { avgRating = topic.AvgUserRating, totalRatings = topic.UsageCount });
    }

    private static TopicDto MapToDto(Topic topic)
    {
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
            TotalQuestion = topic.Questions?.Count ?? 0
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

