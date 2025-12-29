# 🚀 QUICK START - TẠO TOPICS & QUESTIONS

## 📋 TÓM TẮT NHANH

### **Bước 1: Tạo Part 1 Topic (Work)**
```http
POST {{baseUrl}}/api/topics
Content-Type: application/json

{
  "title": "Work",
  "slug": "work",
  "description": "Questions about your job, career, and workplace",
  "partNumber": 1,
  "difficultyLevel": "beginner",
  "topicCategory": "personal",
  "keywords": ["work", "job", "career", "occupation", "workplace"]
}
```

**→ Lưu lại `topicId` từ response!**

---

### **Bước 2: Thêm 12 Questions cho Part 1**

Dùng `topicId` vừa tạo:

```http
POST {{baseUrl}}/api/topics/{topicId}/questions
Content-Type: application/json

{
  "questionText": "What do you do? / What is your job?",
  "questionType": "Part 1",
  "timeLimitSeconds": 30,
  "sampleAnswers": [
    "I'm a software developer at a tech company.",
    "I'm currently a university student."
  ],
  "keyVocabulary": ["occupation", "profession", "career"],
  "estimatedBandRequirement": 5.0
}
```

**→ Lặp lại 11 lần với questions khác nhau!**

---

### **Bước 3: Tạo Part 2 Topic**

```http
POST {{baseUrl}}/api/topics
Content-Type: application/json

{
  "title": "Describe a Person You Admire",
  "slug": "describe-person-admire",
  "description": "Describe a person who has influenced you",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "people",
  "keywords": ["person", "admire", "influence", "role model"]
}
```

**→ Lưu lại `part2TopicId`!**

---

### **Bước 4: Thêm 1 Cue Card cho Part 2**

```http
POST {{baseUrl}}/api/topics/{part2TopicId}/questions
Content-Type: application/json

{
  "questionText": "Describe a person you admire.\n\nYou should say:\n• who this person is\n• how you know this person\n• what this person is like\n• and explain why you admire this person",
  "questionType": "Part 2",
  "timeLimitSeconds": 120,
  "sampleAnswers": [
    "I'd like to talk about my high school teacher, Ms. Nguyen. She taught me English for three years..."
  ],
  "keyVocabulary": ["admire", "role model", "inspiring", "dedicated"],
  "estimatedBandRequirement": 6.0
}
```

---

### **Bước 5: Thêm 4 Part 3 Questions (cùng topic với Part 2)**

```http
POST {{baseUrl}}/api/topics/{part2TopicId}/questions
Content-Type: application/json

{
  "questionText": "What qualities make someone a good role model?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45,
  "keyVocabulary": ["qualities", "role model", "integrity"],
  "estimatedBandRequirement": 6.5
}
```

**→ Lặp lại 3 lần với questions khác!**

---

### **Bước 6: Test Mock Test**

```http
POST {{baseUrl}}/api/mock-tests/start
Authorization: Bearer {{accessToken}}
Content-Type: application/json

{
  "part1QuestionCount": 12,
  "part2QuestionCount": 1,
  "part3QuestionCount": 4
}
```

**Response sẽ có:**
- 12 Part 1 questions (random từ database)
- 1 Part 2 cue card
- 4 Part 3 questions

---

## 📊 CHECKLIST ĐỂ MOCK TEST HOẠT ĐỘNG

### **Minimum Requirements:**

- [ ] Ít nhất **10 Part 1 questions** trong database
- [ ] Ít nhất **1 Part 2 cue card** trong database  
- [ ] Ít nhất **4 Part 3 questions** trong database

### **Recommended:**

- [ ] **100+ Part 1 questions** (nhiều topics)
- [ ] **20+ Part 2 cue cards** (diverse categories)
- [ ] **80+ Part 3 questions** (liên quan Part 2)

---

## 🎯 DANH SÁCH PART 1 TOPICS ĐỀ XUẤT

Tạo theo thứ tự này (dễ → khó):

1. **Hometown** (beginner)
2. **Work** (beginner)
3. **Study** (beginner)
4. **Family** (beginner)
5. **Hobbies** (beginner)
6. **Food** (beginner)
7. **Shopping** (beginner)
8. **Transport** (intermediate)
9. **Weather** (beginner)
10. **Clothes** (intermediate)
11. **Music** (beginner)
12. **Movies** (beginner)
13. **Sports** (beginner)
14. **Technology** (intermediate)
15. **Internet** (intermediate)

Mỗi topic cần **10-12 questions**.

---

## 🎯 DANH SÁCH PART 2 TOPICS ĐỀ XUẤT

### **People (5 topics):**
1. Describe a person you admire
2. Describe a family member
3. Describe a friend who is important to you
4. Describe someone who helped you
5. Describe a famous person you would like to meet

### **Places (5 topics):**
1. Describe a place you visited
2. Describe your favorite place in your hometown
3. Describe a historic place
4. Describe a natural place
5. Describe a city you want to visit

