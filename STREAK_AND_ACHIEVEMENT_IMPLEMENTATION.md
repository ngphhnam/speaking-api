# 🎯 Tóm tắt Implementation: Streak & Achievement System

## ✅ Đã hoàn thành

### 1. **Streak System - Cải tiến**

#### Thay đổi chính:
- ✅ **Streak chỉ tăng khi hoàn thành câu hỏi** (không phải khi tạo session)
- ✅ **Cho phép khôi phục streak** nếu bỏ lỡ 1 ngày (daysDifference == 2)
- ✅ **Lưu lịch sử streak** vào bảng `streak_history` để xem lại các streak cũ

#### Files đã tạo/sửa:
- `Domain/Entities/StreakHistory.cs` - Entity lưu lịch sử streak
- `Configurations/StreakHistoryConfiguration.cs` - Cấu hình entity
- `Repositories/IStreakHistoryRepository.cs` - Interface repository
- `Repositories/StreakHistoryRepository.cs` - Implementation repository
- `Services/StreakService.cs` - Cập nhật logic khôi phục streak và lưu lịch sử
- `Services/Interfaces/IStreakService.cs` - Thêm method `GetStreakHistoryAsync`
- `Controllers/StreakController.cs` - API endpoints mới
- `Controllers/AnswersController.cs` - Chuyển logic update streak vào đây
- `Services/SpeakingSessionService.cs` - Xóa logic update streak (chỉ tạo session)

#### Logic khôi phục streak:
```csharp
// Nếu bỏ lỡ 1 ngày (daysDifference == 2), streak vẫn được tiếp tục
// Nếu bỏ lỡ > 1 ngày, streak bị reset về 1
```

---

### 2. **Level System**

#### Thêm vào ApplicationUser:
- `Level` (int, default: 1) - Level hiện tại
- `ExperiencePoints` (int, default: 0) - Điểm kinh nghiệm
- `TotalPoints` (int, default: 0) - Tổng điểm từ achievements

#### Công thức tính level:
```
XP cần để lên level = 100 × level^1.5
```

#### Files đã sửa:
- `Domain/Entities/ApplicationUser.cs` - Thêm 3 fields mới
- `Configurations/ApplicationUserConfiguration.cs` - Cấu hình fields

---

### 3. **Achievement System**

#### Service mới:
- `Services/AchievementService.cs` - Service kiểm tra và trao achievements
- `Services/Interfaces/IAchievementService.cs` - Interface

#### Chức năng:
- ✅ Tự động kiểm tra achievements khi user hoàn thành câu hỏi
- ✅ Trao achievements dựa trên:
  - **Practice Streak**: Số ngày luyện tập liên tiếp
  - **Score Milestone**: Điểm số đạt được
  - **Total Practice Days**: Tổng số ngày đã luyện tập
  - **Total Questions**: Tổng số câu hỏi đã hoàn thành
- ✅ Tự động cập nhật level khi nhận achievement
- ✅ Tính XP và level up khi đạt đủ điểm

#### Files đã sửa:
- `Controllers/AnswersController.cs` - Thêm logic check achievements sau khi submit answer
- `Program.cs` - Đăng ký `IAchievementService`

---

### 4. **API Endpoints mới**

#### StreakController (`/api/streak`):
- `GET /api/streak/info` - Lấy thông tin streak hiện tại
- `GET /api/streak/history` - Lấy lịch sử streak
- `GET /api/streak/level` - Lấy thông tin level và XP

---

## 📊 Database Changes

### Bảng mới: `streak_history`
```sql
CREATE TABLE streak_history (
    id UUID PRIMARY KEY,
    user_id UUID NOT NULL REFERENCES users(id) ON DELETE CASCADE,
    streak_length INTEGER NOT NULL,
    start_date DATE NOT NULL,
    end_date DATE,
    is_active BOOLEAN DEFAULT false,
    created_at TIMESTAMPTZ NOT NULL,
    updated_at TIMESTAMPTZ NOT NULL
);

CREATE INDEX idx_streak_history_user_id ON streak_history(user_id);
CREATE INDEX idx_streak_history_user_active ON streak_history(user_id, is_active);
CREATE INDEX idx_streak_history_start_date ON streak_history(start_date);
```

