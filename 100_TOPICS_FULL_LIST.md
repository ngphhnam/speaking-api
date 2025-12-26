# 📚 DANH SÁCH ĐẦY ĐỦ 100 CHỦ ĐỀ IELTS SPEAKING

## ✅ 30 PART 1 TOPICS

### **Personal Information (10 topics)**
1. Work
2. Study
3. Hometown
4. Accommodation (Home/Apartment)
5. Family
6. Friends
7. Neighbors
8. Children
9. Age
10. Name

### **Daily Life (10 topics)**
11. Daily Routine
12. Food & Cooking
13. Shopping
14. Transport
15. Weather
16. Clothes & Fashion
17. Sleep
18. Weekends
19. Housework
20. Time Management

### **Leisure & Interests (10 topics)**
21. Hobbies
22. Sports
23. Music
24. Movies & TV
25. Reading & Books
26. Travel & Holidays
27. Art & Galleries
28. Photography
29. Gardens & Parks
30. Pets & Animals

---

## ✅ 70 PART 2 & 3 TOPICS

### **PEOPLE (12 topics)**
31. Describe a person you admire
32. Describe a family member you are close to
33. Describe a good friend
34. Describe a teacher who influenced you
35. Describe someone who helped you
36. Describe a famous person you would like to meet
37. Describe a person with an interesting job
38. Describe an old person you know
39. Describe a child you know
40. Describe someone you work/study with
41. Describe a neighbor you know well
42. Describe a person who made you laugh

### **PLACES (12 topics)**
43. Describe a place you visited
44. Describe your hometown
45. Describe your favorite place in your city
46. Describe a historic place
47. Describe a natural place (beach, forest, mountain)
48. Describe a city you want to visit
49. Describe a quiet place
50. Describe a place where you go to relax
51. Describe a shopping center
52. Describe a library
53. Describe a restaurant
54. Describe a café you like

### **OBJECTS & POSSESSIONS (10 topics)**
55. Describe something you own that is important to you
56. Describe a gift you received
57. Describe a photo you like
58. Describe a book you enjoyed
59. Describe a piece of technology you use
60. Describe an item of clothing you like
61. Describe a piece of furniture
62. Describe a vehicle you would like to own
63. Describe a toy from your childhood
64. Describe something you made yourself

### **EVENTS & EXPERIENCES (15 topics)**
65. Describe a memorable event
66. Describe a special occasion you celebrated
67. Describe a time you helped someone
68. Describe a difficult decision you made
69. Describe an achievement you are proud of
70. Describe a time you were late
71. Describe a mistake you made
72. Describe a time you got lost
73. Describe a happy memory from childhood
74. Describe a wedding you attended
75. Describe a party you enjoyed
76. Describe a time you received good news
77. Describe a time you were surprised
78. Describe an interesting conversation
79. Describe a journey you remember

### **ACTIVITIES & SKILLS (12 topics)**
80. Describe a hobby you have
81. Describe a sport you like to watch or play
82. Describe a skill you learned
83. Describe something you do to keep fit
84. Describe a course you took
85. Describe a project you worked on
86. Describe an outdoor activity you like
87. Describe a game you played as a child
88. Describe something you do to help the environment
89. Describe a musical instrument you would like to learn
90. Describe a language you would like to learn
91. Describe a creative activity you enjoy

### **MEDIA & TECHNOLOGY (9 topics)**
92. Describe a movie you enjoyed
93. Describe a TV program you like
94. Describe a website you often visit
95. Describe a mobile app you use
96. Describe an advertisement you remember
97. Describe a song that is special to you
98. Describe a piece of news you heard recently
99. Describe a social media platform you use
100. Describe a video game you have played

---

## 📊 BREAKDOWN

### **Part 1: 30 topics** 
- Mỗi topic: **10-12 questions**
- Total: **~330 questions**
- Time per question: 25-35 seconds

### **Part 2: 70 topics**
- Mỗi topic: **1 cue card**
- Total: **70 cue cards**
- Speaking time: 1-2 minutes
- Format: 4 bullet points

### **Part 3: 70 topics** (liên quan Part 2)
- Mỗi topic: **4-6 questions**
- Total: **~350 questions**
- Time per question: 45-50 seconds

---

## 🎯 GRAND TOTAL

- **Topics**: 100 (30 Part 1 + 70 Part 2)
- **Questions**: ~750 total
  - Part 1: ~330 questions
  - Part 2: 70 cue cards
  - Part 3: ~350 questions

---

## 📝 STRUCTURE FOR EACH TOPIC

### **Part 1 Topic Format:**
```json
{
  "topic": {
    "title": "Work",
    "partNumber": 1,
    "difficultyLevel": "beginner",
    "topicCategory": "personal"
  },
  "questions": [
    {"questionText": "...", "questionType": "Part 1", "timeLimitSeconds": 30},
    // 10-12 questions total
  ]
}
```

### **Part 2 Topic Format:**
```json
{
  "topic": {
    "title": "Describe a person you admire",
    "partNumber": 2,
    "difficultyLevel": "intermediate",
    "topicCategory": "people"
  },
  "part2_question": {
    "questionText": "Describe...\n\nYou should say:\n• point1\n• point2...",
    "questionType": "Part 2",
    "timeLimitSeconds": 120
  },
  "part3_questions": [
    {"questionText": "...", "questionType": "Part 3", "timeLimitSeconds": 45},
    // 4-6 questions total
  ]
}
```

---

## 🚀 HOW TO USE THIS

### **Option 1: Import từng topic manually**
Dùng API endpoints:
1. `POST /api/topics` - Tạo topic
2. `POST /api/topics/{id}/questions` - Add questions

### **Option 2: Bulk import (Recommended)**
Tạo seed data hoặc bulk import endpoint

### **Option 3: Generate full JSON**
File `100_IELTS_TOPICS_COMPLETE.json` có 6 topics mẫu.
Bạn muốn tôi expand thành 100 topics đầy đủ không?

---

## 📂 FILES

1. **100_TOPICS_FULL_LIST.md** (this file) - Danh sách tổng quan
2. **100_IELTS_TOPICS_COMPLETE.json** - 6 topics mẫu với full questions
3. **PART2_PART3_REQUEST_GUIDE.md** - Hướng dẫn tạo Part 2 & 3
4. **IELTS_TOPICS_CREATION_GUIDE.md** - Full guide

---

## 💡 NEXT STEPS

Bạn có muốn:
1. ✅ Expand JSON file với tất cả 100 topics đầy đủ questions?
2. ✅ Tạo seed data file để import vào database?
3. ✅ Tạo bulk import API endpoint?

**Note:** File JSON đầy đủ 100 topics sẽ rất lớn (~20,000+ lines).
Tốt nhất là tạo từng category riêng biệt.









