# 📚 HƯỚNG DẪN TẠO TOPICS & QUESTIONS CHO IELTS SPEAKING

## 🎯 Tổng quan cấu trúc

### **3 Entities chính:**
1. **Topic** - Chủ đề chung (Part 1, 2, 3)
2. **Question** - Câu hỏi thuộc topic (có QuestionType)
3. **MockTest** - Bài thi thử (tổng hợp Part 1, 2, 3)

---

## 📊 CẤU TRÚC IELTS SPEAKING

### **Part 1** - Introduction & Interview (4-5 phút)
- **Đặc điểm**: Câu hỏi ngắn, cá nhân, hàng ngày
- **Số câu**: 10-12 câu
- **Time/câu**: 20-30 giây
- **Topics**: Work, Study, Hometown, Hobbies, Family, etc.

### **Part 2** - Long Turn (3-4 phút)
- **Đặc điểm**: Cue card, nói liên tục 1-2 phút
- **Số câu**: 1 cue card
- **Prep time**: 1 phút
- **Speaking**: 2 phút
- **Topics**: Describe a person, place, event, object

### **Part 3** - Discussion (4-5 phút)
- **Đặc điểm**: Câu hỏi sâu, abstract, liên quan Part 2
- **Số câu**: 4-6 câu
- **Time/câu**: 30-60 giây
- **Topics**: Social issues, opinions, future predictions

---

## 🔧 1. TẠO TOPICS

### A. PART 1 TOPICS

#### **Cấu trúc Topic:**
```json
POST /api/topics
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

#### **Part 1 Topics phổ biến:**

**Personal Life:**
- Work
- Study/Education
- Hometown
- Accommodation (Home/Apartment)
- Family
- Friends

**Daily Life:**
- Daily Routine
- Food & Cooking
- Shopping
- Transport
- Weather
- Clothes & Fashion

**Leisure:**
- Hobbies
- Sports
- Music
- Movies & TV
- Reading
- Travel
- Holidays

**Technology:**
- Internet
- Social Media
- Mobile Phones
- Technology

---

### B. PART 2 TOPICS (CUE CARDS)

#### **Cấu trúc Topic:**
```json
POST /api/topics
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

#### **Part 2 Categories:**

**1. People (Người)**
- Describe a person you admire
- Describe a family member
- Describe a friend
- Describe someone who helped you
- Describe a famous person

**2. Places (Nơi chốn)**
- Describe a place you visited
- Describe your favorite place
- Describe a historic place
- Describe a city you want to visit
- Describe a natural place

**3. Objects (Đồ vật)**
- Describe something you own
- Describe a gift you received
- Describe a photo you like
- Describe a piece of technology
- Describe a book you read

**4. Events (Sự kiện)**
- Describe a special occasion
- Describe a memorable day
- Describe a time when you helped someone
- Describe a difficult decision
- Describe an achievement

**5. Activities (Hoạt động)**
- Describe a hobby you have
- Describe a skill you learned
- Describe a sport you like
- Describe something you do regularly

---

### C. PART 3 TOPICS (DISCUSSION)

Part 3 **KHÔNG CẦN** tạo separate topics!  
Part 3 questions **phụ thuộc** vào Part 2 topic.

**Cách hoạt động:**
- Part 2: "Describe a person you admire"
- Part 3: Questions về "role models in society", "influence of famous people", etc.

---

## 🔧 2. TẠO QUESTIONS

### A. PART 1 QUESTIONS

#### **Đặc điểm:**
- Ngắn gọn, trực tiếp
- Về bản thân, kinh nghiệm cá nhân
- Thì hiện tại đơn/hiện tại hoàn thành

#### **Ví dụ - Topic "Work":**

```json
POST /api/topics/{topicId}/questions
{
  "questionText": "What do you do? / What is your job?",
  "questionType": "Part 1",
  "timeLimitSeconds": 30,
  "suggestedStructure": "{ \"opening\": \"I'm a...\", \"detail\": \"I work at...\", \"extra\": \"I've been doing this for...\" }",
  "sampleAnswers": [
    "I'm a software developer. I work at a tech company in Ho Chi Minh City. I've been doing this for about 3 years now.",
    "I'm currently a university student studying Business Administration."
  ],
  "keyVocabulary": ["occupation", "profession", "career", "industry", "responsibilities"],
  "estimatedBandRequirement": 5.0
}
```

