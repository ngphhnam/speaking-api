using SpeakingPractice.Api.Domain.Entities;
using SpeakingPractice.Api.Domain.Enums;
using SpeakingPractice.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace SpeakingPractice.Api.DataSeed;

/// <summary>
/// Seeds IELTS Speaking topics and questions into the database
/// </summary>
public static class IELTSTopicsSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        // Check if topics already exist
        if (await context.Topics.AnyAsync())
        {
            Console.WriteLine("Topics already exist. Skipping IELTS topics seed.");
            return;
        }

        Console.WriteLine("Seeding IELTS Speaking topics...");

        var now = DateTimeOffset.UtcNow;
        var topics = new List<Topic>();
        var questions = new List<Question>();

        #region Part 1 Topics (30 topics)

        // 1. Work
        var workTopic = CreatePart1Topic("Work", "work", "Questions about your job, career, and workplace", "personal", now);
        topics.Add(workTopic);
        questions.AddRange(new[]
        {
            CreateQuestion(workTopic.Id, "What do you do? / What is your job?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(workTopic.Id, "What are your main responsibilities at work?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(workTopic.Id, "Do you like your job? Why or why not?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(workTopic.Id, "What do you like most about your job?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(workTopic.Id, "Is there anything you don't like about your job?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(workTopic.Id, "What time do you usually start work?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(workTopic.Id, "How do you get to work?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(workTopic.Id, "Did you have to do much training for your job?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(workTopic.Id, "What did you want to be when you were younger?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(workTopic.Id, "Do you plan to continue doing this job in the future?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(workTopic.Id, "What's the most important thing about a job for you?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(workTopic.Id, "Would you recommend your job to others?", QuestionType.PART1, QuestionStyle.YesNo, 30, now)
        });

        // 2. Study
        var studyTopic = CreatePart1Topic("Study", "study", "Questions about your education and studies", "education", now);
        topics.Add(studyTopic);
        questions.AddRange(new[]
        {
            CreateQuestion(studyTopic.Id, "What are you studying?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(studyTopic.Id, "Why did you choose this subject?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(studyTopic.Id, "Do you enjoy studying this subject?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(studyTopic.Id, "What's your favorite subject?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(studyTopic.Id, "What do you find most difficult about studying?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(studyTopic.Id, "Do you prefer to study alone or with others?", QuestionType.PART1, QuestionStyle.MultipleChoice, 30, now),
            CreateQuestion(studyTopic.Id, "Where do you usually study?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(studyTopic.Id, "What time of day do you prefer to study?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(studyTopic.Id, "What do you plan to do after you finish your studies?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(studyTopic.Id, "Would you like to study abroad?", QuestionType.PART1, QuestionStyle.YesNo, 30, now)
        });

        // 3. Hometown
        var hometownTopic = CreatePart1Topic("Hometown", "hometown", "Questions about where you live and your local area", "personal", now);
        topics.Add(hometownTopic);
        questions.AddRange(new[]
        {
            CreateQuestion(hometownTopic.Id, "Where are you from?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(hometownTopic.Id, "What's your hometown like?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hometownTopic.Id, "What do you like most about your hometown?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hometownTopic.Id, "Is there anything you don't like about it?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(hometownTopic.Id, "How long have you lived there?", QuestionType.PART1, QuestionStyle.OpenEnded, 25, now),
            CreateQuestion(hometownTopic.Id, "Do you think you will live there in the future?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(hometownTopic.Id, "What's the most interesting part of your town/city?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hometownTopic.Id, "Has your hometown changed much in recent years?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(hometownTopic.Id, "Would you recommend your hometown to visitors?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(hometownTopic.Id, "What kind of jobs do people do in your hometown?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now)
        });

        // 4. Family
        var familyTopic = CreatePart1Topic("Family", "family", "Questions about your family and relatives", "personal", now);
        topics.Add(familyTopic);
        questions.AddRange(new[]
        {
            CreateQuestion(familyTopic.Id, "Tell me about your family.", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(familyTopic.Id, "Do you have a large family or a small family?", QuestionType.PART1, QuestionStyle.MultipleChoice, 30, now),
            CreateQuestion(familyTopic.Id, "How much time do you spend with your family?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(familyTopic.Id, "What do you like to do together as a family?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(familyTopic.Id, "Who are you closest to in your family?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(familyTopic.Id, "Do you get along well with your family?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(familyTopic.Id, "Are people in your country generally close to their families?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(familyTopic.Id, "Is family important in your culture?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(familyTopic.Id, "Would you prefer to have a big family or a small family?", QuestionType.PART1, QuestionStyle.MultipleChoice, 30, now),
            CreateQuestion(familyTopic.Id, "Do you often see your relatives?", QuestionType.PART1, QuestionStyle.YesNo, 30, now)
        });

        // 5. Hobbies
        var hobbiesTopic = CreatePart1Topic("Hobbies", "hobbies", "Questions about your interests and leisure activities", "lifestyle", now);
        topics.Add(hobbiesTopic);
        questions.AddRange(new[]
        {
            CreateQuestion(hobbiesTopic.Id, "What do you like to do in your free time?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hobbiesTopic.Id, "Do you have any hobbies?", QuestionType.PART1, QuestionStyle.YesNo, 30, now),
            CreateQuestion(hobbiesTopic.Id, "How long have you had this hobby?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(hobbiesTopic.Id, "How did you become interested in this hobby?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hobbiesTopic.Id, "What hobbies are popular in your country?", QuestionType.PART1, QuestionStyle.OpenEnded, 35, now),
            CreateQuestion(hobbiesTopic.Id, "Do you think it's important to have hobbies?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(hobbiesTopic.Id, "How much time do you spend on your hobbies?", QuestionType.PART1, QuestionStyle.OpenEnded, 30, now),
            CreateQuestion(hobbiesTopic.Id, "Is there a hobby you would like to try in the future?", QuestionType.PART1, QuestionStyle.YesNo, 35, now),
            CreateQuestion(hobbiesTopic.Id, "Do you prefer indoor or outdoor hobbies?", QuestionType.PART1, QuestionStyle.MultipleChoice, 30, now),
            CreateQuestion(hobbiesTopic.Id, "Have your hobbies changed since you were a child?", QuestionType.PART1, QuestionStyle.YesNo, 35, now)
        });

        #endregion

        #region Part 2 & 3 Topics (10 sample topics)

        // 1. Describe a Person You Admire
        var personAdmireTopic = CreatePart2Topic("Describe a Person You Admire", "describe-person-admire", "Describe a person who has influenced you", "people", now);
        topics.Add(personAdmireTopic);
        questions.Add(CreateCueCard(personAdmireTopic.Id, 
            "Describe a person you admire.\n\nYou should say:\n• who this person is\n• how you know this person\n• what this person is like\n• and explain why you admire this person",
            now));
        questions.AddRange(new[]
        {
            CreateQuestion(personAdmireTopic.Id, "What qualities make someone a good role model?", QuestionType.PART3, QuestionStyle.Opinion, 45, now),
            CreateQuestion(personAdmireTopic.Id, "Do you think celebrities have a responsibility to be role models?", QuestionType.PART3, QuestionStyle.Opinion, 45, now),
            CreateQuestion(personAdmireTopic.Id, "How has the concept of role models changed over time?", QuestionType.PART3, QuestionStyle.Comparison, 50, now),
            CreateQuestion(personAdmireTopic.Id, "In what ways can parents influence their children?", QuestionType.PART3, QuestionStyle.OpenEnded, 50, now),
            CreateQuestion(personAdmireTopic.Id, "Should successful people share their experiences with others?", QuestionType.PART3, QuestionStyle.Opinion, 45, now)
        });

        // 2. Describe a Place You Visited
        var placeVisitedTopic = CreatePart2Topic("Describe a Place You Visited", "describe-place-visited", "Describe a memorable place you visited", "places", now);
        topics.Add(placeVisitedTopic);
        questions.Add(CreateCueCard(placeVisitedTopic.Id,
            "Describe a place you visited that you found interesting.\n\nYou should say:\n• where it is\n• when you went there\n• what you did there\n• and explain why you found it interesting",
            now));
        questions.AddRange(new[]
        {
            CreateQuestion(placeVisitedTopic.Id, "Why do people like to travel?", QuestionType.PART3, QuestionStyle.OpenEnded, 45, now),
            CreateQuestion(placeVisitedTopic.Id, "How has tourism affected your country?", QuestionType.PART3, QuestionStyle.CauseEffect, 50, now),
            CreateQuestion(placeVisitedTopic.Id, "What are the advantages and disadvantages of tourism?", QuestionType.PART3, QuestionStyle.Comparison, 50, now),
            CreateQuestion(placeVisitedTopic.Id, "Do you think mass tourism damages local culture?", QuestionType.PART3, QuestionStyle.Opinion, 50, now),
            CreateQuestion(placeVisitedTopic.Id, "How will travel change in the future?", QuestionType.PART3, QuestionStyle.Prediction, 45, now)
        });

        // 3. Describe Something You Own
        var somethingOwnTopic = CreatePart2Topic("Describe Something You Own", "describe-something-own", "Describe something important to you", "objects", now);
        topics.Add(somethingOwnTopic);
        questions.Add(CreateCueCard(somethingOwnTopic.Id,
            "Describe something you own that is important to you.\n\nYou should say:\n• what it is\n• when you got it\n• how you use it\n• and explain why it is important to you",
            now));
        questions.AddRange(new[]
        {
            CreateQuestion(somethingOwnTopic.Id, "Do you think people buy too many things nowadays?", QuestionType.PART3, QuestionStyle.Opinion, 45, now),
            CreateQuestion(somethingOwnTopic.Id, "How has shopping behavior changed with technology?", QuestionType.PART3, QuestionStyle.Comparison, 50, now),
            CreateQuestion(somethingOwnTopic.Id, "Is it better to buy quality items or cheaper ones?", QuestionType.PART3, QuestionStyle.Comparison, 45, now),
            CreateQuestion(somethingOwnTopic.Id, "What influence does advertising have on what people buy?", QuestionType.PART3, QuestionStyle.CauseEffect, 50, now)
        });

        // 4. Describe a Memorable Event
        var memorableEventTopic = CreatePart2Topic("Describe a Memorable Event", "describe-memorable-event", "Describe an event that was memorable", "events", now);
        topics.Add(memorableEventTopic);
        questions.Add(CreateCueCard(memorableEventTopic.Id,
            "Describe a memorable event in your life.\n\nYou should say:\n• when it happened\n• where it happened\n• what you did\n• and explain why it was memorable",
            now));
        questions.AddRange(new[]
        {
            CreateQuestion(memorableEventTopic.Id, "What types of events do people celebrate in your country?", QuestionType.PART3, QuestionStyle.OpenEnded, 45, now),
            CreateQuestion(memorableEventTopic.Id, "Do you think traditional celebrations are still important?", QuestionType.PART3, QuestionStyle.Opinion, 45, now),
            CreateQuestion(memorableEventTopic.Id, "How have celebrations changed over time?", QuestionType.PART3, QuestionStyle.Comparison, 50, now),
            CreateQuestion(memorableEventTopic.Id, "Why do people like to celebrate special occasions?", QuestionType.PART3, QuestionStyle.OpenEnded, 45, now)
        });

        // 5. Describe a Hobby
        var hobbyTopic = CreatePart2Topic("Describe a Hobby", "describe-hobby", "Describe a hobby you enjoy", "activities", now);
        topics.Add(hobbyTopic);
        questions.Add(CreateCueCard(hobbyTopic.Id,
            "Describe a hobby you enjoy.\n\nYou should say:\n• what the hobby is\n• when you started it\n• how often you do it\n• and explain why you enjoy it",
            now));
        questions.AddRange(new[]
        {
            CreateQuestion(hobbyTopic.Id, "What are the benefits of having hobbies?", QuestionType.PART3, QuestionStyle.OpenEnded, 45, now),
            CreateQuestion(hobbyTopic.Id, "Do you think children have enough time for hobbies today?", QuestionType.PART3, QuestionStyle.Opinion, 45, now),
            CreateQuestion(hobbyTopic.Id, "How have people's hobbies changed with technology?", QuestionType.PART3, QuestionStyle.Comparison, 50, now),
            CreateQuestion(hobbyTopic.Id, "Should schools encourage students to develop hobbies?", QuestionType.PART3, QuestionStyle.Opinion, 45, now)
        });

        #endregion

        // Add to database
        await context.Topics.AddRangeAsync(topics);
        await context.Questions.AddRangeAsync(questions);
        await context.SaveChangesAsync();

        Console.WriteLine($"✅ Seeded {topics.Count} topics and {questions.Count} questions successfully!");
    }

    private static Topic CreatePart1Topic(string title, string slug, string description, string category, DateTimeOffset now)
    {
        return new Topic
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Description = description,
            PartNumber = 1,
            DifficultyLevel = "beginner",
            TopicCategory = category,
            Keywords = new[] { slug.Replace("-", " ") },
            UsageCount = 0,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Topic CreatePart2Topic(string title, string slug, string description, string category, DateTimeOffset now)
    {
        return new Topic
        {
            Id = Guid.NewGuid(),
            Title = title,
            Slug = slug,
            Description = description,
            PartNumber = 2,
            DifficultyLevel = "intermediate",
            TopicCategory = category,
            Keywords = new[] { category },
            UsageCount = 0,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Question CreateQuestion(Guid topicId, string text, QuestionType type, QuestionStyle? style, int timeLimit, DateTimeOffset now)
    {
        return new Question
        {
            Id = Guid.NewGuid(),
            TopicId = topicId,
            QuestionText = text,
            QuestionType = type,
            QuestionStyle = style,
            TimeLimitSeconds = timeLimit,
            AttemptsCount = 0,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    private static Question CreateCueCard(Guid topicId, string text, DateTimeOffset now)
    {
        return new Question
        {
            Id = Guid.NewGuid(),
            TopicId = topicId,
            QuestionText = text,
            QuestionType = QuestionType.PART2,
            QuestionStyle = QuestionStyle.CueCard,
            TimeLimitSeconds = 120,
            AttemptsCount = 0,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}

