# 🏆 Hướng dẫn Seed Achievements

## 📋 Tổng quan

File `AchievementsSeeder.cs` chứa **19 achievements** được chia thành 4 loại:

1. **Practice Streak** (5 achievements) - Luyện tập liên tiếp
2. **Score Milestone** (5 achievements) - Điểm số đạt được
3. **Total Practice Days** (4 achievements) - Tổng ngày luyện tập
4. **Total Questions** (5 achievements) - Tổng số câu hỏi hoàn thành

## 🚀 Cách sử dụng

### Tự động seed (đã cấu hình trong Program.cs):
Achievements sẽ tự động được seed khi ứng dụng khởi động (nếu chưa có data).

### Seed thủ công:
```csharp
using SpeakingPractice.Api.DataSeed;
using SpeakingPractice.Api.Infrastructure.Persistence;

// Trong Program.cs hoặc một endpoint admin
await AchievementsSeeder.SeedAsync(context);
```

## 📊 Danh sách Achievements

### 1. Practice Streak Achievements

| ID | Title | Points | Requirement |
|----|-------|--------|-------------|
| `11111111-...` | First Flame | 50 | 3 ngày liên tiếp |
| `22222222-...` | Week Warrior | 150 | 7 ngày liên tiếp |
| `33333333-...` | Fortnight Fighter | 300 | 14 ngày liên tiếp |
| `44444444-...` | Monthly Master | 500 | 30 ngày liên tiếp |
| `55555555-...` | Century Champion | 1000 | 100 ngày liên tiếp |

### 2. Score Milestone Achievements

| ID | Title | Points | Requirement |
|----|-------|--------|-------------|
| `66666666-...` | Getting Started | 30 | Đạt 5.0 |
| `77777777-...` | Good Progress | 50 | Đạt 6.0 |
| `88888888-...` | Great Job | 100 | Đạt 7.0 |
| `99999999-...` | Excellent | 200 | Đạt 8.0 |
| `aaaaaaaa-...` | Perfect Score | 500 | Đạt 9.0 |

### 3. Total Practice Days Achievements

| ID | Title | Points | Requirement |
|----|-------|--------|-------------|
| `bbbbbbbb-...` | Week Explorer | 50 | 7 ngày tổng cộng |
| `cccccccc-...` | Month Explorer | 150 | 30 ngày tổng cộng |
| `dddddddd-...` | Quarter Explorer | 300 | 90 ngày tổng cộng |
| `eeeeeeee-...` | Year Explorer | 1000 | 365 ngày tổng cộng |

### 4. Total Questions Achievements

| ID | Title | Points | Requirement |
|----|-------|--------|-------------|
| `ffffffff-...` | First Steps | 30 | 10 câu hỏi |
| `10101010-...` | Getting Serious | 100 | 50 câu hỏi |
| `20202020-...` | Dedicated Learner | 200 | 100 câu hỏi |
| `30303030-...` | Practice Master | 500 | 500 câu hỏi |
| `40404040-...` | Question King | 1000 | 1000 câu hỏi |

## ⚠️ Lưu ý

1. **Không trùng lặp**: Seeder sẽ kiểm tra nếu đã có achievements thì không seed lại
2. **Achievements cũ**: File `SampleDataSeeder.cs` có 5 achievements cũ, bạn có thể:
   - Xóa chúng đi (nếu không cần)
   - Hoặc để lại (sẽ không ảnh hưởng vì seeder mới dùng GUID khác)
3. **Badge Icons**: Các badge icons cần được upload vào thư mục `/badges/` trên server

## 🔄 Cập nhật Achievements

Nếu muốn thêm achievements mới:

1. Mở file `DataSeed/AchievementsSeeder.cs`
2. Thêm achievement mới vào list
3. Restart ứng dụng hoặc gọi seeder lại

## 🧪 Test

Sau khi seed, kiểm tra bằng cách:

```sql
-- Xem tất cả achievements
SELECT id, title, achievement_type, points, is_active 
FROM achievements 
ORDER BY achievement_type, points;

-- Đếm số achievements theo loại
SELECT achievement_type, COUNT(*) 
FROM achievements 
WHERE is_active = true 
GROUP BY achievement_type;
```

## 📝 RequirementCriteria Format

Mỗi achievement type có format khác nhau:

- **practice_streak**: `{"streak_days": 7}`
- **score_milestone**: `{"min_score": 7.0}`
- **total_practice_days**: `{"total_days": 30}`
- **total_questions**: `{"total_questions": 100}`

