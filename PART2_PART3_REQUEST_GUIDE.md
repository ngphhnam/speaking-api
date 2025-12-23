# 📝 HƯỚNG DẪN BODY REQUEST - PART 2 & PART 3

## 🎯 WORKFLOW HOÀN CHỈNH

```
1. Tạo Topic (Part 2)
   ↓
2. Thêm 1 Cue Card (Part 2 Question)
   ↓
3. Thêm 4-6 Part 3 Questions (cùng topic)
```

---

## 📊 PART 2 - CUE CARD

### **Bước 1: Tạo Topic cho Part 2**

```http
POST {{baseUrl}}/api/topics
Content-Type: application/json
```

**Body:**
```json
{
  "title": "Describe a Person You Admire",
  "slug": "describe-person-admire",
  "description": "Describe a person who has influenced you or someone you admire",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "people",
  "keywords": ["person", "admire", "influence", "role model", "inspiration"]
}
```

**Response sẽ trả về:**
```json
{
  "success": true,
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",  // ← LƯU ID NÀY!
    "title": "Describe a Person You Admire",
    "partNumber": 2,
    ...
  }
}
```

---

### **Bước 2: Thêm Cue Card (Part 2 Question)**

**QUAN TRỌNG:** Dùng `topicId` vừa tạo ở Bước 1!

```http
POST {{baseUrl}}/api/topics/550e8400-e29b-41d4-a716-446655440000/questions
Content-Type: application/json
```

**Body:**
```json
{
  "questionText": "Describe a person you admire.\n\nYou should say:\n• who this person is\n• how you know this person\n• what this person is like\n• and explain why you admire this person",
  "questionType": "Part 2",
  "timeLimitSeconds": 120,
  "suggestedStructure": "{\"intro\":\"I'd like to talk about...\",\"point1\":\"First, regarding who this person is...\",\"point2\":\"I first met/knew this person when...\",\"point3\":\"As for what they're like...\",\"conclusion\":\"The reason I admire them is...\"}",
  "sampleAnswers": [
    "I'd like to talk about my high school English teacher, Ms. Nguyen. She taught me for three years and had a huge impact on my life. She's incredibly patient, passionate about teaching, and always goes the extra mile to help students. What I admire most is her dedication and the way she inspired me to pursue my dreams. She never gave up on any student and always believed in their potential. Thanks to her, I developed a love for English and decided to continue studying it at university."
  ],
  "keyVocabulary": [
    "admire",
    "role model",
    "inspiring",
    "influential",
    "dedicated",
    "passionate",
    "impact",
    "motivated",
    "look up to",
    "shaped my life"
  ],
  "estimatedBandRequirement": 6.0
}
```

---

## 📊 PART 3 - DISCUSSION QUESTIONS

### **Bước 3: Thêm Part 3 Questions**

**QUAN TRỌNG:** Dùng **CÙNG `topicId`** như Part 2 ở trên!

```http
POST {{baseUrl}}/api/topics/550e8400-e29b-41d4-a716-446655440000/questions
Content-Type: application/json
```

### **Question 1:**
```json
{
  "questionText": "What qualities make someone a good role model in your country?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45,
  "suggestedStructure": "{\"direct\":\"I think several qualities are important...\",\"expand\":\"For example...\",\"reason\":\"This is because...\",\"conclusion\":\"Overall...\"}",
  "sampleAnswers": [
    "I think several qualities make someone a good role model in Vietnam. Firstly, integrity is crucial - they should practice what they preach and be honest in their dealings. Secondly, they need to be hardworking and show dedication to their work or cause. Vietnamese people especially respect those who have overcome difficulties through perseverance and stayed humble despite their success. Additionally, being family-oriented and respecting traditional values is highly valued. Finally, contributing to society, whether through charity work or helping others, is seen as a mark of a true role model."
  ],
  "keyVocabulary": [
    "qualities",
    "role model",
    "integrity",
    "dedication",
    "perseverance",
    "humble",
    "contribute to society",
    "values"
  ],
  "estimatedBandRequirement": 6.5
}
```

### **Question 2:**
```json
{
  "questionText": "Do you think celebrities have a responsibility to be good role models?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45,
  "suggestedStructure": "{\"opinion\":\"Yes/No, I believe...\",\"reason\":\"This is because...\",\"example\":\"For instance...\",\"counterpoint\":\"However...\"}",
  "sampleAnswers": [
    "Yes, I believe celebrities do have a responsibility to be good role models, especially if they have young fans. They have chosen to be in the public eye and enjoy the benefits of fame, so they should accept the responsibilities that come with it. Their actions and words can significantly influence people, particularly young and impressionable fans. However, I also think we shouldn't put unrealistic expectations on them - they're human and will make mistakes. The key is how they handle their mistakes and whether they generally try to have a positive influence on their audience."
  ],
  "keyVocabulary": [
    "celebrities",
    "responsibility",
    "influence",
    "public eye",
    "role models",
    "impact",
    "impressionable",
    "positive influence"
  ],
  "estimatedBandRequirement": 7.0
}
```

