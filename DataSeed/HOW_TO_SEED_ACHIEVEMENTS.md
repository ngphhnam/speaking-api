# 📝 Hướng dẫn Seed Achievements Data

## ✅ File đã tạo

File `AchievementsSeeder.cs` đã được tạo với **19 achievements** được chia thành 4 loại:

### 1. Practice Streak Achievements (5 achievements)
- 🔥 First Flame - 3 ngày liên tiếp (50 points)
- 🔥🔥 Week Warrior - 7 ngày liên tiếp (150 points)
- 🔥🔥🔥 Fortnight Fighter - 14 ngày liên tiếp (300 points)
- 🔥🔥🔥🔥 Monthly Master - 30 ngày liên tiếp (500 points)
- 🔥🔥🔥🔥🔥 Century Champion - 100 ngày liên tiếp (1000 points)

### 2. Score Milestone Achievements (5 achievements)
- ⭐ Getting Started - Đạt 5.0 (30 points)
- ⭐⭐ Good Progress - Đạt 6.0 (50 points)
- ⭐⭐⭐ Great Job - Đạt 7.0 (100 points)
- ⭐⭐⭐⭐ Excellent - Đạt 8.0 (200 points)
- ⭐⭐⭐⭐⭐ Perfect Score - Đạt 9.0 (500 points)

### 3. Total Practice Days Achievements (4 achievements)
- 📅 Week Explorer - 7 ngày tổng cộng (50 points)
- 📅📅 Month Explorer - 30 ngày tổng cộng (150 points)
- 📅📅📅 Quarter Explorer - 90 ngày tổng cộng (300 points)
- 📅📅📅📅 Year Explorer - 365 ngày tổng cộng (1000 points)

### 4. Total Questions Achievements (5 achievements)
- 💬 First Steps - 10 câu hỏi (30 points)
- 💬💬 Getting Serious - 50 câu hỏi (100 points)
- 💬💬💬 Dedicated Learner - 100 câu hỏi (200 points)
- 💬💬💬💬 Practice Master - 500 câu hỏi (500 points)
- 💬💬💬💬💬 Question King - 1000 câu hỏi (1000 points)

---

## 🚀 Cách sử dụng

### Option 1: Tự động seed khi chạy với `--seed` flag

File `Program.cs` đã được cập nhật để tự động seed achievements:

```bash
dotnet run --seed
```

Hoặc khi chạy ứng dụng:

```bash
dotnet run -- --seed
```

### Option 2: Seed thủ công trong code

Thêm vào `Program.cs` sau khi build app:

```csharp
var app = builder.Build();

// Seed achievements
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await SpeakingPractice.Api.DataSeed.AchievementsSeeder.SeedAsync(context);
}

app.Run();
```

### Option 3: Tạo endpoint để seed (cho development)

Thêm endpoint vào một controller (chỉ dùng trong development):

```csharp
[HttpPost("seed/achievements")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> SeedAchievements(CancellationToken ct)
{
    using var scope = HttpContext.RequestServices.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await AchievementsSeeder.SeedAsync(context);
    return Ok("Achievements seeded successfully");
}
```

---

## 🔍 Kiểm tra sau khi seed

### 1. Kiểm tra trong database:

```sql
SELECT 
    title, 
    achievement_type, 
    points, 
    requirement_criteria,
    is_active
FROM achievements
ORDER BY achievement_type, points;
```

### 2. Kiểm tra qua API:

```http
GET /api/achievements
```

Response sẽ trả về danh sách tất cả achievements.

---

## ⚠️ Lưu ý

1. **Seeder tự động skip nếu đã có data**: Seeder sẽ kiểm tra `if (await context.Achievements.AnyAsync())` và không seed lại nếu đã có achievements.

2. **Để seed lại**: Xóa tất cả achievements trong database trước:
   ```sql
   DELETE FROM user_achievements; -- Xóa user achievements trước
   DELETE FROM achievements; -- Sau đó xóa achievements
   ```

3. **GUIDs cố định**: Tất cả achievements có GUID cố định để dễ quản lý và reference.

4. **Badge Icons**: Các badge icon URLs là placeholder. Bạn cần upload các icon thực tế vào thư mục `/badges/` hoặc thay đổi URLs.

---

## 📊 Tổng kết

- **Tổng số achievements**: 19
- **Tổng điểm có thể đạt**: 5,880 points
- **4 loại achievements**: Practice Streak, Score Milestone, Total Practice Days, Total Questions
- **Tất cả đều active**: `IsActive = true`

---

## 🎯 Next Steps

1. ✅ Chạy seed để thêm achievements vào database
2. ⏳ Upload badge icons vào thư mục `/wwwroot/badges/` hoặc CDN
3. ⏳ Test achievement system bằng cách:
   - Hoàn thành câu hỏi
   - Đạt streak
   - Đạt điểm cao
4. ⏳ Kiểm tra achievements được trao tự động