#### **Part 1 Question Templates:**

**About current situation:**
- What do you do? (work/study)
- Where do you live?
- What do you like about...?
- How often do you...?

**About past experience:**
- Did you like... when you were younger?
- Have you ever...?
- When did you start...?

**About future:**
- Would you like to...?
- Do you think you will... in the future?

**About preferences:**
- Do you prefer... or...?
- What's your favorite...?
- Which do you like better?

#### **Full example - 12 questions cho topic "Work":**

1. "What do you do? / What is your job?"
2. "What are your responsibilities at work?"
3. "Do you like your job? Why or why not?"
4. "What do you like most about your job?"
5. "Is there anything you don't like about your job?"
6. "What time do you usually start work?"
7. "How do you get to work?"
8. "Did you have to do a lot of training for your job?"
9. "What did you want to be when you were younger?"
10. "Do you plan to continue doing this job in the future?"
11. "What's the most important thing about a job for you?"
12. "Would you recommend your job to others?"

---

### B. PART 2 QUESTIONS (CUE CARDS)

#### **Đặc điểm:**
- 1 topic = 1 cue card
- Cue card có 4 bullet points
- Speaking time: 1-2 minutes

#### **Format chuẩn:**

```json
POST /api/topics/{topicId}/questions
{
  "questionText": "Describe a person you admire.\n\nYou should say:\n• who this person is\n• how you know this person\n• what this person is like\n• and explain why you admire this person",
  "questionType": "Part 2",
  "timeLimitSeconds": 120,
  "suggestedStructure": "{ \"intro\": \"I'd like to talk about...\", \"point1\": \"First, regarding who this person is...\", \"point2\": \"I first met/knew this person...\", \"point3\": \"As for what they're like...\", \"conclusion\": \"The reason I admire them is...\" }",
  "sampleAnswers": [
    "I'd like to talk about my high school teacher, Ms. Nguyen. She taught me English for 3 years and had a huge impact on my life. She's incredibly patient, passionate about teaching, and always goes the extra mile to help students. What I admire most is her dedication and the way she inspired me to pursue my dreams. She never gave up on any student and always believed in their potential."
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
    "look up to"
  ],
  "estimatedBandRequirement": 6.0
}
```

#### **Part 2 Cue Card Templates:**

**People:**
```
Describe [a person].
You should say:
• who this person is
• how you know this person
• what this person is/was like
• and explain [why/how/what makes them special]
```

**Places:**
```
Describe [a place].
You should say:
• where it is
• when you went there
• what you did there
• and explain why [you like it/it was special]
```

**Objects:**
```
Describe [an object].
You should say:
• what it is
• when you got it
• how you use it
• and explain why [it's important/special to you]
```

**Events:**
```
Describe [an event/experience].
You should say:
• when it happened
• where it happened
• what you did
• and explain [how you felt/why it was memorable]
```

---

### C. PART 3 QUESTIONS (DISCUSSION)

#### **Đặc điểm:**
- Liên quan đến Part 2
- Abstract, analytical, opinions
- So sánh, dự đoán, giải thích

#### **QUAN TRỌNG:** Part 3 questions **phụ thuộc** Part 2 topic

Nếu Part 2 là "Describe a person you admire", thì:

