using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Vocabulary;
using SpeakingPractice.Api.Repositories;

namespace SpeakingPractice.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VocabularyController(
    IVocabularyRepository vocabularyRepository,
    ITopicRepository topicRepository,
    ILogger<VocabularyController> logger) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAll(
        [FromQuery] decimal? minBand,
        [FromQuery] decimal? maxBand,
        [FromQuery] string? topicCategory,
        [FromQuery] string? search,
        CancellationToken ct = default)
    {
        IEnumerable<Domain.Entities.Vocabulary> vocabularies;

        if (!string.IsNullOrWhiteSpace(search))
        {
            vocabularies = await vocabularyRepository.SearchAsync(search, ct);
        }
        else if (!string.IsNullOrWhiteSpace(topicCategory))
        {
            vocabularies = await vocabularyRepository.GetByTopicCategoryAsync(topicCategory, ct);
        }
        else if (minBand.HasValue || maxBand.HasValue)
        {
            vocabularies = await vocabularyRepository.GetByBandLevelAsync(minBand, maxBand, ct);
        }
        else
        {
            vocabularies = await vocabularyRepository.GetAllAsync(ct);
        }

        return Ok(vocabularies.Select(MapToDto));
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct = default)
    {
        var vocabulary = await vocabularyRepository.GetByIdAsync(id, ct);
        if (vocabulary is null)
        {
            return NotFound();
        }

        return Ok(MapToDto(vocabulary));
    }

    [HttpGet("search")]
    [AllowAnonymous]
    public async Task<IActionResult> Search([FromQuery] string q, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query is required");
        }

        var vocabularies = await vocabularyRepository.SearchAsync(q, ct);
        return Ok(vocabularies.Select(MapToDto));
    }

    [HttpGet("topic/{topicId:guid}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByTopic(Guid topicId, CancellationToken ct = default)
    {
        var topic = await topicRepository.GetByIdAsync(topicId, ct);
        if (topic is null)
        {
            return NotFound();
        }

        // Get vocabulary from topic's questions
        var allVocabulary = topic.Questions?
            .SelectMany(q => q.KeyVocabulary ?? Array.Empty<string>())
            .Distinct()
            .ToList() ?? new List<string>();

        // Try to find vocabulary entries for these words
        var vocabularies = new List<Domain.Entities.Vocabulary>();
        foreach (var word in allVocabulary)
        {
            var vocab = await vocabularyRepository.GetByWordAsync(word, ct);
            if (vocab is not null)
            {
                vocabularies.Add(vocab);
            }
        }

        return Ok(vocabularies.Select(MapToDto));
    }

    [HttpGet("band/{bandLevel:decimal}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetByBandLevel(decimal bandLevel, CancellationToken ct = default)
    {
        var vocabularies = await vocabularyRepository.GetByBandLevelAsync(bandLevel, bandLevel + 1, ct);
        return Ok(vocabularies.Select(MapToDto));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Create([FromBody] CreateVocabularyRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Word))
        {
            return BadRequest("Word is required");
        }

        // Check if word already exists
        var existing = await vocabularyRepository.GetByWordAsync(request.Word, ct);
        if (existing is not null)
        {
            return Conflict($"Vocabulary with word '{request.Word}' already exists");
        }

        var vocabulary = new Domain.Entities.Vocabulary
        {
            Word = request.Word,
            Phonetic = request.Phonetic,
            PartOfSpeech = request.PartOfSpeech,
            DefinitionEn = request.DefinitionEn,
            DefinitionVi = request.DefinitionVi,
            IeltsBandLevel = request.IeltsBandLevel,
            TopicCategories = request.TopicCategories,
            ExampleSentences = request.ExampleSentences,
            Synonyms = request.Synonyms,
            Antonyms = request.Antonyms,
            Collocations = request.Collocations,
            CreatedAt = DateTimeOffset.UtcNow,
            UpdatedAt = DateTimeOffset.UtcNow
        };

        await vocabularyRepository.AddAsync(vocabulary, ct);
        await vocabularyRepository.SaveChangesAsync(ct);

        logger.LogInformation("Created vocabulary {VocabularyId} for word {Word}", vocabulary.Id, vocabulary.Word);
        return CreatedAtAction(nameof(GetById), new { id = vocabulary.Id }, MapToDto(vocabulary));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Teacher")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVocabularyRequest request, CancellationToken ct = default)
    {
        var vocabulary = await vocabularyRepository.GetByIdAsync(id, ct);
        if (vocabulary is null)
        {
            return NotFound();
        }

        if (!string.IsNullOrWhiteSpace(request.Word) && request.Word != vocabulary.Word)
        {
            var existing = await vocabularyRepository.GetByWordAsync(request.Word, ct);
            if (existing is not null && existing.Id != id)
            {
                return Conflict($"Vocabulary with word '{request.Word}' already exists");
            }
            vocabulary.Word = request.Word;
        }

        if (request.Phonetic is not null) vocabulary.Phonetic = request.Phonetic;
        if (request.PartOfSpeech is not null) vocabulary.PartOfSpeech = request.PartOfSpeech;
        if (request.DefinitionEn is not null) vocabulary.DefinitionEn = request.DefinitionEn;
        if (request.DefinitionVi is not null) vocabulary.DefinitionVi = request.DefinitionVi;
        if (request.IeltsBandLevel.HasValue) vocabulary.IeltsBandLevel = request.IeltsBandLevel;
        if (request.TopicCategories is not null) vocabulary.TopicCategories = request.TopicCategories;
        if (request.ExampleSentences is not null) vocabulary.ExampleSentences = request.ExampleSentences;
        if (request.Synonyms is not null) vocabulary.Synonyms = request.Synonyms;
        if (request.Antonyms is not null) vocabulary.Antonyms = request.Antonyms;
        if (request.Collocations is not null) vocabulary.Collocations = request.Collocations;

        vocabulary.UpdatedAt = DateTimeOffset.UtcNow;

        await vocabularyRepository.UpdateAsync(vocabulary, ct);
        await vocabularyRepository.SaveChangesAsync(ct);

        return Ok(MapToDto(vocabulary));
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct = default)
    {
        var vocabulary = await vocabularyRepository.GetByIdAsync(id, ct);
        if (vocabulary is null)
        {
            return NotFound();
        }

        await vocabularyRepository.DeleteAsync(vocabulary, ct);
        await vocabularyRepository.SaveChangesAsync(ct);

        logger.LogInformation("Deleted vocabulary {VocabularyId}", id);
        return NoContent();
    }

    private static VocabularyDto MapToDto(Domain.Entities.Vocabulary vocabulary)
    {
        return new VocabularyDto
        {
            Id = vocabulary.Id,
            Word = vocabulary.Word,
            Phonetic = vocabulary.Phonetic,
            PartOfSpeech = vocabulary.PartOfSpeech,
            DefinitionEn = vocabulary.DefinitionEn,
            DefinitionVi = vocabulary.DefinitionVi,
            IeltsBandLevel = vocabulary.IeltsBandLevel,
            TopicCategories = vocabulary.TopicCategories,
            ExampleSentences = vocabulary.ExampleSentences,
            Synonyms = vocabulary.Synonyms,
            Antonyms = vocabulary.Antonyms,
            Collocations = vocabulary.Collocations,
            UsageFrequency = vocabulary.UsageFrequency,
            CreatedAt = vocabulary.CreatedAt,
            UpdatedAt = vocabulary.UpdatedAt
        };
    }
}
