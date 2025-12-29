# 🧪 STREAK FEATURE - TESTING GUIDE

## Quick Test Scenarios

### 1. Test Login Response (Kiểm tra streak có xuất hiện)

```http
POST {{baseUrl}}/api/auth/login
Content-Type: application/json

{
  "email": "your-email@example.com",
  "password": "your-password"
}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "accessToken": "...",
    "user": {
      "id": "...",
      "email": "your-email@example.com",
      "fullName": "Your Name",
      "currentStreak": 0,        // ← Phải có field này
      "longestStreak": 0,        // ← Phải có field này
      "lastPracticeDate": null,  // ← Phải có field này
      "totalPracticeDays": 0     // ← Phải có field này
    }
  }
}
```

---

### 2. Test Create Practice Session (Kiểm tra auto-update streak)

```http
POST {{baseUrl}}/api/speaking-sessions
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "topicId": "a1b2c3d4-e5f6-4789-a012-345678901234"
}
```

**Check Logs:**
```
[Information] User {userId} started their first practice streak
hoặc
[Information] User {userId} continued their streak: 2 days
hoặc
[Information] User {userId} achieved new longest streak: 5 days!
```

---

### 3. Test User Statistics (Kiểm tra streak trong statistics)

```http
GET {{baseUrl}}/api/user-progress/user/{userId}/statistics
Authorization: Bearer {{accessToken}}
```

**Expected Response:**
```json
{
  "success": true,
  "data": {
    "userId": "...",
    "totalSessions": 5,
    "totalRecordings": 15,
    "totalPracticeMinutes": 75,
    "currentAvgScore": 6.5,
    "bestScore": 7.0,
    "improvementPercentage": 15.5,
    "currentStreak": 3,    // ← Từ database
    "longestStreak": 5     // ← Từ database
  }
}
```

---

### 4. Test Leaderboard (Kiểm tra performance)

```http
GET {{baseUrl}}/api/leaderboard/top-streaks?limit=10
```

**Expected Response:**
```json
{
  "success": true,
  "data": [
    {
      "rank": 1,
      "userId": "...",
      "fullName": "Top User",
      "avatarUrl": "/avatars/user.jpg",
      "currentStreak": 30,
      "longestStreak": 45,
      "lastPracticeDate": "2024-12-12"
    }
  ]
}
```

**Performance Check:**
- Response time phải < 50ms
- Nếu > 100ms thì có vấn đề

---

### 5. Test Streak Logic

#### Scenario A: First Practice
```
Day 1, 10:00 AM: Create session
→ current_streak = 1
→ longest_streak = 1
→ total_practice_days = 1
```

#### Scenario B: Same Day Multiple Sessions
```
Day 1, 10:00 AM: Create session → streak = 1
Day 1, 3:00 PM: Create session → streak = 1 (không đổi)
Day 1, 8:00 PM: Create session → streak = 1 (không đổi)
```

#### Scenario C: Consecutive Days
```
Day 1: Practice → streak = 1
Day 2: Practice → streak = 2
Day 3: Practice → streak = 3
Day 4: Practice → streak = 4
```

#### Scenario D: Streak Break
```
Day 1: Practice → streak = 1
Day 2: Practice → streak = 2
Day 3: SKIP
Day 4: Practice → streak = 1 (reset)
                  longest_streak = 2 (giữ nguyên)
```

#### Scenario E: New Record
```
Current: streak = 10, longest = 15
Day X: Practice → streak = 11
...
Day X+5: Practice → streak = 16, longest = 16 (NEW RECORD!)
```

---

## SQL Queries để Verify

### Check user streak:
```sql
SELECT 
    id,
    email,
    full_name,
    current_streak,
    longest_streak,
    last_practice_date,
    total_practice_days
FROM users
WHERE email = 'your-email@example.com';
```

### Check all users with streaks:
```sql
SELECT 
    full_name,
    current_streak,
    longest_streak,
    last_practice_date
FROM users
WHERE current_streak > 0
ORDER BY current_streak DESC;
```

### Manually update streak (for testing):
```sql
-- Set streak for testing
UPDATE users
SET current_streak = 5,
    longest_streak = 10,
    last_practice_date = CURRENT_DATE,
    total_practice_days = 20
WHERE email = 'your-email@example.com';
```