### **Question 3:**
```json
{
  "questionText": "How has the concept of a role model changed over the years?",
  "questionType": "Part 3",
  "timeLimitSeconds": 50,
  "suggestedStructure": "{\"past\":\"In the past...\",\"present\":\"Nowadays...\",\"comparison\":\"The main difference is...\",\"reason\":\"This change is due to...\"}",
  "sampleAnswers": [
    "The concept of role models has changed quite dramatically over the years. In the past, role models were typically historical figures, political leaders, or local community members like teachers and doctors. Today, with the rise of social media and the entertainment industry, many young people look up to celebrities, influencers, and YouTube stars. The criteria have also shifted - it's now often about success, wealth, and fame rather than moral character or contribution to society. Additionally, role models have become more accessible through social media, but this also means their flaws and mistakes are more visible. I think this change reflects broader societal shifts in values and how we communicate."
  ],
  "keyVocabulary": [
    "dramatically",
    "evolved",
    "social media",
    "influencers",
    "criteria",
    "shifted",
    "accessible",
    "visible",
    "societal shifts"
  ],
  "estimatedBandRequirement": 7.5
}
```

### **Question 4:**
```json
{
  "questionText": "In what ways can parents influence their children to become better people?",
  "questionType": "Part 3",
  "timeLimitSeconds": 50,
  "suggestedStructure": "{\"main\":\"Parents can influence in several ways...\",\"method1\":\"Firstly...\",\"method2\":\"Secondly...\",\"method3\":\"Additionally...\",\"conclusion\":\"In conclusion...\"}",
  "sampleAnswers": [
    "Parents can influence their children in numerous ways. Most importantly, they should lead by example - children learn more from what parents do than what they say. Showing values like honesty, kindness, and hard work in daily life teaches children these principles naturally. Parents should also provide guidance while allowing children to make their own decisions and learn from mistakes. Creating a supportive environment where children feel safe to express themselves is crucial. Additionally, encouraging education, teaching respect for others, and exposing children to diverse experiences can broaden their perspectives. Finally, spending quality time together and having open conversations helps build strong relationships and trust."
  ],
  "keyVocabulary": [
    "influence",
    "lead by example",
    "values",
    "guidance",
    "supportive environment",
    "perspectives",
    "diverse experiences",
    "relationships"
  ],
  "estimatedBandRequirement": 7.0
}
```

---

## 🎨 TEMPLATES CHO CÁC LOẠI PART 2

### **1. PEOPLE (Người)**

```json
{
  "title": "Describe a Family Member",
  "slug": "describe-family-member",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "people"
}
```

**Cue Card:**
```json
{
  "questionText": "Describe a family member you are close to.\n\nYou should say:\n• who this person is\n• what your relationship is like\n• what you do together\n• and explain why you are close to this person",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}
```

**Part 3 Questions:**
- "How important is family in your country?"
- "Do you think family relationships have changed in recent years?"
- "What are the benefits of living in an extended family?"
- "How can parents balance work and family life?"

---

### **2. PLACES (Nơi chốn)**

```json
{
  "title": "Describe a Place You Visited",
  "slug": "describe-place-visited",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "places"
}
```

**Cue Card:**
```json
{
  "questionText": "Describe a place you visited that you found interesting.\n\nYou should say:\n• where it is\n• when you went there\n• what you did there\n• and explain why you found it interesting",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}
```

**Part 3 Questions:**
- "Why do people like to visit tourist attractions?"
- "How has tourism changed in your country?"
- "What are the advantages and disadvantages of tourism?"
- "Do you think virtual tourism will replace physical travel?"

---

### **3. OBJECTS (Đồ vật)**

```json
{
  "title": "Describe Something You Own",
  "slug": "describe-something-own",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "objects"
}
```

**Cue Card:**
```json
{
  "questionText": "Describe something you own that is important to you.\n\nYou should say:\n• what it is\n• when you got it\n• how you use it\n• and explain why it is important to you",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}
```

**Part 3 Questions:**
- "Do you think people buy too many things nowadays?"
- "How has shopping behavior changed with technology?"
- "Is it better to buy quality items or cheaper ones?"
- "What influence does advertising have on what people buy?"

---

### **4. EVENTS (Sự kiện)**

```json
{
  "title": "Describe a Memorable Event",
  "slug": "describe-memorable-event",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "events"
}
```

**Cue Card:**
```json
{
  "questionText": "Describe a memorable event in your life.\n\nYou should say:\n• when it happened\n• where it happened\n• what you did\n• and explain why it was memorable",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}
```

**Part 3 Questions:**
- "What types of events do people celebrate in your country?"
- "Do you think traditional celebrations are still important?"
- "How have celebrations changed over time?"
- "Why do people like to celebrate special occasions?"

---

### **5. ACTIVITIES (Hoạt động)**

```json
{
  "title": "Describe a Hobby",
  "slug": "describe-hobby",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "activities"
}
```

**Cue Card:**
```json
{
  "questionText": "Describe a hobby you enjoy.\n\nYou should say:\n• what the hobby is\n• when you started it\n• how often you do it\n• and explain why you enjoy it",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}
```

