# 🏆 Gợi ý Achievements cho Project

## 📋 Tổng quan
Tài liệu này gợi ý các achievements có thể thêm vào project để tăng động lực cho người dùng.

---

## 🎯 Loại Achievements

### 1. **Practice Streak Achievements** (Luyện tập liên tục)

| Achievement | Mô tả | Điểm | RequirementCriteria |
|------------|-------|------|-------------------|
| 🔥 **First Flame** | Luyện tập 3 ngày liên tiếp | 50 | `{"streak_days": 3}` |
| 🔥🔥 **Week Warrior** | Luyện tập 7 ngày liên tiếp | 150 | `{"streak_days": 7}` |
| 🔥🔥🔥 **Fortnight Fighter** | Luyện tập 14 ngày liên tiếp | 300 | `{"streak_days": 14}` |
| 🔥🔥🔥🔥 **Monthly Master** | Luyện tập 30 ngày liên tiếp | 500 | `{"streak_days": 30}` |
| 🔥🔥🔥🔥🔥 **Century Champion** | Luyện tập 100 ngày liên tiếp | 1000 | `{"streak_days": 100}` |

### 2. **Score Milestone Achievements** (Điểm số)

| Achievement | Mô tả | Điểm | RequirementCriteria |
|------------|-------|------|-------------------|
| ⭐ **Getting Started** | Đạt điểm 5.0 | 30 | `{"min_score": 5.0}` |
| ⭐⭐ **Good Progress** | Đạt điểm 6.0 | 50 | `{"min_score": 6.0}` |
| ⭐⭐⭐ **Great Job** | Đạt điểm 7.0 | 100 | `{"min_score": 7.0}` |
| ⭐⭐⭐⭐ **Excellent** | Đạt điểm 8.0 | 200 | `{"min_score": 8.0}` |
| ⭐⭐⭐⭐⭐ **Perfect Score** | Đạt điểm 9.0 | 500 | `{"min_score": 9.0}` |

### 3. **Total Practice Days Achievements** (Tổng ngày luyện tập)

| Achievement | Mô tả | Điểm | RequirementCriteria |
|------------|-------|------|-------------------|
| 📅 **Week Explorer** | Luyện tập tổng cộng 7 ngày | 50 | `{"total_days": 7}` |
| 📅📅 **Month Explorer** | Luyện tập tổng cộng 30 ngày | 150 | `{"total_days": 30}` |
| 📅📅📅 **Quarter Explorer** | Luyện tập tổng cộng 90 ngày | 300 | `{"total_days": 90}` |
| 📅📅📅📅 **Year Explorer** | Luyện tập tổng cộng 365 ngày | 1000 | `{"total_days": 365}` |

### 4. **Total Questions Achievements** (Tổng số câu hỏi)

| Achievement | Mô tả | Điểm | RequirementCriteria |
|------------|-------|------|-------------------|
| 💬 **First Steps** | Hoàn thành 10 câu hỏi | 30 | `{"total_questions": 10}` |
| 💬💬 **Getting Serious** | Hoàn thành 50 câu hỏi | 100 | `{"total_questions": 50}` |
| 💬💬💬 **Dedicated Learner** | Hoàn thành 100 câu hỏi | 200 | `{"total_questions": 100}` |
| 💬💬💬💬 **Practice Master** | Hoàn thành 500 câu hỏi | 500 | `{"total_questions": 500}` |
| 💬💬💬💬💬 **Question King** | Hoàn thành 1000 câu hỏi | 1000 | `{"total_questions": 1000}` |

---

## 🎮 Level System

### Công thức tính Level:
```
XP cần để lên level = 100 × level^1.5
```

### Level Milestones:
- **Level 1-10**: Beginner (Mỗi level cần ~100-500 XP)
- **Level 11-25**: Intermediate (Mỗi level cần ~500-1500 XP)
- **Level 26-50**: Advanced (Mỗi level cần ~1500-3500 XP)
- **Level 51-75**: Expert (Mỗi level cần ~3500-6500 XP)
- **Level 76-100**: Master (Mỗi level cần ~6500-10000 XP)

---

## 📝 Cách thêm Achievements vào Database

### SQL Example:
```sql
-- Practice Streak Achievement
INSERT INTO achievements (id, title, description, achievement_type, requirement_criteria, points, badge_icon_url, is_active, created_at)
VALUES (
    gen_random_uuid(),
    'Week Warrior',
    'Luyện tập 7 ngày liên tiếp',
    'practice_streak',
    '{"streak_days": 7}',
    150,
    '/badges/week-warrior.png',
    true,
    NOW()
);

-- Score Milestone Achievement
INSERT INTO achievements (id, title, description, achievement_type, requirement_criteria, points, badge_icon_url, is_active, created_at)
VALUES (
    gen_random_uuid(),
    'Excellent',
    'Đạt điểm 8.0',
    'score_milestone',
    '{"min_score": 8.0}',
    200,
    '/badges/excellent.png',
    true,
    NOW()
);
```

---

## 🎨 Badge Icons Suggestions

- 🔥 Fire icons cho streak achievements
- ⭐ Star icons cho score achievements
- 📅 Calendar icons cho total days achievements
- 💬 Speech bubble icons cho total questions achievements
- 🏆 Trophy icons cho special achievements

---

## 💡 Special Achievements (Có thể thêm sau)

1. **Perfect Week**: Đạt điểm >= 7.0 trong 7 ngày liên tiếp
2. **Speed Demon**: Hoàn thành 10 câu hỏi trong 1 ngày
3. **Consistency King**: Luyện tập đúng giờ trong 7 ngày
4. **Comeback Kid**: Khôi phục streak sau khi bị mất
5. **Topic Master**: Hoàn thành tất cả câu hỏi trong 1 topic

---

## 🔄 Auto-Check System

Hệ thống tự động kiểm tra achievements khi:
- ✅ User hoàn thành một câu hỏi (trong `AnswersController`)
- ✅ Streak được update (trong `StreakService`)
- ✅ User đạt điểm cao (trong `AnswersController`)

---

## 📊 API Endpoints

### Get User Level:
```
GET /api/streak/level
```

### Get Streak Info:
```
GET /api/streak/info
```

### Get Streak History:
```
GET /api/streak/history
```

---

## 🚀 Next Steps

1. Tạo migration cho các thay đổi database
2. Seed data với các achievements gợi ý
3. Tạo badge icons cho achievements
4. Thêm notification khi user đạt achievement
5. Tạo leaderboard dựa trên level và achievements

