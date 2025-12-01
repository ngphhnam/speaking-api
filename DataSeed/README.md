# Sample Data Seeding Guide

This directory contains sample data for the Speaking Practice API database.

## Files

- `SampleDataSeeder.cs` - C# class with Entity Framework seed data
- `../sample_data.sql` - SQL script with raw INSERT statements

## What Data is Included

### 1. Topics (10 topics)
- **Part 1 Topics**: Hometown, Work, Studies, Hobbies
- **Part 2 Topics**: Describe a Person, Describe a Place, Describe an Event
- **Part 3 Topics**: Education System, Technology Impact, Environment

### 2. Questions (8 questions)
- **Part 1 Questions**: Personal questions about hometown and work
- **Part 2 Questions**: Cue card questions for long turn speaking
- **Part 3 Questions**: Abstract discussion questions

### 3. Vocabulary (5 words)
- IELTS vocabulary words with:
  - Phonetic transcriptions
  - Definitions (English & Vietnamese)
  - Example sentences
  - Synonyms, antonyms, collocations
  - Band level indicators

### 4. Achievements (5 achievements)
- Practice milestones
- Score achievements
- Vocabulary milestones
- Streak achievements

## How to Use

### Option 1: Using C# Seeder (Recommended)

Add this to your `Program.cs` or create a migration/seeding endpoint:

```csharp
using SpeakingPractice.Api.DataSeed;
using SpeakingPractice.Api.Infrastructure.Persistence;

// In your Program.cs or startup code
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SampleDataSeeder.SeedAsync(context);
}
```

### Option 2: Using SQL Script

1. Connect to your PostgreSQL database
2. Run the `sample_data.sql` file:
   ```bash
   psql -U your_username -d your_database -f sample_data.sql
   ```
   Or use your preferred database tool (pgAdmin, DBeaver, etc.)

### Option 3: Using Entity Framework Migrations

You can also create a migration that includes this seed data:

```bash
dotnet ef migrations add SeedSampleData
```

Then modify the migration's `Up()` method to call the seeder.

## Data Relationships

The sample data includes:
- Topics → Questions (one-to-many)
- Questions can reference Topics
- Vocabulary is standalone (can be linked to users via UserVocabulary)
- Achievements are standalone (can be linked to users via UserAchievements)

## User-Related Data

For tables that require user IDs (PracticeSessions, Recordings, AnalysisResults, etc.), you'll need to:

1. **Create users first** through your API registration endpoint
2. **Get the user IDs** (GUIDs) from the database
3. **Insert user-related data** using those IDs

Example SQL for user-related data (replace `USER_ID` with actual GUIDs):

```sql
-- Practice Session
INSERT INTO practice_sessions (id, user_id, session_type, part_number, topic_id, ...)
VALUES ('session-id', 'USER_ID', 'practice', 2, 'topic-id', ...);

-- Recording
INSERT INTO recordings (id, session_id, user_id, question_id, ...)
VALUES ('recording-id', 'session-id', 'USER_ID', 'question-id', ...);
```

## Customization

You can modify the seed data to:
- Add more topics/questions
- Adjust difficulty levels
- Add more vocabulary words
- Create custom achievements
- Change ratings and usage counts

## Notes

- The seeder checks if data already exists to avoid duplicates
- All timestamps use `DateTimeOffset.UtcNow`
- GUIDs are fixed for consistency (you can change them if needed)
- The data is designed to be realistic for IELTS speaking practice

## Testing

After seeding, you can test your API endpoints:
- `GET /api/topics` - Should return 10 topics
- `GET /api/questions` - Should return 8 questions
- `GET /api/vocabulary` - Should return 5 vocabulary words
- `GET /api/achievements` - Should return 5 achievements



