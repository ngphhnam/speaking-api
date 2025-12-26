# 🚀 QUICK CREATE 100 TOPICS - COPY & PASTE READY

## 📊 FILES ĐÃ TẠO

1. ✅ **100_TOPICS_FULL_LIST.md** - Danh sách đầy đủ 100 topics
2. ✅ **100_IELTS_TOPICS_COMPLETE.json** - 6 topics mẫu với full questions
3. ✅ **QUICK_CREATE_100_TOPICS.md** - File này

---

## 🎯 TẠO NHANH - 3 BƯỚC

### **BƯỚC 1: Chọn Category**

Xem `100_TOPICS_FULL_LIST.md` và chọn topic bạn muốn tạo:
- Part 1: Topics 1-30
- Part 2: Topics 31-100

### **BƯỚC 2: Copy Template**

**For Part 1:**
```http
POST {{baseUrl}}/api/topics
Content-Type: application/json

{
  "title": "[TOPIC NAME]",
  "slug": "[topic-slug]",
  "description": "[Description]",
  "partNumber": 1,
  "difficultyLevel": "beginner",
  "topicCategory": "personal",
  "keywords": ["keyword1", "keyword2"]
}
```

**For Part 2:**
```http
POST {{baseUrl}}/api/topics
Content-Type: application/json

{
  "title": "Describe [TOPIC]",
  "slug": "describe-[topic]",
  "description": "Describe a [topic]",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "people",
  "keywords": ["keyword1", "keyword2"]
}
```

### **BƯỚC 3: Add Questions**

Xem `100_IELTS_TOPICS_COMPLETE.json` để copy questions template.

---

## 📋 PART 1 - COPY-PASTE TOPICS (30 topics)

### **1-6. Personal Topics (Copy mỗi cái này):**

```json
{"title": "Work", "slug": "work", "partNumber": 1, "topicCategory": "personal"}
{"title": "Study", "slug": "study", "partNumber": 1, "topicCategory": "education"}
{"title": "Hometown", "slug": "hometown", "partNumber": 1, "topicCategory": "personal"}
{"title": "Accommodation", "slug": "accommodation", "partNumber": 1, "topicCategory": "personal"}
{"title": "Family", "slug": "family", "partNumber": 1, "topicCategory": "personal"}
{"title": "Friends", "slug": "friends", "partNumber": 1, "topicCategory": "personal"}
```

### **7-16. Daily Life Topics:**

```json
{"title": "Daily Routine", "slug": "daily-routine", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Food & Cooking", "slug": "food-cooking", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Shopping", "slug": "shopping", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Transport", "slug": "transport", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Weather", "slug": "weather", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Clothes", "slug": "clothes", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Sleep", "slug": "sleep", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Weekends", "slug": "weekends", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Housework", "slug": "housework", "partNumber": 1, "topicCategory": "daily-life"}
{"title": "Time Management", "slug": "time-management", "partNumber": 1, "topicCategory": "daily-life"}
```

### **17-30. Leisure Topics:**

```json
{"title": "Hobbies", "slug": "hobbies", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Sports", "slug": "sports", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Music", "slug": "music", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Movies & TV", "slug": "movies-tv", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Reading", "slug": "reading", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Travel", "slug": "travel", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Art", "slug": "art", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Photography", "slug": "photography", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Gardens", "slug": "gardens", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Pets", "slug": "pets", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Technology", "slug": "technology", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Internet", "slug": "internet", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Social Media", "slug": "social-media", "partNumber": 1, "topicCategory": "lifestyle"}
{"title": "Mobile Phones", "slug": "mobile-phones", "partNumber": 1, "topicCategory": "lifestyle"}
```

---

## 📋 PART 2 - COPY-PASTE TOPICS (70 topics)

### **31-42. PEOPLE (12 topics):**

```json
{"title": "Describe a Person You Admire", "slug": "describe-person-admire", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Family Member", "slug": "describe-family-member", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Good Friend", "slug": "describe-friend", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Teacher", "slug": "describe-teacher", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe Someone Who Helped You", "slug": "describe-someone-helped", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Famous Person", "slug": "describe-famous-person", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Person with Interesting Job", "slug": "describe-person-job", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe an Old Person", "slug": "describe-old-person", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Child You Know", "slug": "describe-child", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe Someone You Work/Study With", "slug": "describe-colleague", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe a Neighbor", "slug": "describe-neighbor", "partNumber": 2, "topicCategory": "people"}
{"title": "Describe Someone Who Made You Laugh", "slug": "describe-funny-person", "partNumber": 2, "topicCategory": "people"}
```

### **43-54. PLACES (12 topics):**