### Bảng `users` - Thêm columns:
```sql
ALTER TABLE users ADD COLUMN level INTEGER DEFAULT 1;
ALTER TABLE users ADD COLUMN experience_points INTEGER DEFAULT 0;
ALTER TABLE users ADD COLUMN total_points INTEGER DEFAULT 0;
```

---

## 🔄 Flow hoạt động

### Khi user hoàn thành câu hỏi:

1. **Submit Answer** (`AnswersController.SubmitAnswer`)
   - Xử lý audio, transcription, scoring
   - Lưu recording và analysis result

2. **Update Streak** (`StreakService.UpdateStreakAsync`)
   - Kiểm tra ngày luyện tập cuối
   - Tăng streak nếu liên tiếp
   - Khôi phục streak nếu bỏ lỡ 1 ngày
   - Lưu vào `streak_history`

3. **Check Achievements** (`AchievementService.CheckAndAwardAchievementsAsync`)
   - Kiểm tra tất cả achievements active
   - Trao achievements nếu đủ điều kiện
   - Cập nhật XP và level

4. **Return Response**
   - Trả về kết quả scoring
   - (Có thể thêm thông tin achievements earned)

---

## 📝 Migration cần tạo

Chạy lệnh sau để tạo migration:

```bash
dotnet ef migrations add AddStreakHistoryAndLevelSystem
dotnet ef database update
```

---

## 🎮 Gợi ý Achievements

Xem file `ACHIEVEMENT_SUGGESTIONS.md` để biết:
- Danh sách achievements gợi ý
- Cách thêm achievements vào database
- Badge icons suggestions
- Special achievements

---

## 🚀 Next Steps

1. **Tạo Migration**: Chạy migration để tạo bảng `streak_history` và thêm columns vào `users`
2. **Seed Achievements**: Thêm các achievements gợi ý vào database
3. **Test Flow**: Test toàn bộ flow từ submit answer → update streak → check achievements
4. **UI Integration**: Frontend cần hiển thị:
   - Streak info và history
   - Level và XP progress
   - Achievements earned
   - Badge icons
5. **Notifications**: Thêm notification khi user đạt achievement hoặc level up

---

## 📚 API Examples

### Get Streak Info:
```http
GET /api/streak/info
Authorization: Bearer {token}

Response:
{
  "success": true,
  "data": {
    "userId": "...",
    "currentStreak": 5,
    "longestStreak": 10,
    "lastPracticeDate": "2024-12-15",
    "totalPracticeDays": 25
  }
}
```

### Get Streak History:
```http
GET /api/streak/history
Authorization: Bearer {token}

Response:
{
  "success": true,
  "data": [
    {
      "id": "...",
      "streakLength": 10,
      "startDate": "2024-12-01",
      "endDate": "2024-12-10",
      "isActive": false,
      "createdAt": "2024-12-01T00:00:00Z"
    },
    {
      "id": "...",
      "streakLength": 5,
      "startDate": "2024-12-11",
      "endDate": null,
      "isActive": true,
      "createdAt": "2024-12-11T00:00:00Z"
    }
  ]
}
```

### Get User Level:
```http
GET /api/streak/level
Authorization: Bearer {token}

Response:
{
  "success": true,
  "data": {
    "level": 5,
    "experiencePoints": 350,
    "totalPoints": 500,
    "pointsToNextLevel": 150,
    "pointsForCurrentLevel": 250
  }
}
```

---

## ✅ Checklist hoàn thành

- [x] Tạo StreakHistory entity
- [x] Sửa StreakService với logic khôi phục
- [x] Chuyển streak update sang AnswersController
- [x] Thêm level system vào ApplicationUser
- [x] Tạo AchievementService
- [x] Tạo StreakController với API endpoints
- [x] Đăng ký services trong Program.cs
- [x] Tạo tài liệu gợi ý achievements
- [ ] Tạo migration (cần chạy lệnh)
- [ ] Seed achievements vào database
- [ ] Test toàn bộ flow

---

## 🐛 Notes

- Streak chỉ tăng khi **hoàn thành câu hỏi**, không phải khi tạo session
- Nếu bỏ lỡ **1 ngày**, streak vẫn được tiếp tục (khôi phục)
- Nếu bỏ lỡ **> 1 ngày**, streak bị reset về 1
- Achievements được check tự động sau mỗi lần hoàn thành câu hỏi
- Level được tính dựa trên XP từ achievements



