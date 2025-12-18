# Hướng Dẫn Tạo Chủ Đề Part 2 và Part 3 trên UI

## Tổng Quan

Trong IELTS Speaking:
- **Part 2**: 1 topic với 1 cue card question (thời gian chuẩn bị + nói)
- **Part 3**: Cùng topic với Part 2, nhưng có nhiều discussion questions (4-6 câu)

## Flow Tạo Topic Part 2 + Part 3

### ⚡ Cách 1: Tạo Tất Cả Cùng Lúc (Khuyến Nghị)

**Endpoint:** `POST /api/Topics/with-questions`

**Request Body:**
```json
{
  "title": "Describe a Hobby",
  "description": "Describe a hobby you enjoy",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "activities",
  "keywords": ["hobby", "interest", "leisure"],
  "part2Question": {
    "questionText": "Describe a hobby you enjoy.\n\nYou should say:\n• what the hobby is\n• when you started it\n• how often you do it\n• and explain why you enjoy it",
    "questionType": "PART2",
    "questionStyle": "CueCard",
    "suggestedStructure": "Introduction - Name the hobby\nWhen started - Time and context\nFrequency - How often you do it\nWhy enjoy - Reasons and benefits\nConclusion - Summary",
    "keyVocabulary": ["hobby", "leisure", "pastime", "interest", "enjoy"],
    "estimatedBandRequirement": 6.5,
    "timeLimitSeconds": 120
  },
  "part3Questions": [
    {
      "questionText": "What are the benefits of having hobbies?",
      "questionType": "PART3",
      "questionStyle": "OpenEnded",
      "suggestedStructure": "Introduction - State benefits\nMain point 1 - Personal benefits\nMain point 2 - Social benefits\nConclusion - Summary",
      "keyVocabulary": ["benefit", "leisure", "wellbeing", "social"],
      "estimatedBandRequirement": 7.0,
      "timeLimitSeconds": 120
    },
    {
      "questionText": "Do you think children have enough time for hobbies today?",
      "questionType": "PART3",
      "questionStyle": "Opinion",
      "suggestedStructure": "Introduction - State your view\nMain point 1 - Current situation\nMain point 2 - Comparison with past\nConclusion - Your opinion",
      "keyVocabulary": ["children", "leisure", "time", "pressure"],
      "estimatedBandRequirement": 7.5,
      "timeLimitSeconds": 120
    },
    {
      "questionText": "How have people's hobbies changed with technology?",
      "questionType": "PART3",
      "questionStyle": "Comparison",
      "suggestedStructure": "Introduction - Acknowledge change\nMain point 1 - Old hobbies\nMain point 2 - New hobbies\nConclusion - Summary",
      "keyVocabulary": ["technology", "digital", "traditional", "modern"],
      "estimatedBandRequirement": 8.0,
      "timeLimitSeconds": 120
    },
    {
      "questionText": "Should schools encourage students to develop hobbies?",
      "questionType": "PART3",
      "questionStyle": "Opinion",
      "suggestedStructure": "Introduction - State your position\nMain point 1 - Arguments for\nMain point 2 - Arguments against\nConclusion - Balanced view",
      "keyVocabulary": ["school", "encourage", "development", "balance"],
      "estimatedBandRequirement": 7.5,
      "timeLimitSeconds": 120
    }
  ]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
    "title": "Describe a Hobby",
    "partNumber": 2,
    "totalQuestion": 5,
    "part2QuestionCount": 1,
    "part3QuestionCount": 4
  },
  "message": "Topic created successfully with 1 Part 2 question and 4 Part 3 questions"
}
```

**Lưu ý:**
- Endpoint này chỉ dành cho Part 2 topics (`partNumber: 2`)
- `part2Question` là **bắt buộc**
- `part3Questions` là **tùy chọn** (có thể để mảng rỗng hoặc null)
- Tất cả questions sẽ được tạo cùng lúc trong một transaction

---

### Cách 2: Tạo Từng Bước (Nếu Cần Kiểm Soát Chi Tiết)

### Bước 1: Tạo Topic Part 2

**Endpoint:** `POST /api/Topics`