**Part 3 Questions:**
- "What are the benefits of having hobbies?"
- "Do you think children have enough time for hobbies today?"
- "How have people's hobbies changed with technology?"
- "Should schools encourage students to develop hobbies?"

---

## 📋 FIELD EXPLANATIONS

### **All Fields:**

| Field | Type | Required | Description | Example |
|-------|------|----------|-------------|---------|
| `questionText` | string | ✅ Yes | Câu hỏi đầy đủ | "Describe a person..." |
| `questionType` | string | ✅ Yes | Phải là "Part 2" hoặc "Part 3" | "Part 2" |
| `timeLimitSeconds` | number | ✅ Yes | Part 2: 120, Part 3: 45-50 | 120 |
| `suggestedStructure` | string (JSON) | ❌ No | Gợi ý cấu trúc trả lời | `"{\"intro\":\"...\"}` |
| `sampleAnswers` | array | ❌ No | Mẫu câu trả lời | ["I'd like to..."] |
| `keyVocabulary` | array | ❌ No | Từ vựng quan trọng | ["admire", "role model"] |
| `estimatedBandRequirement` | number | ❌ No | Band score yêu cầu | 6.5 |

---

## ⚠️ COMMON MISTAKES

### **❌ SAI:**

**1. Thiếu bullet points trong Part 2:**
```json
{
  "questionText": "Describe a person you admire",  // ← Thiếu structure!
  "questionType": "Part 2"
}
```

**2. QuestionType sai:**
```json
{
  "questionText": "...",
  "questionType": "part 2"  // ← Phải viết hoa "Part 2"
}
```

**3. Part 3 ở topic khác với Part 2:**
```json
// Part 2 topic: "Describe a person"
// Part 3 ở topic: "Describe a place"  ← SAI! Phải cùng topic
```

### **✅ ĐÚNG:**

**1. Part 2 với đầy đủ bullet points:**
```json
{
  "questionText": "Describe a person you admire.\n\nYou should say:\n• who this person is\n• how you know this person\n• what this person is like\n• and explain why you admire this person",
  "questionType": "Part 2"
}
```

**2. Part 3 cùng topic với Part 2:**
```json
// Cùng topicId với Part 2 về "person"
{
  "questionText": "What qualities make a good role model?",
  "questionType": "Part 3"
}
```

---

## 🎯 CHECKLIST

Trước khi submit request, check:

### **Part 2 Cue Card:**
- [ ] `questionText` có format: "Describe... \n\nYou should say:\n• point1\n• point2..."
- [ ] `questionType` = "Part 2" (viết hoa)
- [ ] `timeLimitSeconds` = 120
- [ ] `sampleAnswers` dài ít nhất 100 words
- [ ] `keyVocabulary` có 8-10 từ

### **Part 3 Questions:**
- [ ] Dùng **cùng topicId** với Part 2
- [ ] `questionType` = "Part 3" (viết hoa)
- [ ] `timeLimitSeconds` = 45-50
- [ ] Questions abstract, opinion-based
- [ ] Tối thiểu 4 questions

---

## 🔄 COMPLETE WORKFLOW EXAMPLE

### **Full workflow tạo 1 topic hoàn chỉnh:**

```bash
# Step 1: Create Topic
POST /api/topics
{
  "title": "Describe a Person You Admire",
  "partNumber": 2,
  ...
}
# → Get topicId: abc-123

# Step 2: Add Part 2 Cue Card
POST /api/topics/abc-123/questions
{
  "questionText": "Describe a person...\n\n• who...\n• how...",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}

# Step 3: Add Part 3 Question 1
POST /api/topics/abc-123/questions
{
  "questionText": "What qualities make a good role model?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45
}

# Step 4: Add Part 3 Question 2
POST /api/topics/abc-123/questions
{
  "questionText": "Do celebrities have responsibility?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45
}

# Step 5: Add Part 3 Question 3
POST /api/topics/abc-123/questions
{
  "questionText": "How has concept changed?",
  "questionType": "Part 3",
  "timeLimitSeconds": 50
}

# Step 6: Add Part 3 Question 4
POST /api/topics/abc-123/questions
{
  "questionText": "How can parents influence?",
  "questionType": "Part 3",
  "timeLimitSeconds": 50
}

# ✅ DONE! 1 complete topic với:
# - 1 Part 2 cue card
# - 4 Part 3 questions
```

---

## 📊 SUMMARY

| Step | Endpoint | Body Keys | Notes |
|------|----------|-----------|-------|
| 1 | `POST /api/topics` | title, partNumber=2 | Tạo topic Part 2 |
| 2 | `POST /api/topics/{id}/questions` | questionType="Part 2" | Add cue card |
| 3-6 | `POST /api/topics/{id}/questions` | questionType="Part 3" | Add 4 Part 3 questions |

**Key Points:**
- Part 3 questions **PHẢI** cùng topicId với Part 2
- Part 2: 1 cue card với 4 bullet points
- Part 3: 4-6 questions liên quan Part 2
- All questionType phải viết hoa: "Part 2", "Part 3"

---

**Happy creating! 🎉**





