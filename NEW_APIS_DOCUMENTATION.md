# New APIs Added - Complete Documentation

This document lists all the new APIs that have been added to support the 15 pages in your IELTS Speaking Practice application.

## 1. Profile Image Upload APIs

### Upload Avatar
**POST** `/api/Auth/upload-avatar`
- **Authorization**: Required
- **Content-Type**: `multipart/form-data`
- **Request Body**: 
  - `avatar`: IFormFile (max 10MB)
  - Allowed types: JPEG, PNG, GIF, WEBP
- **Response**: 
```json
{
  "data": {
    "avatarUrl": "http://minio:9000/avatars/avatar_userid_timestamp.jpg",
    "user": { /* UserDto */ }
  },
  "message": "Avatar uploaded successfully"
}
```

### Delete Avatar
**DELETE** `/api/Auth/avatar`
- **Authorization**: Required
- **Response**: Removes the user's avatar URL

### Enhanced Profile Update
**PUT** `/api/Auth/profile`
- **Authorization**: Required
- **Request Body**:
```json
{
  "fullName": "John Doe",
  "phone": "+1234567890",
  "dateOfBirth": "1990-01-01",
  "targetBandScore": 7.5,
  "currentLevel": "intermediate",
  "examDate": "2024-12-31"
}
```

---

## 2. User Settings APIs

### Get User Settings
**GET** `/api/user-settings`
- **Authorization**: Required
- **Response**: Returns user settings including target score, exam date, subscription info

### Update User Settings
**PUT** `/api/user-settings`
- **Authorization**: Required
- **Request Body**:
```json
{
  "targetBandScore": 7.5,
  "currentLevel": "advanced",
  "examDate": "2024-12-31"
}
```

### Get Notification Preferences
**GET** `/api/user-settings/notification-preferences`
- **Authorization**: Required
- **Response**:
```json
{
  "userId": "guid",
  "emailNotifications": true,
  "practiceReminders": true,
  "achievementNotifications": true,
  "weeklySummary": true,
  "streakReminders": true
}
```

### Update Notification Preferences
**PUT** `/api/user-settings/notification-preferences`
- **Authorization**: Required
- **Request Body**: Same structure as GET response

### Get Practice Preferences
**GET** `/api/user-settings/practice-preferences`
- **Authorization**: Required
- **Response**:
```json
{
  "userId": "guid",
  "recordingQuality": "high",
  "autoSubmit": false,
  "feedbackDetailLevel": "detailed",
  "preferredAIModel": "llama",
  "showHints": true,
  "enableTimer": true
}
```

### Update Practice Preferences
**PUT** `/api/user-settings/practice-preferences`
- **Authorization**: Required
- **Request Body**: Same structure as GET response

---

## 3. Mock Test APIs

### Start Mock Test
**POST** `/api/mock-tests/start`
- **Authorization**: Required
- **Request Body** (optional):
```json
{
  "part1QuestionCount": 3,
  "part2QuestionCount": 1,
  "part3QuestionCount": 4
}
```
- **Response**: Returns mock test with all questions for Part 1, 2, and 3

### Get Mock Test by ID
**GET** `/api/mock-tests/{id}`
- **Authorization**: Required
- **Response**: Returns complete mock test details

### Submit Part
**POST** `/api/mock-tests/{id}/submit-part`
- **Authorization**: Required
- **Request Body**:
```json
{
  "part": 1
}
```
- **Description**: Marks Part 1, 2, or 3 as completed

### Complete Mock Test
**POST** `/api/mock-tests/{id}/complete`
- **Authorization**: Required
- **Description**: Marks the entire mock test as completed and calculates overall score

### Get User Mock Test History
**GET** `/api/mock-tests/user/{userId}/history`
- **Authorization**: Required
- **Response**: Returns list of all mock tests taken by the user

### Delete Mock Test
**DELETE** `/api/mock-tests/{id}`
- **Authorization**: Required
- **Description**: Deletes a mock test

---

## 4. Leaderboard APIs

### Get Top Scores
**GET** `/api/leaderboard/top-scores?period=all&limit=50`
- **Authorization**: Not required
- **Query Parameters**:
  - `period`: "all", "week", "month"
  - `limit`: Number of results (default: 50)
- **Response**:
```json
[
  {
    "rank": 1,
    "userId": "guid",
    "fullName": "John Doe",
    "avatarUrl": "url",
    "avgScore": 7.5,
    "totalRecordings": 50,
    "bestScore": 8.0
  }
]
```