**Request Body:**
```json
{
  "title": "Describe a Hobby",
  "description": "Describe a hobby you enjoy",
  "partNumber": 2,
  "difficultyLevel": "intermediate",
  "topicCategory": "activities",
  "keywords": ["hobby", "interest", "leisure"]
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "id": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
    "title": "Describe a Hobby",
    "slug": "describe-hobby",
    "partNumber": 2,
    ...
  }
}
```

**Lưu `topicId` từ response để dùng cho các bước sau.**

---

### Bước 2: Tạo Part 2 Question (Cue Card)

**Endpoint:** `POST /api/Questions`

**Request Body:**
```json
{
  "topicId": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
  "questionText": "Describe a hobby you enjoy.\n\nYou should say:\n• what the hobby is\n• when you started it\n• how often you do it\n• and explain why you enjoy it",
  "questionType": "PART2",
  "questionStyle": "CueCard",
  "suggestedStructure": "Introduction - Name the hobby\nWhen started - Time and context\nFrequency - How often you do it\nWhy enjoy - Reasons and benefits\nConclusion - Summary",
  "keyVocabulary": ["hobby", "leisure", "pastime", "interest", "enjoy"],
  "estimatedBandRequirement": 6.5,
  "timeLimitSeconds": 120
}
```

**Lưu ý:**
- `questionType`: Phải là `"PART2"`
- `questionStyle`: Phải là `"CueCard"`
- `timeLimitSeconds`: Thường là 120 giây (2 phút)
- `questionText`: Nên có format cue card với các bullet points

---

### Bước 3: Tạo Part 3 Questions (Discussion Questions)

**Endpoint:** `POST /api/Questions` (gọi nhiều lần, mỗi lần 1 question)

**Request Body (Question 1):**
```json
{
  "topicId": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
  "questionText": "What are the benefits of having hobbies?",
  "questionType": "PART3",
  "questionStyle": "OpenEnded",
  "suggestedStructure": "Introduction - State benefits\nMain point 1 - Personal benefits\nMain point 2 - Social benefits\nConclusion - Summary",
  "keyVocabulary": ["benefit", "leisure", "wellbeing", "social"],
  "estimatedBandRequirement": 7.0,
  "timeLimitSeconds": 120
}
```

**Request Body (Question 2):**
```json
{
  "topicId": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
  "questionText": "Do you think children have enough time for hobbies today?",
  "questionType": "PART3",
  "questionStyle": "Opinion",
  "suggestedStructure": "Introduction - State your view\nMain point 1 - Current situation\nMain point 2 - Comparison with past\nConclusion - Your opinion",
  "keyVocabulary": ["children", "leisure", "time", "pressure"],
  "estimatedBandRequirement": 7.5,
  "timeLimitSeconds": 120
}
```

**Request Body (Question 3):**
```json
{
  "topicId": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
  "questionText": "How have people's hobbies changed with technology?",
  "questionType": "PART3",
  "questionStyle": "Comparison",
  "suggestedStructure": "Introduction - Acknowledge change\nMain point 1 - Old hobbies\nMain point 2 - New hobbies\nConclusion - Summary",
  "keyVocabulary": ["technology", "digital", "traditional", "modern"],
  "estimatedBandRequirement": 8.0,
  "timeLimitSeconds": 120
}
```

**Request Body (Question 4):**
```json
{
  "topicId": "68cc0837-8313-4f2f-a48a-0b75fda82ad6",
  "questionText": "Should schools encourage students to develop hobbies?",
  "questionType": "PART3",
  "questionStyle": "Opinion",
  "suggestedStructure": "Introduction - State your position\nMain point 1 - Arguments for\nMain point 2 - Arguments against\nConclusion - Balanced view",
  "keyVocabulary": ["school", "encourage", "development", "balance"],
  "estimatedBandRequirement": 7.5,
  "timeLimitSeconds": 120
}
```

**Lưu ý:**
- `questionType`: Phải là `"PART3"`
- `questionStyle`: Có thể là `"OpenEnded"`, `"Opinion"`, `"Comparison"`, `"CauseEffect"`, `"Prediction"`
- `timeLimitSeconds`: Thường là 120 giây
- Nên tạo 4-6 Part 3 questions cho mỗi topic

---

## UI Flow Đề Xuất