### **Objects (5 topics):**
1. Describe something you own that is important to you
2. Describe a gift you received
3. Describe a photo you like
4. Describe a book you enjoyed reading
5. Describe a piece of technology you use often

### **Events (5 topics):**
1. Describe a memorable event
2. Describe a special occasion you celebrated
3. Describe a time when you helped someone
4. Describe a difficult decision you made
5. Describe an achievement you are proud of

Mỗi Part 2 topic cần:
- **1 cue card** (QuestionType = "Part 2")
- **4-6 Part 3 questions** (QuestionType = "Part 3")

---

## 💡 TEMPLATES SẴN DÙNG

### **Part 1 Question Template:**

```json
{
  "questionText": "[Your question here]",
  "questionType": "Part 1",
  "timeLimitSeconds": 30,
  "sampleAnswers": [
    "[Answer 1]",
    "[Answer 2]"
  ],
  "keyVocabulary": ["word1", "word2", "word3"],
  "estimatedBandRequirement": 5.0
}
```

### **Part 2 Cue Card Template:**

```json
{
  "questionText": "Describe [topic].\n\nYou should say:\n• [point 1]\n• [point 2]\n• [point 3]\n• and explain [why/how]",
  "questionType": "Part 2",
  "timeLimitSeconds": 120,
  "sampleAnswers": [
    "[Long detailed answer, 150-200 words]"
  ],
  "keyVocabulary": ["word1", "word2", "word3", "word4", "word5"],
  "estimatedBandRequirement": 6.0
}
```

### **Part 3 Question Template:**

```json
{
  "questionText": "[Abstract/opinion question]",
  "questionType": "Part 3",
  "timeLimitSeconds": 45,
  "sampleAnswers": [
    "[Analytical answer with examples and reasons]"
  ],
  "keyVocabulary": ["academic", "vocabulary", "here"],
  "estimatedBandRequirement": 6.5
}
```

---

## 🔄 WORKFLOW AUTOMATION

### **Option 1: Manual (Qua Postman/API)**
1. Tạo từng topic
2. Thêm từng question
3. Test mock test

### **Option 2: Bulk Import (Khuyến nghị)**
1. Chuẩn bị JSON file với 100+ questions
2. Tạo endpoint bulk create
3. Import 1 lần

### **Option 3: Seed Data (Tốt nhất)**
1. Add vào `SampleDataSeeder.cs`
2. Run `dotnet run --seed`
3. Auto-populate database

---

## 📝 SAMPLE WORKFLOW

### **Ngày 1: Part 1**
```bash
# Morning
- Tạo 5 Part 1 topics
- Add 10 questions cho mỗi topic
- Total: 50 questions

# Afternoon
- Tạo thêm 5 Part 1 topics
- Add 10 questions cho mỗi topic
- Total: 100 questions Part 1 ✅
```

### **Ngày 2: Part 2 & 3**
```bash
# Morning
- Tạo 10 Part 2 topics
- Add 1 cue card cho mỗi topic
- Total: 10 cue cards

# Afternoon
- Add 5 Part 3 questions cho mỗi Part 2 topic
- Total: 50 Part 3 questions ✅
```

### **Ngày 3: Test & Polish**
```bash
# Test mock test
POST /api/mock-tests/start

# Verify
- Questions đủ và random tốt
- Sample answers có quality
- Vocabulary relevant

# Polish
- Fix typos
- Improve sample answers
- Add more keywords
```

---

## ⚠️ COMMON MISTAKES

### **❌ SAI:**
```json
{
  "questionText": "Do you like your job",  // ← Thiếu dấu ?
  "questionType": "part 1",                 // ← Phải viết hoa "Part 1"
  "timeLimitSeconds": "30"                  // ← Phải là number, không phải string
}
```

### **✅ ĐÚNG:**
```json
{
  "questionText": "Do you like your job?",
  "questionType": "Part 1",
  "timeLimitSeconds": 30
}
```

---

## 🎯 VERIFICATION

### **Check Part 1:**
```http
GET {{baseUrl}}/api/questions?questionType=Part 1
```
**Expected:** Ít nhất 100 questions

### **Check Part 2:**
```http
GET {{baseUrl}}/api/questions?questionType=Part 2
```
**Expected:** Ít nhất 10 cue cards

### **Check Part 3:**
```http
GET {{baseUrl}}/api/questions?questionType=Part 3
```
**Expected:** Ít nhất 40 questions

---

## 🚀 NEXT STEPS

1. ✅ Tạo ít nhất 10 Part 1 topics với 10 questions mỗi cái
2. ✅ Tạo ít nhất 5 Part 2 topics với cue cards
3. ✅ Thêm Part 3 questions cho mỗi Part 2 topic
4. ✅ Test mock test
5. ✅ Scale up lên 100+ topics

---

**Happy creating! 🎉**

**Files để tham khảo:**
- `IELTS_TOPICS_CREATION_GUIDE.md` - Full guide
- `IELTS_TOPICS_JSON_EXAMPLES.json` - JSON examples
- `SpeakingPractice_API.postman_collection.json` - Postman collection