```json
POST /api/topics/{samePart2TopicId}/questions
[
  {
    "questionText": "What qualities make someone a good role model in your country?",
    "questionType": "Part 3",
    "timeLimitSeconds": 45,
    "suggestedStructure": "{ \"direct\": \"I think several qualities are important...\", \"expand\": \"For example...\", \"reason\": \"This is because...\" }",
    "sampleAnswers": [
      "I think several qualities make someone a good role model. Firstly, integrity is crucial - they should practice what they preach. Secondly, they need to be hardworking and show dedication. In my country, people especially respect those who have overcome difficulties and stayed humble despite their success."
    ],
    "keyVocabulary": ["qualities", "role model", "integrity", "dedication", "influence"],
    "estimatedBandRequirement": 6.5
  },
  {
    "questionText": "Do you think celebrities have a responsibility to be good role models?",
    "questionType": "Part 3",
    "timeLimitSeconds": 45,
    "keyVocabulary": ["celebrities", "responsibility", "influence", "public figure"],
    "estimatedBandRequirement": 7.0
  },
  {
    "questionText": "How has the concept of a role model changed over the years?",
    "questionType": "Part 3",
    "timeLimitSeconds": 50,
    "keyVocabulary": ["change", "evolve", "past vs present", "society"],
    "estimatedBandRequirement": 7.0
  },
  {
    "questionText": "In what ways can parents influence their children to become better people?",
    "questionType": "Part 3",
    "timeLimitSeconds": 50,
    "keyVocabulary": ["parents", "influence", "upbringing", "values"],
    "estimatedBandRequirement": 6.5
  }
]
```

#### **Part 3 Question Types:**

**1. General Questions (Mở rộng topic):**
- "What do most people think about...?"
- "Is it common in your country to...?"
- "What are the advantages/disadvantages of...?"

**2. Comparison Questions:**
- "How has... changed over the years?"
- "What's the difference between... and...?"
- "Do you think... is better than...?"

**3. Opinion Questions:**
- "Do you think...?"
- "Should... (do something)?"
- "Is it important to...?"

**4. Prediction Questions:**
- "How do you think... will change in the future?"
- "Will... become more/less...?"

**5. Cause/Effect Questions:**
- "Why do you think...?"
- "What are the reasons for...?"
- "What effects does... have on...?"

---

## 🔧 3. MOCK TEST

Mock Test **TỰ ĐỘNG** lấy questions từ database!

### **Cách hoạt động:**

```json
POST /api/mock-tests/start
{
  "part1QuestionCount": 12,  // Optional, default: 3
  "part2QuestionCount": 1,   // Optional, default: 1
  "part3QuestionCount": 4    // Optional, default: 4
}
```

**System sẽ:**
1. Random lấy questions từ database:
   - Part 1: 12 questions (QuestionType = "Part 1")
   - Part 2: 1 cue card (QuestionType = "Part 2")
   - Part 3: 4 questions (QuestionType = "Part 3")
2. Tạo MockTest entity
3. Trả về full test

**Response:**
```json
{
  "id": "mock-test-id",
  "status": "in_progress",
  "part1Questions": [/* 12 questions */],
  "part2Questions": [/* 1 cue card */],
  "part3Questions": [/* 4 questions */]
}
```

---

## 📝 WORKFLOW TẠO TOPICS & QUESTIONS

### **Bước 1: Tạo Topics**

#### **Part 1 - Tạo 20-30 topics:**
```bash
# Personal
POST /api/topics - "Work"
POST /api/topics - "Study"
POST /api/topics - "Hometown"
POST /api/topics - "Family"

# Daily Life
POST /api/topics - "Food"
POST /api/topics - "Shopping"
POST /api/topics - "Transport"

# Leisure
POST /api/topics - "Hobbies"
POST /api/topics - "Sports"
POST /api/topics - "Music"
```

#### **Part 2 - Tạo 50-100 cue cards:**
```bash
# People
POST /api/topics - "Describe a person you admire"
POST /api/topics - "Describe a family member"

# Places
POST /api/topics - "Describe a place you visited"
POST /api/topics - "Describe your favorite place"

# Objects
POST /api/topics - "Describe something you own"

# Events
POST /api/topics - "Describe a memorable day"
```

### **Bước 2: Thêm Questions vào Topics**

#### **For Part 1 topics:**
Mỗi topic cần **10-15 questions**

```bash
# Topic: Work
POST /api/topics/{workTopicId}/questions
  → Add 12 questions về work

# Topic: Hobbies
POST /api/topics/{hobbiesTopicId}/questions
  → Add 12 questions về hobbies
```