### Option 1: Form Đơn Giản (Step-by-Step)

1. **Step 1: Tạo Topic**
   - Form: Title, Description, Category, Keywords
   - Auto-set: `partNumber = 2`
   - Button: "Create Topic & Continue"

2. **Step 2: Tạo Part 2 Question**
   - Form: Cue Card Text (textarea với format)
   - Auto-set: `questionType = "PART2"`, `questionStyle = "CueCard"`
   - Button: "Save Part 2 Question & Continue"

3. **Step 3: Tạo Part 3 Questions**
   - Form: Multiple questions (có thể thêm/xóa)
   - Mỗi question có: Text, Style, Structure, Vocabulary
   - Auto-set: `questionType = "PART3"`
   - Button: "Save All Part 3 Questions & Finish"

### Option 2: Form Tổng Hợp (Single Page)

1. **Section 1: Topic Information**
   - Title, Description, Category, Keywords

2. **Section 2: Part 2 Question**
   - Cue Card Text Editor
   - Suggested Structure
   - Key Vocabulary

3. **Section 3: Part 3 Questions**
   - List of questions (có thể thêm/xóa)
   - Mỗi question có: Text, Style, Structure, Vocabulary

4. **Button: "Save Topic & All Questions"**
   - Gọi API theo thứ tự: Topic → Part 2 Question → Part 3 Questions

---

## QuestionStyle Values cho Part 3

- `"OpenEnded"`: Câu hỏi mở (What are the benefits...?)
- `"Opinion"`: Câu hỏi ý kiến (Do you think...?)
- `"Comparison"`: Câu hỏi so sánh (How have... changed?)
- `"CauseEffect"`: Câu hỏi nguyên nhân - kết quả (What influence does...?)
- `"Prediction"`: Câu hỏi dự đoán (How will... change in the future?)

---

## Validation Rules

1. **Topic Part 2:**
   - `partNumber` phải là `2`
   - Title không được trống
   - Slug sẽ tự động generate từ Title

2. **Part 2 Question:**
   - Phải có `topicId`
   - `questionType` phải là `"PART2"`
   - `questionStyle` phải là `"CueCard"`
   - `timeLimitSeconds` thường là 120

3. **Part 3 Questions:**
   - Phải có `topicId` (cùng với Part 2)
   - `questionType` phải là `"PART3"`
   - `questionStyle` không được là `"CueCard"`
   - Nên có ít nhất 4 questions

---

## Example: Complete Flow

```javascript
// 1. Create Topic
const topicResponse = await fetch('/api/Topics', {
  method: 'POST',
  body: JSON.stringify({
    title: "Describe a Hobby",
    description: "Describe a hobby you enjoy",
    partNumber: 2,
    difficultyLevel: "intermediate",
    topicCategory: "activities"
  })
});
const topic = await topicResponse.json();
const topicId = topic.data.id;

// 2. Create Part 2 Question
await fetch('/api/Questions', {
  method: 'POST',
  body: JSON.stringify({
    topicId: topicId,
    questionText: "Describe a hobby you enjoy...",
    questionType: "PART2",
    questionStyle: "CueCard",
    timeLimitSeconds: 120
  })
});

// 3. Create Part 3 Questions
const part3Questions = [
  {
    topicId: topicId,
    questionText: "What are the benefits of having hobbies?",
    questionType: "PART3",
    questionStyle: "OpenEnded",
    timeLimitSeconds: 120
  },
  {
    topicId: topicId,
    questionText: "Do you think children have enough time for hobbies today?",
    questionType: "PART3",
    questionStyle: "Opinion",
    timeLimitSeconds: 120
  }
  // ... more questions
];

for (const question of part3Questions) {
  await fetch('/api/Questions', {
    method: 'POST',
    body: JSON.stringify(question)
  });
}
```

---

## Tips cho UI

1. **Auto-fill**: Khi tạo Part 3 questions, có thể suggest `topicId` từ Part 2
2. **Validation**: Check `partNumber = 2` trước khi cho phép tạo Part 2 question
3. **Preview**: Hiển thị preview topic với tất cả questions trước khi save
4. **Bulk Create**: Có thể tạo endpoint mới để tạo nhiều questions cùng lúc (optional)

