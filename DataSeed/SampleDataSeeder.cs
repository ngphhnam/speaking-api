using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Domain.Enums;
using SpeakingPractice.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SpeakingPractice.Api.DataSeed;

/// <summary>
/// Seeds the database with realistic sample data for development and testing
/// </summary>
public static class SampleDataSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if data already exists
        if (await context.Topics.AnyAsync())
        {
            return; // Data already seeded
        }

        var now = DateTimeOffset.UtcNow;

        // ============================================
        // TOPICS
        // ============================================
        var topics = new List<Topic>
        {
            // Part 1 Topics
            new Topic
            {
                Id = Guid.Parse("a1b2c3d4-e5f6-4789-a012-345678901234"),
                Title = "Hometown",
                Slug = "hometown",
                Description = "Questions about your hometown, where you live, and your local area",
                PartNumber = 1,
                DifficultyLevel = "beginner",
                TopicCategory = "personal",
                Keywords = new[] { "hometown", "city", "place", "location" },
                UsageCount = 45,
                AvgUserRating = 4.2m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Topic
            {
                Id = Guid.Parse("b2c3d4e5-f6a7-4890-b123-456789012345"),
                Title = "Work",
                Slug = "work",
                Description = "Questions about your job, career, and professional life",
                PartNumber = 1,
                DifficultyLevel = "intermediate",
                TopicCategory = "professional",
                Keywords = new[] { "work", "job", "career", "profession" },
                UsageCount = 38,
                AvgUserRating = 4.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Topic
            {
                Id = Guid.Parse("c3d4e5f6-a7b8-4901-c234-567890123456"),
                Title = "Studies",
                Slug = "studies",
                Description = "Questions about your education, school, and learning",
                PartNumber = 1,
                DifficultyLevel = "beginner",
                TopicCategory = "education",
                Keywords = new[] { "study", "education", "school", "university" },
                UsageCount = 52,
                AvgUserRating = 4.3m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Topic
            {
                Id = Guid.Parse("d4e5f6a7-b8c9-4012-d345-678901234567"),
                Title = "Hobbies",
                Slug = "hobbies",
                Description = "Questions about your interests, hobbies, and free time activities",
                PartNumber = 1,
                DifficultyLevel = "beginner",
                TopicCategory = "lifestyle",
                Keywords = new[] { "hobby", "interest", "leisure", "activity" },
                UsageCount = 41,
                AvgUserRating = 4.4m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Part 2 Topics (Part 3 questions will be added to these topics)
            new Topic
            {
                Id = Guid.Parse("e5f6a7b8-c9d0-4123-e456-789012345678"),
                Title = "Describe a Person",
                Slug = "describe-a-person",
                Description = "Describe someone who influenced you, a family member, or a famous person",
                PartNumber = 2,
                DifficultyLevel = "intermediate",
                TopicCategory = "descriptive",
                Keywords = new[] { "person", "describe", "character", "influence" },
                UsageCount = 67,
                AvgUserRating = 4.6m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Topic
            {
                Id = Guid.Parse("f6a7b8c9-d0e1-4234-f567-890123456789"),
                Title = "Describe a Place",
                Slug = "describe-a-place",
                Description = "Describe a place you visited, want to visit, or a memorable location",
                PartNumber = 2,
                DifficultyLevel = "intermediate",
                TopicCategory = "descriptive",
                Keywords = new[] { "place", "location", "travel", "visit" },
                UsageCount = 73,
                AvgUserRating = 4.7m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Topic
            {
                Id = Guid.Parse("a7b8c9d0-e1f2-4345-a678-901234567890"),
                Title = "Describe an Event",
                Slug = "describe-an-event",
                Description = "Describe a memorable event, celebration, or experience",
                PartNumber = 2,
                DifficultyLevel = "intermediate",
                TopicCategory = "narrative",
                Keywords = new[] { "event", "celebration", "experience", "occasion" },
                UsageCount = 58,
                AvgUserRating = 4.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Topics.AddRangeAsync(topics);

        // ============================================
        // QUESTIONS
        // ============================================
        var questions = new List<Question>
        {
            // Part 1 Questions
            new Question
            {
                Id = Guid.Parse("00010001-0000-0000-0000-000000000001"),
                TopicId = Guid.Parse("a1b2c3d4-e5f6-4789-a012-345678901234"),
                QuestionText = "Where are you from?",
                QuestionType = QuestionType.PART1,
                QuestionStyle = QuestionStyle.OpenEnded,
                SuggestedStructure = "Introduction - State your hometown\nDetails - Describe location and size\nPersonal connection - How long you've lived there",
                KeyVocabulary = new[] { "hometown", "located", "reside", "originally" },
                EstimatedBandRequirement = 5.0m,
                TimeLimitSeconds = 60,
                AttemptsCount = 23,
                AvgScore = 6.2m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00010001-0000-0000-0000-000000000002"),
                TopicId = Guid.Parse("a1b2c3d4-e5f6-4789-a012-345678901234"),
                QuestionText = "What do you like most about your hometown?",
                QuestionType = QuestionType.PART1,
                QuestionStyle = QuestionStyle.OpenEnded,
                SuggestedStructure = "Introduction - State your favorite aspect\nMain point - Specific feature with example\nConclusion - Why it matters to you",
                KeyVocabulary = new[] { "appreciate", "favorite", "attraction", "characteristic" },
                EstimatedBandRequirement = 5.5m,
                TimeLimitSeconds = 60,
                AttemptsCount = 18,
                AvgScore = 6.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00010001-0000-0000-0000-000000000003"),
                TopicId = Guid.Parse("b2c3d4e5-f6a7-4890-b123-456789012345"),
                QuestionText = "What is your job?",
                QuestionType = QuestionType.PART1,
                QuestionStyle = QuestionStyle.OpenEnded,
                SuggestedStructure = "Introduction - State your profession\nDetails - Describe your role and responsibilities\nPersonal opinion - How you feel about it",
                KeyVocabulary = new[] { "profession", "occupation", "role", "responsibilities" },
                EstimatedBandRequirement = 5.5m,
                TimeLimitSeconds = 60,
                AttemptsCount = 31,
                AvgScore = 6.8m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Part 2 Questions
            new Question
            {
                Id = Guid.Parse("00020001-0000-0000-0000-000000000001"),
                TopicId = Guid.Parse("e5f6a7b8-c9d0-4123-e456-789012345678"),
                QuestionText = "Describe a person who has influenced you. You should say:\n- Who this person is\n- How you know them\n- What they have done to influence you\n- And explain why this person is important to you",
                QuestionType = QuestionType.PART2,
                QuestionStyle = QuestionStyle.CueCard,
                SuggestedStructure = "Introduction - Name the person\nWho they are - Relationship and background\nHow you know them - Context of your relationship\nTheir influence - Specific examples of impact\nWhy important - Personal significance\nConclusion - Summary",
                KeyVocabulary = new[] { "influence", "inspire", "mentor", "role model", "impact" },
                EstimatedBandRequirement = 6.5m,
                TimeLimitSeconds = 120,
                AttemptsCount = 15,
                AvgScore = 7.2m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00020001-0000-0000-0000-000000000002"),
                TopicId = Guid.Parse("f6a7b8c9-d0e1-4234-f567-890123456789"),
                QuestionText = "Describe a place you would like to visit. You should say:\n- Where it is\n- What you know about it\n- What you would like to do there\n- And explain why you want to visit this place",
                QuestionType = QuestionType.PART2,
                QuestionStyle = QuestionStyle.CueCard,
                SuggestedStructure = "Introduction - Name the place\nLocation - Where it is geographically\nWhat you know - Facts and information\nActivities - What you'd like to do\nWhy visit - Reasons and motivation\nConclusion - Summary",
                KeyVocabulary = new[] { "destination", "landmark", "attraction", "itinerary", "explore" },
                EstimatedBandRequirement = 7.0m,
                TimeLimitSeconds = 120,
                AttemptsCount = 12,
                AvgScore = 7.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Part 3 Questions - Related to Part 2 Topics
            // Part 3 questions for "Describe a Person" topic
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000001"),
                TopicId = Guid.Parse("e5f6a7b8-c9d0-4123-e456-789012345678"), // Describe a Person
                QuestionText = "What qualities make someone a good role model?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.Opinion,
                SuggestedStructure = "Introduction - State your view\nMain point 1 - Quality with example\nMain point 2 - Another quality\nConclusion - Summary",
                KeyVocabulary = new[] { "role model", "qualities", "influence", "inspire", "character" },
                EstimatedBandRequirement = 7.0m,
                TimeLimitSeconds = 120,
                AttemptsCount = 8,
                AvgScore = 7.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000002"),
                TopicId = Guid.Parse("e5f6a7b8-c9d0-4123-e456-789012345678"), // Describe a Person
                QuestionText = "Do you think celebrities have a responsibility to be role models?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.Opinion,
                SuggestedStructure = "Introduction - State your position\nMain point 1 - Arguments for\nMain point 2 - Arguments against\nConclusion - Balanced view",
                KeyVocabulary = new[] { "celebrity", "responsibility", "role model", "influence", "public figure" },
                EstimatedBandRequirement = 7.5m,
                TimeLimitSeconds = 120,
                AttemptsCount = 6,
                AvgScore = 7.8m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Part 3 questions for "Describe a Place" topic
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000003"),
                TopicId = Guid.Parse("f6a7b8c9-d0e1-4234-f567-890123456789"), // Describe a Place
                QuestionText = "Why do people like to travel?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.OpenEnded,
                SuggestedStructure = "Introduction - Acknowledge the question\nMain point 1 - Reason with example\nMain point 2 - Another reason\nConclusion - Summary",
                KeyVocabulary = new[] { "travel", "tourism", "explore", "discover", "experience" },
                EstimatedBandRequirement = 6.5m,
                TimeLimitSeconds = 120,
                AttemptsCount = 10,
                AvgScore = 7.0m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000004"),
                TopicId = Guid.Parse("f6a7b8c9-d0e1-4234-f567-890123456789"), // Describe a Place
                QuestionText = "What are the advantages and disadvantages of tourism?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.Comparison,
                SuggestedStructure = "Introduction - State your view\nMain point 1 - Advantages with examples\nMain point 2 - Disadvantages with examples\nConclusion - Balanced summary",
                KeyVocabulary = new[] { "tourism", "advantage", "disadvantage", "economy", "culture", "environment" },
                EstimatedBandRequirement = 7.5m,
                TimeLimitSeconds = 120,
                AttemptsCount = 7,
                AvgScore = 7.8m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            // Part 3 questions for "Describe an Event" topic
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000005"),
                TopicId = Guid.Parse("a7b8c9d0-e1f2-4345-a678-901234567890"), // Describe an Event
                QuestionText = "What types of events do people celebrate in your country?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.OpenEnded,
                SuggestedStructure = "Introduction - Overview\nMain point 1 - Type of event with example\nMain point 2 - Another type\nConclusion - Summary",
                KeyVocabulary = new[] { "celebrate", "event", "tradition", "festival", "occasion" },
                EstimatedBandRequirement = 6.5m,
                TimeLimitSeconds = 120,
                AttemptsCount = 9,
                AvgScore = 7.2m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Question
            {
                Id = Guid.Parse("00030001-0000-0000-0000-000000000006"),
                TopicId = Guid.Parse("a7b8c9d0-e1f2-4345-a678-901234567890"), // Describe an Event
                QuestionText = "Do you think traditional celebrations are still important?",
                QuestionType = QuestionType.PART3,
                QuestionStyle = QuestionStyle.Opinion,
                SuggestedStructure = "Introduction - State your position\nMain point 1 - Why they're important\nMain point 2 - Challenges they face\nConclusion - Your view",
                KeyVocabulary = new[] { "traditional", "celebration", "culture", "heritage", "preserve" },
                EstimatedBandRequirement = 7.0m,
                TimeLimitSeconds = 120,
                AttemptsCount = 8,
                AvgScore = 7.5m,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Questions.AddRangeAsync(questions);

        // ============================================
        // VOCABULARY
        // ============================================
        var vocabularies = new List<Vocabulary>
        {
            new Vocabulary
            {
                Id = Guid.Parse("0000a001-0000-0000-0000-000000000001"),
                Word = "significant",
                Phonetic = "/sɪɡˈnɪfɪkənt/",
                PartOfSpeech = "adjective",
                DefinitionEn = "Important or noticeable",
                DefinitionVi = "Quan trọng, đáng kể",
                IeltsBandLevel = 6.5m,
                TopicCategories = new[] { "academic", "general" },
                ExampleSentences = new[] { "The changes have had a significant impact on the environment.", "This is a significant achievement for the team." },
                Synonyms = new[] { "important", "substantial", "notable" },
                Antonyms = new[] { "insignificant", "minor", "trivial" },
                Collocations = new[] { "significant impact", "significant change", "significant difference" },
                UsageFrequency = 45,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Vocabulary
            {
                Id = Guid.Parse("0000a002-0000-0000-0000-000000000002"),
                Word = "sustainable",
                Phonetic = "/səˈsteɪnəbəl/",
                PartOfSpeech = "adjective",
                DefinitionEn = "Able to be maintained at a certain rate or level",
                DefinitionVi = "Bền vững",
                IeltsBandLevel = 7.0m,
                TopicCategories = new[] { "environment", "society" },
                ExampleSentences = new[] { "We need to find sustainable solutions to environmental problems.", "Sustainable development is crucial for future generations." },
                Synonyms = new[] { "maintainable", "viable", "enduring" },
                Antonyms = new[] { "unsustainable", "temporary" },
                Collocations = new[] { "sustainable development", "sustainable energy", "sustainable practice" },
                UsageFrequency = 38,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Vocabulary
            {
                Id = Guid.Parse("0000a003-0000-0000-0000-000000000003"),
                Word = "influence",
                Phonetic = "/ˈɪnfluəns/",
                PartOfSpeech = "noun/verb",
                DefinitionEn = "The capacity to have an effect on someone or something",
                DefinitionVi = "Ảnh hưởng",
                IeltsBandLevel = 6.0m,
                TopicCategories = new[] { "general", "social" },
                ExampleSentences = new[] { "Her teacher had a great influence on her career choice.", "Social media can influence public opinion." },
                Synonyms = new[] { "impact", "effect", "sway" },
                Antonyms = new[] { "uninfluenced" },
                Collocations = new[] { "have influence", "under the influence", "influence on" },
                UsageFrequency = 52,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Vocabulary
            {
                Id = Guid.Parse("0000a004-0000-0000-0000-000000000004"),
                Word = "innovative",
                Phonetic = "/ˈɪnəveɪtɪv/",
                PartOfSpeech = "adjective",
                DefinitionEn = "Featuring new methods; advanced and original",
                DefinitionVi = "Sáng tạo, đổi mới",
                IeltsBandLevel = 7.5m,
                TopicCategories = new[] { "technology", "business" },
                ExampleSentences = new[] { "The company is known for its innovative approach to problem-solving.", "Innovative technologies are transforming healthcare." },
                Synonyms = new[] { "creative", "original", "groundbreaking" },
                Antonyms = new[] { "conventional", "traditional" },
                Collocations = new[] { "innovative solution", "innovative technology", "innovative approach" },
                UsageFrequency = 29,
                CreatedAt = now,
                UpdatedAt = now
            },
            new Vocabulary
            {
                Id = Guid.Parse("0000a005-0000-0000-0000-000000000005"),
                Word = "comprehensive",
                Phonetic = "/ˌkɒmprɪˈhensɪv/",
                PartOfSpeech = "adjective",
                DefinitionEn = "Complete and including everything",
                DefinitionVi = "Toàn diện, bao quát",
                IeltsBandLevel = 7.0m,
                TopicCategories = new[] { "academic", "general" },
                ExampleSentences = new[] { "The report provides a comprehensive analysis of the situation.", "We need a comprehensive approach to solve this problem." },
                Synonyms = new[] { "complete", "thorough", "extensive" },
                Antonyms = new[] { "incomplete", "partial" },
                Collocations = new[] { "comprehensive study", "comprehensive review", "comprehensive coverage" },
                UsageFrequency = 33,
                CreatedAt = now,
                UpdatedAt = now
            }
        };

        await context.Vocabularies.AddRangeAsync(vocabularies);

        // ============================================
        // ACHIEVEMENTS
        // ============================================
        var achievements = new List<Achievement>
        {
            new Achievement
            {
                Id = Guid.Parse("0000b001-0000-0000-0000-000000000001"),
                Title = "First Steps",
                Description = "Complete your first practice session",
                AchievementType = "practice_milestone",
                RequirementCriteria = "{\"sessions_completed\": 1}",
                Points = 10,
                BadgeIconUrl = "/badges/first-steps.png",
                IsActive = true,
                CreatedAt = now
            },
            new Achievement
            {
                Id = Guid.Parse("0000b002-0000-0000-0000-000000000002"),
                Title = "Week Warrior",
                Description = "Practice for 7 consecutive days",
                AchievementType = "practice_streak",
                RequirementCriteria = "{\"consecutive_days\": 7}",
                Points = 50,
                BadgeIconUrl = "/badges/week-warrior.png",
                IsActive = true,
                CreatedAt = now
            },
            new Achievement
            {
                Id = Guid.Parse("0000b003-0000-0000-0000-000000000003"),
                Title = "Band 7 Achiever",
                Description = "Score 7.0 or higher on any practice",
                AchievementType = "score_milestone",
                RequirementCriteria = "{\"min_score\": 7.0}",
                Points = 100,
                BadgeIconUrl = "/badges/band-7.png",
                IsActive = true,
                CreatedAt = now
            },
            new Achievement
            {
                Id = Guid.Parse("0000b004-0000-0000-0000-000000000004"),
                Title = "Vocabulary Master",
                Description = "Master 50 vocabulary words",
                AchievementType = "vocabulary_milestone",
                RequirementCriteria = "{\"words_mastered\": 50}",
                Points = 75,
                BadgeIconUrl = "/badges/vocab-master.png",
                IsActive = true,
                CreatedAt = now
            },
            new Achievement
            {
                Id = Guid.Parse("0000b005-0000-0000-0000-000000000005"),
                Title = "Practice Champion",
                Description = "Complete 50 practice sessions",
                AchievementType = "practice_milestone",
                RequirementCriteria = "{\"sessions_completed\": 50}",
                Points = 150,
                BadgeIconUrl = "/badges/champion.png",
                IsActive = true,
                CreatedAt = now
            }
        };

        await context.Achievements.AddRangeAsync(achievements);

        await context.SaveChangesAsync();
    }
}