#### **For Part 2 topics:**
Mỗi topic = **1 cue card**

```bash
# Topic: Describe a person you admire
POST /api/topics/{topicId}/questions
  → Add 1 cue card (QuestionType = "Part 2")
  
# Sau đó thêm Part 3 questions liên quan
POST /api/topics/{topicId}/questions
  → Add 4-6 Part 3 questions (QuestionType = "Part 3")
```

---

## 🎯 SỐ LƯỢNG ĐỀ XUẤT

### **Minimum (Để hệ thống hoạt động):**
- Part 1: 10 topics × 10 questions = **100 questions**
- Part 2: 20 cue cards = **20 questions**
- Part 3: 20 topics × 4 questions = **80 questions**
- **Total: ~200 questions**

### **Recommended (Production-ready):**
- Part 1: 30 topics × 12 questions = **360 questions**
- Part 2: 100 cue cards = **100 questions**
- Part 3: 100 topics × 5 questions = **500 questions**
- **Total: ~960 questions**

---

## 📊 DATABASE SCHEMA

```
Topic
├── Id (Guid)
├── Title (string)
├── PartNumber (1, 2, 3)
├── DifficultyLevel (beginner, intermediate, advanced)
└── Questions (collection)
    ├── Question 1
    │   ├── QuestionText
    │   ├── QuestionType ("Part 1", "Part 2", "Part 3")
    │   ├── TimeLimitSeconds
    │   └── KeyVocabulary
    ├── Question 2
    └── ...

MockTest
├── Id
├── UserId
├── Part1QuestionIds (comma-separated)
├── Part2QuestionIds (comma-separated)
├── Part3QuestionIds (comma-separated)
└── Status (in_progress, completed)
```

---

## 💡 TIPS

### **Part 1:**
- Questions đơn giản, hàng ngày
- Trả lời ngắn gọn (2-3 câu)
- Cover nhiều topics khác nhau

### **Part 2:**
- Mỗi cue card cần 4 bullet points
- Chuẩn bị diverse topics (people, places, objects, events)
- Sample answer nên dài, detailed

### **Part 3:**
- Questions abstract hơn Part 1
- Yêu cầu critical thinking
- Liên quan trực tiếp đến Part 2 topic

### **Mock Test:**
- System tự động random từ pool questions
- Đảm bảo đủ questions trong database
- Mỗi mock test là unique combination

---

## 🚀 QUICK START

```bash
# 1. Create Part 1 topic
POST /api/topics
{
  "title": "Work",
  "partNumber": 1,
  "difficultyLevel": "beginner",
  "topicCategory": "personal"
}

# 2. Add 12 questions to that topic
POST /api/topics/{topicId}/questions
{
  "questionText": "What do you do?",
  "questionType": "Part 1",
  "timeLimitSeconds": 30
}
... (repeat 12 times)

# 3. Create Part 2 topic
POST /api/topics
{
  "title": "Describe a person you admire",
  "partNumber": 2,
  "difficultyLevel": "intermediate"
}

# 4. Add 1 cue card
POST /api/topics/{part2TopicId}/questions
{
  "questionText": "Describe a person you admire...",
  "questionType": "Part 2",
  "timeLimitSeconds": 120
}

# 5. Add 4-6 Part 3 questions
POST /api/topics/{part2TopicId}/questions
{
  "questionText": "What qualities make a good role model?",
  "questionType": "Part 3",
  "timeLimitSeconds": 45
}
... (repeat 4-6 times)

# 6. Start mock test
POST /api/mock-tests/start
{
  "part1QuestionCount": 12,
  "part2QuestionCount": 1,
  "part3QuestionCount": 4
}
```

---

## 📚 RESOURCES

### **Để tạo content, tham khảo:**
- IELTS Speaking Official Website
- Cambridge IELTS Practice Tests
- IELTS Liz Speaking Topics
- IELTS Simon Speaking Ideas

---

**Happy creating! 🎉**