### Get Top Streaks
**GET** `/api/leaderboard/top-streaks?limit=50`
- **Authorization**: Not required
- **Response**:
```json
[
  {
    "rank": 1,
    "userId": "guid",
    "fullName": "John Doe",
    "avatarUrl": "url",
    "currentStreak": 30
  }
]
```

### Get Top Practice Time
**GET** `/api/leaderboard/top-practice-time?period=month&limit=50`
- **Authorization**: Not required
- **Query Parameters**:
  - `period`: "week", "month", "all"
  - `limit`: Number of results
- **Response**:
```json
[
  {
    "rank": 1,
    "userId": "guid",
    "fullName": "John Doe",
    "avatarUrl": "url",
    "totalMinutes": 1200,
    "totalSessions": 50
  }
]
```

### Get My Rank
**GET** `/api/leaderboard/my-rank?category=score&period=all`
- **Authorization**: Required
- **Query Parameters**:
  - `category`: "score", "streak", "practice-time"
  - `period`: "all", "week", "month"
- **Response**:
```json
{
  "userId": "guid",
  "fullName": "John Doe",
  "avatarUrl": "url",
  "rank": 42,
  "category": "score",
  "period": "all",
  "stats": {
    "avgScore": 7.2
  }
}
```

---

## 5. Enhanced Feedback Management APIs

### Mark Feedback as Reviewed
**PUT** `/api/analysis-results/{id}/mark-reviewed`
- **Authorization**: Required
- **Description**: Marks an analysis/feedback as reviewed

### Add Note to Feedback
**POST** `/api/analysis-results/{id}/notes`
- **Authorization**: Required
- **Request Body**:
```json
{
  "note": "Need to practice more vocabulary in this area"
}
```

### Get Feedback by Skill
**GET** `/api/analysis-results/user/{userId}/by-skill?skill=grammar`
- **Authorization**: Required
- **Query Parameters**:
  - `skill`: "grammar", "vocabulary", "fluency", "pronunciation"
- **Response**: Returns filtered and sorted analysis results

---

## Summary of API Coverage

### ✅ Fully Implemented Pages (10/15):
1. **Dashboard/Statistics** - Complete with progress, trends, streaks
2. **Learning History** - Complete with recordings and sessions
3. **Detailed Reports** - Complete with skill analysis and trends
4. **Vocabulary Page** - Complete with flashcards and spaced repetition
5. **Personal Profile** - Complete with avatar upload and full profile management
6. **Settings** - Complete with notification and practice preferences
7. **Leaderboard** - Complete with multiple categories
8. **Feedback Management** - Complete with review tracking and notes
9. **Answer Comparison** - Already existed
10. **Mock Test** - Complete with full test flow

### ⚠️ Partially Implemented (2/15):
11. **Resources/Materials** - Has topics and questions, missing file uploads
12. **Admin Dashboard** - Has basic endpoints, needs full analytics implementation

### ❌ Not Yet Implemented (3/15):
13. **Community/Forum** - Requires posts, comments, Q&A system
14. **Study Plan** - Requires plan management and goal tracking
15. **Pronunciation Practice** - Requires word-level analysis and IPA guide

---

## Database Migrations Required

To use the Mock Test functionality, you'll need to run a migration:

```bash
dotnet ef migrations add AddMockTestEntity
dotnet ef database update
```

Or create a configuration file for MockTest entity in the `Configurations` folder if you prefer explicit configuration.

---

## Testing the New APIs

### Example: Upload Avatar
```bash
curl -X POST "http://localhost:5000/api/Auth/upload-avatar" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -F "avatar=@profile.jpg"
```

### Example: Start Mock Test
```bash
curl -X POST "http://localhost:5000/api/mock-tests/start" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "part1QuestionCount": 3,
    "part2QuestionCount": 1,
    "part3QuestionCount": 4
  }'
```

### Example: Get Leaderboard
```bash
curl -X GET "http://localhost:5000/api/leaderboard/top-scores?period=month&limit=10"
```

---

## Next Steps

1. **Run database migrations** to add MockTest table
2. **Test avatar upload** with your MinIO setup
3. **Implement remaining pages** (Community, Study Plan, Pronunciation Practice)
4. **Add proper database tables** for notification/practice preferences
5. **Enhance admin analytics** with real-time data
6. **Add export functionality** for learning history

All APIs follow your existing patterns:
- Consistent error handling with `ErrorCodes`
- Authorization checks for protected resources
- ApiOk/ApiUnauthorized/ApiForbid response format
- Comprehensive logging
- Cancellation token support






