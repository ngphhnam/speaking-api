# ✅ DATETIME FIX - PostgreSQL UTC Issue RESOLVED

## 🔴 **Vấn đề ban đầu:**

```json
{
  "error": "Cannot write DateTime with Kind=Unspecified to PostgreSQL type 'timestamp with time zone', only UTC is supported"
}
```

### Khi update profile:
```json
{
  "fullName": "Admin",
  "examDate": "2024-12-01",
  "targetBandScore": 7.5
}
```

**Nguyên nhân:** 
- PostgreSQL yêu cầu DateTime phải có `Kind=UTC`
- Khi gửi date string `"2024-12-01"`, ASP.NET deserialize thành `DateTime` với `Kind=Unspecified`
- Entity Framework không thể save vào PostgreSQL

---

## ✅ **Giải pháp:**

**Đổi `DateTime?` → `DateOnly?`** cho các field chỉ cần ngày (không cần giờ)

### Fields được convert:
1. ✅ `DateOfBirth` - Ngày sinh
2. ✅ `ExamDate` - Ngày thi

**Tại sao DateOnly tốt hơn:**
- ✅ Không có timezone issues
- ✅ Chỉ lưu ngày, không lưu giờ (đúng mục đích)
- ✅ Nhẹ hơn (date vs timestamp)
- ✅ API clear hơn (`"2024-12-01"` thay vì `"2024-12-01T00:00:00Z"`)

---

## 📊 **Database Changes**

### Before (timestamp with time zone):
```sql
date_of_birth | timestamp with time zone
exam_date     | timestamp with time zone
```

### After (date only):
```sql
date_of_birth | date
exam_date     | date
```

**Migration applied:**
```sql
ALTER TABLE users ALTER COLUMN exam_date TYPE date;
ALTER TABLE users ALTER COLUMN date_of_birth TYPE date;
```

---

## 📁 **Files Changed**

1. ✅ `Domain/Entities/ApplicationUser.cs`
   - `DateTime? DateOfBirth` → `DateOnly? DateOfBirth`
   - `DateTime? ExamDate` → `DateOnly? ExamDate`

2. ✅ `DTOs/Common/UserDto.cs`
   - Updated to use `DateOnly?`

3. ✅ `DTOs/Auth/UpdateProfileRequest.cs`
   - Updated to use `DateOnly?`

4. ✅ `Controllers/UserSettingsController.cs`
   - `UserSettingsDto` updated
   - `UpdateUserSettingsRequest` updated

5. ✅ `Migrations/20251212082940_ConvertDateTimeToDateOnly.cs`
   - Migration created and applied

---

## 🎯 **API Behavior**

### ✅ Request (Giờ đây hoạt động):
```http
PUT /api/auth/profile
Content-Type: application/json

{
  "fullName": "Admin",
  "examDate": "2024-12-01",
  "targetBandScore": 7.5
}
```

### ✅ Response:
```json
{
  "success": true,
  "data": {
    "id": "...",
    "fullName": "Admin",
    "examDate": "2024-12-01",        // ← DateOnly format
    "dateOfBirth": "1995-05-15",     // ← DateOnly format
    "targetBandScore": 7.5,
    ...
  }
}
```

### ✅ Format hợp lệ:
```json
{
  "examDate": "2024-12-01",           // ✅ OK
  "dateOfBirth": "1995-05-15"         // ✅ OK
}
```

### ❌ Format KHÔNG hợp lệ (sẽ validation error):
```json
{
  "examDate": "2024-12-01T10:30:00Z", // ❌ NO - có time
  "dateOfBirth": "05/15/1995"         // ❌ NO - wrong format
}
```

---

## 🧪 **Test**

### Test 1: Update với examDate
```bash
curl -X PUT http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "Admin",
    "examDate": "2024-12-01",
    "targetBandScore": 7.5
  }'
```

**Expected:** ✅ Success, không còn lỗi UTC

### Test 2: Update với dateOfBirth
```bash
curl -X PUT http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "dateOfBirth": "1995-05-15"
  }'
```

**Expected:** ✅ Success

### Test 3: Xem profile
```bash
curl -X GET http://localhost:5000/api/auth/profile \
  -H "Authorization: Bearer YOUR_TOKEN"
```

**Expected Response:**
```json
{
  "examDate": "2024-12-01",      // DateOnly format
  "dateOfBirth": "1995-05-15"    // DateOnly format
}
```

---

## 📊 **Database Verification**

### Check column types:
```sql
\d users

-- Should show:
-- date_of_birth | date
-- exam_date     | date
```

### Query examples:
```sql
-- Insert (correct format)
UPDATE users 
SET exam_date = '2024-12-01'
WHERE id = 'user-id';

-- Query
SELECT 
    full_name,
    date_of_birth,
    exam_date
FROM users
WHERE exam_date >= '2024-01-01';
```

---

## 🔒 **Other DateTime Fields** (Không đổi)

Các field sau vẫn dùng `DateTime/DateTimeOffset` vì cần time:

| Field | Type | Lý do |
|-------|------|-------|
| `LastLoginAt` | DateTime? | Cần giờ để track login time |
| `SubscriptionExpiresAt` | DateTime? | Cần giờ chính xác |
| `CreatedAt` | DateTimeOffset | Timestamp với timezone |
| `UpdatedAt` | DateTimeOffset | Timestamp với timezone |
| `LastPracticeDate` | DateOnly? | ✅ Đã dùng DateOnly từ đầu |

---

## 💡 **Best Practices**

### Khi nào dùng DateOnly:
✅ Ngày sinh  
✅ Ngày thi  
✅ Deadline (chỉ ngày)  
✅ Holiday dates  
✅ Scheduled dates  

### Khi nào dùng DateTime/DateTimeOffset:
✅ Login timestamp  
✅ Session expiry  
✅ Created/Updated timestamps  
✅ Event start time  
✅ Appointment time  

---

## 🚨 **Common Pitfalls**

### ❌ WRONG:
```csharp
// Đừng dùng DateTime cho date-only fields
public DateTime? BirthDate { get; set; }
```

### ✅ CORRECT:
```csharp
// Dùng DateOnly cho date-only fields
public DateOnly? BirthDate { get; set; }
```

---

## 🎉 **Summary**

| Before | After |
|--------|-------|
| ❌ DateTime with Kind=Unspecified | ✅ DateOnly (no timezone) |
| ❌ PostgreSQL error | ✅ Works perfectly |
| ❌ Confusing timestamp format | ✅ Clean date format |
| ❌ Timezone issues | ✅ No timezone issues |

---

## 🔄 **Rollback** (nếu cần)

```bash
# Revert migration
dotnet ef database update 20251212061447_AddBioToUsers

# Remove migration
dotnet ef migrations remove
```

---

## 📝 **Migration Info**

```bash
✅ Migration: 20251212082940_ConvertDateTimeToDateOnly
✅ Status: Applied successfully
✅ Changes:
   - date_of_birth: timestamp with time zone → date
   - exam_date: timestamp with time zone → date
```

---

## 🎯 **Next Steps**

1. ✅ Test update profile với examDate
2. ✅ Test update profile với dateOfBirth
3. ✅ Verify database columns
4. ✅ Update frontend nếu cần (DateOnly format)

---

**Created:** 2024-12-12  
**Migration ID:** 20251212082940_ConvertDateTimeToDateOnly  
**Status:** ✅ RESOLVED  
**Issue:** PostgreSQL UTC DateTime error  
**Solution:** Convert DateTime? to DateOnly? for date-only fields









