# 🌱 SEED DATA GUIDE - IMPORT IELTS TOPICS

## ✅ ĐÃ TẠO

File: `DataSeed/IELTSTopicsSeeder.cs`

**Chứa:**
- 5 Part 1 topics với 10-12 questions mỗi cái
- 5 Part 2 topics với cue cards + Part 3 questions
- **Total: 10 topics, ~100 questions**

---

## 🚀 CÁCH SỬ DỤNG

### **Option 1: Run với --seed argument** ⭐ (Recommended)

```bash
dotnet run --seed
```

**Hoặc:**

```bash
cd G:\FINAL_PROJECT\SPEAKING\SpeakingPractice.Api
dotnet run --seed
```

**Kết quả:**
```
Seeding IELTS Speaking topics...
✅ Seeded 10 topics and 100 questions successfully!
```

---

### **Option 2: Gọi từ code**

Trong `Program.cs` đã được update:

```csharp
if (args.Contains("--seed"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await IELTSTopicsSeeder.SeedAsync(context);
}
```

---

### **Option 3: Gọi manual từ controller/endpoint**

Tạo admin endpoint:

```csharp
[HttpPost("seed-topics")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> SeedTopics()
{
    await IELTSTopicsSeeder.SeedAsync(context);
    return Ok("Topics seeded successfully");
}
```

---

## 📊 DATA ĐƯỢC SEED

### **Part 1 Topics (5):**
1. **Work** - 12 questions
2. **Study** - 10 questions
3. **Hometown** - 10 questions
4. **Family** - 10 questions
5. **Hobbies** - 10 questions

### **Part 2 & 3 Topics (5):**
1. **Describe a Person You Admire** - 1 cue card + 5 Part 3 questions
2. **Describe a Place You Visited** - 1 cue card + 5 Part 3 questions
3. **Describe Something You Own** - 1 cue card + 4 Part 3 questions
4. **Describe a Memorable Event** - 1 cue card + 4 Part 3 questions
5. **Describe a Hobby** - 1 cue card + 4 Part 3 questions

**Total: 10 topics, ~100 questions**

---

## ⚙️ FEATURES

### **1. Idempotent (An toàn)**
```csharp
if (await context.Topics.AnyAsync())
{
    Console.WriteLine("Topics already exist. Skipping seed.");
    return;
}
```
- Chỉ seed nếu database chưa có topics
- Chạy nhiều lần không bị duplicate

### **2. Proper GUIDs**
- Mỗi topic và question có unique GUID
- Foreign keys được link đúng

### **3. Timestamps**
- CreatedAt và UpdatedAt được set đúng

### **4. Active by default**
- Tất cả topics và questions có `IsActive = true`

---

## 🔧 CUSTOMIZE

### **Thêm nhiều topics hơn:**

Edit file `DataSeed/IELTSTopicsSeeder.cs`:

```csharp
// Add more Part 1 topics
var musicTopic = CreatePart1Topic("Music", "music", "Questions about music", "lifestyle", now);
topics.Add(musicTopic);
questions.AddRange(new[]
{
    CreateQuestion(musicTopic.Id, "Do you like music?", "Part 1", 30, now),
    CreateQuestion(musicTopic.Id, "What kind of music do you like?", "Part 1", 35, now),
    // ... more questions
});

// Add more Part 2 topics
var movieTopic = CreatePart2Topic("Describe a Movie", "describe-movie", "Describe a movie you enjoyed", "media", now);
topics.Add(movieTopic);
questions.Add(CreateCueCard(movieTopic.Id, "Describe a movie...\n\n• what...", now));
questions.AddRange(new[]
{
    CreateQuestion(movieTopic.Id, "Why do people like movies?", "Part 3", 45, now),
    // ... more Part 3 questions
});
```

---

## 🧪 VERIFY

### **Check database sau khi seed:**

