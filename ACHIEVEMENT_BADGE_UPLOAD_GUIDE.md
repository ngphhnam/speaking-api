# Hướng Dẫn Upload Badge Icon cho Achievement

Vì tất cả hình ảnh được host trên MinIO, bạn cần upload badge icon trước khi tạo hoặc cập nhật achievement.

## 🎯 Quy Trình Upload Badge Icon

### Cách 1: Upload riêng rồi tạo Achievement (Khuyến nghị)

#### Bước 1: Upload Badge Icon
```http
POST /api/Achievements/upload-badge-icon
Authorization: Bearer {admin-token}
Content-Type: multipart/form-data

Body:
- badgeIcon: [Chọn file hình ảnh]
```

**Response:**
```json
{
  "data": {
    "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_xxx_timestamp.png"
  },
  "message": "Badge icon uploaded successfully",
  "success": true
}
```

#### Bước 2: Tạo Achievement với URL vừa nhận được
```http
POST /api/Achievements
Authorization: Bearer {admin-token}
Content-Type: application/json

{
  "title": "First Steps",
  "description": "Complete your first speaking practice session",
  "achievementType": "milestone",
  "requirementCriteria": "{\"type\": \"session_count\", \"target\": 1}",
  "points": 10,
  "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_xxx_timestamp.png"
}
```

---

### Cách 2: Upload và update badge cho Achievement đã tồn tại

Nếu bạn đã tạo achievement rồi muốn thêm/đổi badge icon:

```http
POST /api/Achievements/{achievementId}/upload-badge-icon
Authorization: Bearer {admin-token}
Content-Type: multipart/form-data

Body:
- badgeIcon: [Chọn file hình ảnh]
```

**Response:**
```json
{
  "data": {
    "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_achievementId_timestamp.png",
    "achievement": {
      "id": "...",
      "title": "First Steps",
      "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_achievementId_timestamp.png",
      ...
    }
  },
  "message": "Badge icon uploaded and updated successfully",
  "success": true
}
```

---

## 📋 Yêu Cầu File

### Định dạng cho phép:
- ✅ JPEG / JPG
- ✅ PNG
- ✅ GIF
- ✅ WEBP
- ✅ SVG

### Kích thước:
- **Tối đa**: 5MB
- **Khuyến nghị**: 100x100px hoặc 256x256px (vuông)

---

## 🔧 Test với Postman

### 1. Upload Badge Icon

1. Chọn request **"Upload Badge Icon (Admin)"**
2. Chọn tab **Body** → **form-data**
3. Chọn **badgeIcon** → **Select Files** → Chọn file hình
4. Click **Send**
5. **Copy URL** từ response (`badgeIconUrl`)

### 2. Create Achievement

1. Chọn request **"Create Achievement (Admin)"**
2. Trong Body, **paste URL** vào field `badgeIconUrl`
3. Click **Send**

---

## 💡 Ví Dụ Hoàn Chỉnh

### Scenario: Tạo achievement "Speaking Master"

#### Step 1: Upload badge icon
```bash
curl -X POST "http://localhost:5000/api/Achievements/upload-badge-icon" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -F "badgeIcon=@speaking-master-badge.png"
```

**Response:**
```json
{
  "data": {
    "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_abc123_1234567890.png"
  },
  "message": "Badge icon uploaded successfully"
}
```

#### Step 2: Create achievement
```bash
curl -X POST "http://localhost:5000/api/Achievements" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Speaking Master",
    "description": "Complete 100 speaking sessions",
    "achievementType": "milestone",
    "requirementCriteria": "{\"type\": \"session_count\", \"target\": 100}",
    "points": 500,
    "badgeIconUrl": "http://localhost:9000/avatars/badges/badge_abc123_1234567890.png"
  }'
```

---

## 🎨 Badge Icon Storage

Tất cả badge icons được lưu trong MinIO bucket `avatars` với path:
```
avatars/badges/badge_{guid}_{timestamp}.{ext}
```

Ví dụ:
- `avatars/badges/badge_abc123_1702345678.png`
- `avatars/badges/badge_def456_1702345679.svg`

---

## ⚠️ Lưu Ý

1. **Chỉ Admin** mới có quyền upload badge icon và tạo achievement
2. Badge icon được lưu **vĩnh viễn** trên MinIO
3. Nếu xóa achievement, badge icon **vẫn tồn tại** trên MinIO
4. URL badge icon phải **công khai** để user có thể xem
5. MinIO tự động set public-read policy cho bucket

---

## 🔐 Authentication

Tất cả endpoints đều yêu cầu:
- **Role**: Admin
- **Header**: `Authorization: Bearer {access-token}`

Để lấy admin token:
1. Register hoặc login với user có role "Admin"
2. Copy `accessToken` từ response
3. Sử dụng token này cho các request

---

## 📝 Achievement Types (Gợi ý)

Một số `achievementType` bạn có thể dùng:

- `milestone` - Cột mốc quan trọng
- `streak` - Liên quan đến streak days
- `score` - Đạt điểm số nhất định
- `practice` - Luyện tập nhiều
- `vocabulary` - Học từ vựng
- `perfect` - Hoàn thành hoàn hảo

---

## 🎯 Requirement Criteria Examples

JSON string cho `requirementCriteria`:

### Session count:
```json
{
  "type": "session_count",
  "target": 10
}
```

### Score achievement:
```json
{
  "type": "avg_score",
  "target": 7.5
}
```

### Streak days:
```json
{
  "type": "streak_days",
  "target": 30
}
```

### Vocabulary mastered:
```json
{
  "type": "vocabulary_mastered",
  "target": 100
}
```

---

## 🚀 Quick Start

### Postman Collection đã có sẵn:

1. **Upload Badge Icon (Admin)** - Upload hình trước
2. **Create Achievement (Admin)** - Tạo achievement với URL
3. **Update Achievement (Admin)** - Cập nhật thông tin
4. **Upload and Update Badge Icon (Admin)** - Upload và update cùng lúc
5. **Delete Achievement (Admin)** - Xóa achievement

Tất cả đã được config sẵn trong `SpeakingPractice_API.postman_collection.json`!

---

## 🎉 Done!

Bây giờ bạn có thể tạo achievements với badge icons được host trên MinIO server của riêng bạn! 🚀