```json
{"title": "Describe a Place You Visited", "slug": "describe-place-visited", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe Your Hometown", "slug": "describe-hometown", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe Your Favorite Place", "slug": "describe-favorite-place", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Historic Place", "slug": "describe-historic-place", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Natural Place", "slug": "describe-natural-place", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a City You Want to Visit", "slug": "describe-city-visit", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Quiet Place", "slug": "describe-quiet-place", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Place to Relax", "slug": "describe-relaxing-place", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Shopping Center", "slug": "describe-shopping-center", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Library", "slug": "describe-library", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Restaurant", "slug": "describe-restaurant", "partNumber": 2, "topicCategory": "places"}
{"title": "Describe a Café", "slug": "describe-cafe", "partNumber": 2, "topicCategory": "places"}
```

### **55-64. OBJECTS (10 topics):**

```json
{"title": "Describe Something You Own", "slug": "describe-something-own", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe a Gift", "slug": "describe-gift", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe a Photo", "slug": "describe-photo", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe a Book", "slug": "describe-book", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe Technology", "slug": "describe-technology", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe Clothing", "slug": "describe-clothing", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe Furniture", "slug": "describe-furniture", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe a Vehicle", "slug": "describe-vehicle", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe a Toy", "slug": "describe-toy", "partNumber": 2, "topicCategory": "objects"}
{"title": "Describe Something You Made", "slug": "describe-made-yourself", "partNumber": 2, "topicCategory": "objects"}
```

### **65-79. EVENTS (15 topics):**

```json
{"title": "Describe a Memorable Event", "slug": "describe-memorable-event", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Special Occasion", "slug": "describe-special-occasion", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe Helping Someone", "slug": "describe-helping-someone", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Difficult Decision", "slug": "describe-difficult-decision", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe an Achievement", "slug": "describe-achievement", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe Being Late", "slug": "describe-being-late", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Mistake", "slug": "describe-mistake", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe Getting Lost", "slug": "describe-getting-lost", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe Childhood Memory", "slug": "describe-childhood-memory", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Wedding", "slug": "describe-wedding", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Party", "slug": "describe-party", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe Good News", "slug": "describe-good-news", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Surprise", "slug": "describe-surprise", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Conversation", "slug": "describe-conversation", "partNumber": 2, "topicCategory": "events"}
{"title": "Describe a Journey", "slug": "describe-journey", "partNumber": 2, "topicCategory": "events"}
```

### **80-91. ACTIVITIES (12 topics):**

```json
{"title": "Describe a Hobby", "slug": "describe-hobby", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe a Sport", "slug": "describe-sport", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe a Skill You Learned", "slug": "describe-skill-learned", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Exercise", "slug": "describe-exercise", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe a Course", "slug": "describe-course", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe a Project", "slug": "describe-project", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Outdoor Activity", "slug": "describe-outdoor-activity", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe a Game", "slug": "describe-game", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Environmental Activity", "slug": "describe-environment-activity", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Musical Instrument", "slug": "describe-instrument", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Language Learning", "slug": "describe-language", "partNumber": 2, "topicCategory": "activities"}
{"title": "Describe Creative Activity", "slug": "describe-creative", "partNumber": 2, "topicCategory": "activities"}
```

### **92-100. MEDIA (9 topics):**

```json
{"title": "Describe a Movie", "slug": "describe-movie", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe a TV Program", "slug": "describe-tv-program", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe a Website", "slug": "describe-website", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe a Mobile App", "slug": "describe-app", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe an Advertisement", "slug": "describe-advertisement", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe a Song", "slug": "describe-song", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe News", "slug": "describe-news", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe Social Media", "slug": "describe-social-media", "partNumber": 2, "topicCategory": "media"}
{"title": "Describe a Video Game", "slug": "describe-video-game", "partNumber": 2, "topicCategory": "media"}
```

---

## 🎯 AUTOMATION SCRIPT

Nếu bạn muốn tạo hết 100 topics tự động, tôi có thể tạo:

1. **Bulk Import JSON** - 1 file JSON lớn với tất cả 100 topics
2. **Seed Data Script** - C# seeder class
3. **SQL Script** - Direct SQL inserts
4. **Python/Node Script** - Script gọi API tự động

**Bạn muốn option nào?**

---

## 📊 PROGRESS TRACKER

Track tiến độ tạo topics của bạn:

```
Part 1 Topics (30):
□□□□□□□□□□ (0/30)

Part 2 Topics (70):
People:    □□□□□□□□□□□□ (0/12)
Places:    □□□□□□□□□□□□ (0/12)
Objects:   □□□□□□□□□□ (0/10)
Events:    □□□□□□□□□□□□□□□ (0/15)
Activities:□□□□□□□□□□□□ (0/12)
Media:     □□□□□□□□□ (0/9)
```

---

## 💡 PRO TIPS

1. **Start with Part 1** - Dễ hơn, questions ngắn
2. **Use templates** - Copy từ file mẫu
3. **Batch create** - Tạo theo category (all People, all Places, etc.)
4. **Test early** - Tạo 10 topics rồi test mock test
5. **Expand gradually** - Không cần tạo hết 100 cùng lúc

---

**Bạn cần tôi tạo thêm gì không?**
- Bulk import script?
- More detailed questions cho từng topic?
- Seed data file?