```sql
-- Check topics
SELECT COUNT(*) FROM topics;
-- Expected: 10

SELECT part_number, COUNT(*) 
FROM topics 
GROUP BY part_number;
-- Expected: Part 1: 5, Part 2: 5

-- Check questions
SELECT COUNT(*) FROM questions;
-- Expected: ~100

SELECT question_type, COUNT(*) 
FROM questions 
GROUP BY question_type;
-- Expected: Part 1: ~52, Part 2: 5, Part 3: ~22
```

### **Test Mock Test:**

```http
POST {{baseUrl}}/api/mock-tests/start
{
  "part1QuestionCount": 12,
  "part2QuestionCount": 1,
  "part3QuestionCount": 4
}
```

**Expected:** ✅ Success với random questions từ seeded data

---

## 🔄 RE-SEED

Nếu muốn seed lại:

### **Option 1: Drop và recreate database**
```bash
dotnet ef database drop
dotnet ef database update
dotnet run --seed
```

### **Option 2: Delete topics manually**
```sql
DELETE FROM questions;
DELETE FROM topics;
```

Sau đó:
```bash
dotnet run --seed
```

---

## 📝 STRUCTURE

```
DataSeed/
├── SampleDataSeeder.cs          # Existing seeder (achievements, etc.)
└── IELTSTopicsSeeder.cs         # New IELTS topics seeder
```

**Program.cs:**
```csharp
if (args.Contains("--seed"))
{
    await IELTSTopicsSeeder.SeedAsync(context);
}
```

---

## 💡 BEST PRACTICES

### **Development:**
```bash
# Seed once during development
dotnet run --seed
```

### **Production:**
```bash
# Run migration first
dotnet ef database update

# Then seed
dotnet run --seed
```

### **Docker:**
```dockerfile
# In Dockerfile or docker-compose
CMD ["dotnet", "run", "--seed"]
```

---

## 🚨 TROUBLESHOOTING

### **Error: "Topics already exist"**
✅ This is normal! Seeder is idempotent.

**Solution:** Delete existing topics if you want to re-seed.

### **Error: "Foreign key constraint"**
❌ Topics must be added before questions.

**Solution:** Check that topics are added to context before questions.

### **Error: "Duplicate key"**
❌ GUID collision (very rare).

**Solution:** Restart seed process.

---

## 📊 EXPAND TO 100 TOPICS

Hiện tại: **10 topics**

Để expand lên **100 topics**, thêm vào `IELTSTopicsSeeder.cs`:

### **Part 1 (thêm 25 topics):**
- Music, Movies, Sports, Reading, Travel
- Food, Shopping, Transport, Weather, Clothes
- Technology, Internet, Social Media, Phones
- Sleep, Weekends, Housework, Time Management
- Art, Photography, Gardens, Pets, etc.

### **Part 2 (thêm 65 topics):**
- **People** (7 more): Teacher, Friend, Family member, Famous person, etc.
- **Places** (12): Historic place, Natural place, City, Quiet place, etc.
- **Objects** (10): Gift, Photo, Book, Technology, Clothing, etc.
- **Events** (15): Achievement, Wedding, Party, Decision, Mistake, etc.
- **Activities** (12): Sport, Skill, Course, Project, Game, etc.
- **Media** (9): Movie, TV, Website, App, Advertisement, etc.

---

## 🎯 QUICK COMMANDS

```bash
# Seed data
dotnet run --seed

# Check if seeded
dotnet ef dbcontext info

# View topics
# Use SQL client or API: GET /api/topics

# Test mock test
# POST /api/mock-tests/start
```

---

## ✅ CHECKLIST

- [x] Created `IELTSTopicsSeeder.cs`
- [x] Updated `Program.cs` with --seed argument
- [x] 10 topics with ~100 questions
- [x] Idempotent seeding
- [x] Proper GUIDs and timestamps
- [ ] Run `dotnet run --seed`
- [ ] Verify in database
- [ ] Test mock test API

---

**🎉 Ready to seed! Run `dotnet run --seed`**