### Reset streak (for testing):
```sql
UPDATE users
SET current_streak = 0,
    longest_streak = 0,
    last_practice_date = NULL,
    total_practice_days = 0
WHERE email = 'your-email@example.com';
```

---

## Performance Testing

### Test Leaderboard Performance:
```bash
# Sử dụng curl để đo response time
time curl -X GET "http://localhost:5000/api/leaderboard/top-streaks?limit=50"

# Hoặc dùng Postman/Insomnia để xem response time
```

**Expected:**
- < 20ms: Excellent ✅
- 20-50ms: Good ✅
- 50-100ms: Acceptable ⚠️
- > 100ms: Need optimization ❌

---

## Automated Test (Optional)

### C# Unit Test Example:
```csharp
[Fact]
public async Task UpdateStreak_FirstPractice_ShouldSetStreakToOne()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new ApplicationUser { Id = userId };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    
    // Act
    var result = await streakService.UpdateStreakAsync(userId);
    
    // Assert
    Assert.Equal(1, result.CurrentStreak);
    Assert.Equal(1, result.LongestStreak);
    Assert.True(result.IsNewRecord);
}

[Fact]
public async Task UpdateStreak_ConsecutiveDay_ShouldIncreaseStreak()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new ApplicationUser 
    { 
        Id = userId,
        CurrentStreak = 5,
        LongestStreak = 10,
        LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-1))
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    
    // Act
    var result = await streakService.UpdateStreakAsync(userId);
    
    // Assert
    Assert.Equal(6, result.CurrentStreak);
    Assert.Equal(10, result.LongestStreak);
    Assert.True(result.StreakContinued);
}

[Fact]
public async Task UpdateStreak_SkippedDays_ShouldResetStreak()
{
    // Arrange
    var userId = Guid.NewGuid();
    var user = new ApplicationUser 
    { 
        Id = userId,
        CurrentStreak = 5,
        LongestStreak = 10,
        LastPracticeDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-3))
    };
    context.Users.Add(user);
    await context.SaveChangesAsync();
    
    // Act
    var result = await streakService.UpdateStreakAsync(userId);
    
    // Assert
    Assert.Equal(1, result.CurrentStreak);
    Assert.Equal(10, result.LongestStreak); // Longest không đổi
    Assert.True(result.StreakBroken);
}
```

---

## Troubleshooting

### Issue 1: Streak không update
**Check:**
1. Logs có thông báo streak update không?
2. Database có columns mới chưa?
3. StreakService có được register trong DI không?

**Solution:**
```bash
# Check migration
dotnet ef migrations list

# Check database
psql -d your_database -c "\d users"

# Check DI registration
# Xem Program.cs có dòng: builder.Services.AddScoped<IStreakService, StreakService>();
```

### Issue 2: Leaderboard chậm
**Check:**
1. Index có được tạo chưa?
2. Query có dùng đúng column không?

**Solution:**
```sql
-- Check index
SELECT indexname, indexdef 
FROM pg_indexes 
WHERE tablename = 'users';

-- Should see: idx_users_current_streak

-- Test query performance
EXPLAIN ANALYZE
SELECT * FROM users
WHERE is_active = true AND current_streak > 0
ORDER BY current_streak DESC
LIMIT 10;
```

### Issue 3: Streak bị reset sai
**Check:**
1. Timezone có đúng không?
2. LastPracticeDate có được set đúng không?

**Solution:**
```csharp
// Đảm bảo dùng UTC
var today = DateOnly.FromDateTime(DateTime.UtcNow);
```

---

## Checklist

- [ ] Migration applied thành công
- [ ] Database có 4 columns mới
- [ ] Index được tạo
- [ ] Login response có streak fields
- [ ] Create session tự động update streak
- [ ] Statistics API trả về streak
- [ ] Leaderboard nhanh (< 50ms)
- [ ] Logs hiển thị streak updates
- [ ] Streak logic hoạt động đúng:
  - [ ] First practice → streak = 1
  - [ ] Same day → streak không đổi
  - [ ] Consecutive day → streak +1
  - [ ] Skip day → streak reset
  - [ ] New record → longest streak update

---

## 🎯 Success Criteria

✅ All API endpoints trả về streak data  
✅ Streak tự động update khi practice  
✅ Leaderboard query < 50ms  
✅ Logs hiển thị streak changes  
✅ No errors in application logs  

---

**Happy Testing!** 🚀











