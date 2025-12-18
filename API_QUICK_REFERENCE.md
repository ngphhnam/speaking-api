# API Quick Reference - New Endpoints

## 🎯 Profile & Avatar

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/Auth/upload-avatar` | Upload profile image (max 10MB) |
| DELETE | `/api/Auth/avatar` | Remove profile image |
| PUT | `/api/Auth/profile` | Update profile (name, phone, DOB, target score, etc.) |

## ⚙️ User Settings

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/user-settings` | Get user settings |
| PUT | `/api/user-settings` | Update settings (target score, level, exam date) |
| GET | `/api/user-settings/notification-preferences` | Get notification preferences |
| PUT | `/api/user-settings/notification-preferences` | Update notification preferences |
| GET | `/api/user-settings/practice-preferences` | Get practice preferences |
| PUT | `/api/user-settings/practice-preferences` | Update practice preferences |

## 🎓 Mock Tests

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/mock-tests/start` | Start a new mock test (Part 1+2+3) |
| GET | `/api/mock-tests/{id}` | Get mock test details |
| POST | `/api/mock-tests/{id}/submit-part` | Submit Part 1, 2, or 3 |
| POST | `/api/mock-tests/{id}/complete` | Complete mock test & calculate score |
| GET | `/api/mock-tests/user/{userId}/history` | Get user's mock test history |
| DELETE | `/api/mock-tests/{id}` | Delete mock test |

## 🏆 Leaderboard

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/leaderboard/top-scores` | Top users by average score |
| GET | `/api/leaderboard/top-streaks` | Top users by practice streak |
| GET | `/api/leaderboard/top-practice-time` | Top users by practice time |
| GET | `/api/leaderboard/my-rank` | Get current user's rank |

### Query Parameters for Leaderboard:
- `period`: "all", "week", "month" (for scores & practice time)
- `limit`: Number of results (default: 50)
- `category`: "score", "streak", "practice-time" (for my-rank)

## 📝 Enhanced Feedback Management

| Method | Endpoint | Description |
|--------|----------|-------------|
| PUT | `/api/analysis-results/{id}/mark-reviewed` | Mark feedback as reviewed |
| POST | `/api/analysis-results/{id}/notes` | Add personal note to feedback |
| GET | `/api/analysis-results/user/{userId}/by-skill` | Filter feedback by skill |

### Query Parameters for Feedback:
- `skill`: "grammar", "vocabulary", "fluency", "pronunciation"

---

## 📊 API Coverage by Page

| # | Page | Status | Key Endpoints |
|---|------|--------|---------------|
| 1 | Dashboard | ✅ Ready | `/api/UserProgress/user/{id}/statistics`, `/api/UserProgress/user/{id}/trends` |
| 2 | Learning History | ✅ Ready | `/api/Recordings/user/{id}`, `/api/session/speaking-session` |
| 3 | Detailed Reports | ✅ Ready | `/api/AnalysisResults/user/{id}/trends`, `/api/AnalysisResults/user/{id}/statistics` |
| 4 | Vocabulary | ✅ Ready | `/api/UserVocabulary/user/{id}/*`, `/api/Vocabulary/search` |
| 5 | Mock Test | ✅ Ready | `/api/mock-tests/*` |
| 6 | Personal Profile | ✅ Ready | `/api/Auth/profile`, `/api/Auth/upload-avatar` |
| 7 | Settings | ✅ Ready | `/api/user-settings/*` |
| 8 | Leaderboard | ✅ Ready | `/api/leaderboard/*` |
| 9 | Community/Forum | ❌ Not Ready | Need to implement |
| 10 | Study Plan | ❌ Not Ready | Need to implement |
| 11 | Resources | ⚠️ Partial | Has topics/questions, needs file uploads |
| 12 | Feedback Management | ✅ Ready | `/api/analysis-results/*` |
| 13 | Answer Comparison | ✅ Ready | `/api/Recordings/compare` |
| 14 | Pronunciation Practice | ⚠️ Partial | Has basic scores, needs word-level analysis |
| 15 | Admin Dashboard | ⚠️ Partial | Has basic endpoints, needs full analytics |

---

## 🚀 Quick Start Examples

### 1. Upload Avatar
```javascript
const formData = new FormData();
formData.append('avatar', avatarFile);

const response = await fetch('/api/Auth/upload-avatar', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});
```

### 2. Start Mock Test
```javascript
const response = await fetch('/api/mock-tests/start', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    part1QuestionCount: 3,
    part2QuestionCount: 1,
    part3QuestionCount: 4
  })
});
```

### 3. Get Leaderboard
```javascript
const response = await fetch('/api/leaderboard/top-scores?period=month&limit=10');
const data = await response.json();
```

### 4. Update User Settings
```javascript
const response = await fetch('/api/user-settings', {
  method: 'PUT',
  headers: {
    'Authorization': `Bearer ${token}`,
    'Content-Type': 'application/json'
  },
  body: JSON.stringify({
    targetBandScore: 7.5,
    currentLevel: 'advanced',
    examDate: '2024-12-31'
  })
});
```

---

## 📋 Before You Start

1. **Run Migration for MockTest**:
   ```bash
   dotnet ef migrations add AddMockTestEntity
   dotnet ef database update
   ```

2. **Verify MinIO Configuration**:
   - Ensure MinIO is running
   - Check `appsettings.json` for correct MinIO settings
   - Test avatar upload to ensure "avatars" bucket is created

3. **Test Authentication**:
   - All new endpoints (except leaderboard) require authentication
   - Use `/api/Auth/login` to get access token
   - Include token in `Authorization: Bearer {token}` header

---

## 🎨 Frontend Integration Tips

### Avatar Display
```javascript
// After upload, the avatar URL is returned
<img src={user.avatarUrl} alt="Profile" />

// Handle no avatar
<img src={user.avatarUrl || '/default-avatar.png'} alt="Profile" />
```

### Mock Test Flow
```javascript
// 1. Start test
const test = await startMockTest();

// 2. Record answers for each part
// ... user records answers ...

// 3. Submit each part as completed
await submitPart(test.id, 1); // Part 1
await submitPart(test.id, 2); // Part 2
await submitPart(test.id, 3); // Part 3

// 4. Complete test (calculates final score)
const result = await completeMockTest(test.id);
console.log('Final Score:', result.overallScore);
```

### Leaderboard with Rank Highlight
```javascript
const leaderboard = await getTopScores('month', 50);
const myRank = await getMyRank('score', 'month');

// Highlight current user in leaderboard
const highlightedLeaderboard = leaderboard.map(entry => ({
  ...entry,
  isCurrentUser: entry.userId === myRank.userId
}));
```

---

## 🔧 Troubleshooting

### Avatar Upload Fails
- Check file size < 10MB
- Verify file type is JPEG, PNG, GIF, or WEBP
- Ensure MinIO is accessible

### Mock Test Not Saving
- Verify database migration was run
- Check that questions exist in database
- Ensure user is authenticated

### Leaderboard Empty
- Need analysis results with scores in database
- Check date filters (week/month)
- Verify users have completed recordings

---

## 📞 Support

If you encounter issues:
1. Check API logs in console
2. Verify authentication token is valid
3. Ensure all dependencies are installed
4. Run database migrations
5. Check MinIO connectivity

**Happy Coding! 🎉**

