# HTTP Testing Guide

This guide explains how to use the `SpeakingPractice.Api.http` file to test your APIs.

## 🚀 Quick Start

### 1. Set Up Variables
At the top of the file, update these variables:

```http
@baseUrl = http://localhost:5260
@accessToken = YOUR_ACCESS_TOKEN_HERE
@userId = YOUR_USER_ID_HERE
```

### 2. Start Your API
```bash
dotnet run
```

### 3. Test Authentication Flow

#### Step 1: Register a new user
Find this request and click "Send Request" (VS Code) or use the REST Client extension:
```http
POST {{baseUrl}}/api/Auth/register
```

#### Step 2: Login
```http
POST {{baseUrl}}/api/Auth/login
```

**Copy the `accessToken` from the response** and update the variable at the top:
```http
@accessToken = eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

#### Step 3: Get your profile
```http
GET {{baseUrl}}/api/Auth/me
```

**Copy your `id` from the response** and update:
```http
@userId = 12345678-1234-1234-1234-123456789abc
```

## 📋 Testing Sections

The file is organized into these sections:

### 1. **Authentication & User Management**
- Register, login, logout
- Profile management
- Avatar upload/delete
- Change password

### 2. **User Settings & Preferences**
- General settings (target score, exam date)
- Notification preferences
- Practice preferences

### 3. **Topics**
- Browse topics
- Create/update topics
- Get popular/recommended topics
- Rate topics

### 4. **Questions**
- Browse questions
- Get questions by topic
- Get random questions
- Generate vocabulary for questions

### 5. **Mock Tests** ⭐ NEW
- Start full mock test (Part 1+2+3)
- Submit individual parts
- Complete test with scoring
- View test history

### 6. **Speaking Sessions**
- Browse sessions
- Get active/completed sessions
- Session statistics

### 7. **Recordings**
- Browse recordings
- Get recording analysis
- Refine recordings
- Compare versions

### 8. **Analysis Results & Feedback** ⭐ ENHANCED
- Get analysis results
- View trends and statistics
- Mark feedback as reviewed
- Add personal notes
- Filter by skill

### 9. **User Progress & Statistics**
- Daily/weekly/monthly progress
- Statistics and trends
- Improvement tracking

### 10. **Vocabulary Management**
- Search vocabulary
- Browse by band level
- Browse by topic
- Create vocabulary (Admin/Teacher)

### 11. **User Vocabulary (Personal Learning)**
- Personal vocabulary list
- Learning/reviewing/mastered words
- Flashcard system with spaced repetition
- Add personal notes

### 12. **Achievements**
- Browse achievements
- User achievement progress
- Completed achievements

### 13. **Leaderboard** ⭐ NEW
- Top scores (all time/week/month)
- Top streaks
- Top practice time
- Get your personal rank

### 14. **Admin - User Management**
- Manage users (Admin only)
- Assign roles
- Lock/unlock users
- Platform statistics

### 15. **Health Check**
- API health status

## 🎯 Testing Workflows

### Workflow 1: Complete Mock Test
```
1. Start mock test → GET mock test ID
2. Record answers for Part 1 questions
3. Submit Part 1 → POST /submit-part {"part": 1}
4. Record answers for Part 2
5. Submit Part 2 → POST /submit-part {"part": 2}
6. Record answers for Part 3
7. Submit Part 3 → POST /submit-part {"part": 3}
8. Complete test → POST /complete
9. View test history
```

### Workflow 2: Vocabulary Learning
```
1. Search vocabulary → GET /search?q=word
2. Add to personal list → POST /UserVocabulary
3. Review vocabulary → GET /due-for-review
4. Mark as reviewed → PUT /{id}/review
5. Check statistics → GET /statistics
```

### Workflow 3: Check Your Progress
```
1. Get statistics → GET /UserProgress/statistics
2. View trends → GET /UserProgress/trends
3. Check leaderboard rank → GET /leaderboard/my-rank
4. View weak areas → GET /analysis-results/weak-areas
5. View improvement → GET /UserProgress/improvement
```

## 🔧 Tools for Testing

### VS Code (Recommended)
1. Install **REST Client** extension by Huachao Mao
2. Open `.http` file
3. Click "Send Request" above any request
4. View response in new panel

### Postman
1. Import the requests manually
2. Set up environment variables
3. For file uploads (avatar), use Postman's form-data

### cURL (Command Line)
Convert any request to cURL:
```bash
curl -X POST "http://localhost:5260/api/Auth/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"test@example.com","password":"Test@123456"}'
```

## ⚠️ Important Notes

### File Uploads (Avatar)
The avatar upload endpoint requires `multipart/form-data`. Use Postman or a similar tool:
```
POST http://localhost:5260/api/Auth/upload-avatar
Authorization: Bearer YOUR_TOKEN
Content-Type: multipart/form-data

Body:
- avatar: [Select File]
```

### Authentication
Most endpoints require authentication. Steps:
1. Login to get access token
2. Copy the token
3. Update `@accessToken` variable
4. Token is automatically included in authenticated requests

### ID Placeholders
Replace these placeholders with actual IDs from your database:
- `YOUR_TOPIC_ID_HERE`
- `YOUR_QUESTION_ID_HERE`
- `YOUR_RECORDING_ID_HERE`
- `YOUR_MOCK_TEST_ID_HERE`
- etc.

### Database Setup
Before testing, ensure:
1. Database is running and migrated
2. Some seed data exists (topics, questions)
3. MinIO is running (for avatar upload)

## 📊 Response Examples

### Successful Response (200 OK)
```json
{
  "data": {
    "id": "123-456",
    "fullName": "John Doe",
    "email": "john@example.com"
  },
  "message": "Success",
  "success": true
}
```

### Error Response (400 Bad Request)
```json
{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid input",
    "details": {
      "errors": ["Email is required"]
    }
  },
  "success": false
}
```

### Unauthorized (401)
```json
{
  "error": {
    "code": "UNAUTHORIZED",
    "message": "User not authenticated"
  },
  "success": false
}
```

## 🎓 Pro Tips

1. **Save Responses**: Save successful responses to use IDs in subsequent requests

2. **Test in Order**: Some endpoints depend on others (e.g., need recording ID before getting analysis)

3. **Use Variables**: Update variables at the top instead of editing each request

4. **Check Logs**: Monitor your API logs to see what's happening

5. **Test Edge Cases**: Try invalid data to test error handling

6. **Leaderboard**: Need multiple users with data to see meaningful leaderboard results

7. **Mock Tests**: Need questions in database before starting a mock test

## 🐛 Troubleshooting

### 401 Unauthorized
- Check if token is valid
- Token might be expired (login again)
- Ensure `Authorization: Bearer` header is included

### 404 Not Found
- Verify the ID exists in database
- Check the endpoint URL
- Ensure database is seeded

### 500 Internal Server Error
- Check API logs
- Verify database connection
- Ensure all dependencies are running (MinIO, Database)

### Empty Leaderboard
- Need users with completed recordings and analysis results
- Try creating mock data with multiple users

## 📝 Next Steps

After testing:
1. Review the responses
2. Integrate with your frontend
3. Check `NEW_APIS_DOCUMENTATION.md` for detailed API specs
4. Check `API_QUICK_REFERENCE.md` for quick reference

Happy Testing! 🎉





