# 👥 Hướng dẫn Seed Users Data

## ✅ File đã tạo

File `UsersSeeder.cs` đã được tạo với **6 sample users**:

### 1. Admin User (1 user)
- **Email**: `admin@speakingpractice.com`
- **Password**: `Admin@123456`
- **Role**: Admin
- **Subscription**: Premium (permanent)
- **Level**: 10
- **XP**: 5000

### 2. Regular Users (5 users)
- **Email**: `user1@test.com` đến `user5@test.com`
- **Password**: `User@123456`
- **Role**: User
- **Subscription**: Mix of Premium và Free
- **Level**: 1-8
- **Streak**: 0-20 days (varied)

---

## 🚀 Cách sử dụng

### Option 1: Tự động seed khi chạy với `--seed` flag

File `Program.cs` đã được cập nhật để tự động seed users:

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

// Seed users
using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await SpeakingPractice.Api.DataSeed.UsersSeeder.SeedAsync(userManager, roleManager);
}

app.Run();
```

---

## 🔐 Login Credentials

### Admin
```
Email: admin@speakingpractice.com
Password: Admin@123456
```

### Regular Users
```
Email: user1@test.com
Password: User@123456

Email: user2@test.com
Password: User@123456

... (user3, user4, user5)
```

---

## 🔍 Kiểm tra sau khi seed

### 1. Kiểm tra trong database:

```sql
SELECT 
    email,
    full_name,
    subscription_type,
    level,
    experience_points,
    current_streak,
    is_active
FROM users
ORDER BY created_at;
```

### 2. Kiểm tra roles:

```sql
SELECT 
    u.email,
    r.name as role
FROM users u
JOIN user_roles ur ON u.id = ur.user_id
JOIN roles r ON ur.role_id = r.id
ORDER BY u.email;
```

### 3. Test login qua API:

```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "admin@speakingpractice.com",
  "password": "Admin@123456"
}
```

---

## 📊 User Details

### Admin User
- ✅ Email verified
- ✅ Premium subscription (permanent)
- ✅ Level 10
- ✅ 5000 XP
- ✅ Advanced level

### Regular Users
- ✅ Mix of verified/unverified emails
- ✅ Mix of Premium và Free subscriptions
- ✅ Various levels (1-8)
- ✅ Different streak statuses
- ✅ Different practice histories

---

## ⚠️ Lưu ý

1. **Seeder tự động skip nếu đã có data**: Seeder sẽ kiểm tra `if (await userManager.Users.AnyAsync())` và không seed lại nếu đã có users.

2. **Để seed lại**: 
   - **CẨN THẬN**: Xóa users sẽ xóa tất cả data liên quan (sessions, recordings, achievements, etc.)
   ```sql
   -- Xóa user achievements trước
   DELETE FROM user_achievements;
   
   -- Xóa recordings và sessions
   DELETE FROM recordings;
   DELETE FROM practice_sessions;
   
   -- Xóa users (sẽ cascade delete các bảng liên quan)
   DELETE FROM users;
   ```

3. **Passwords**: Tất cả passwords đều tuân thủ yêu cầu (minimum 8 characters, có chữ hoa, chữ thường, số).

4. **GUIDs cố định**: Tất cả users có GUID cố định để dễ reference trong testing.

5. **Roles**: Seeder tự động tạo roles nếu chưa tồn tại (Admin, User).

---

## 🎯 Use Cases

### Testing Premium Features
- Sử dụng `user1@test.com` hoặc `user5@test.com` (premium users)
- Test unlimited practice sessions
- Test premium-only features

### Testing Free User Limits
- Sử dụng `user2@test.com`, `user3@test.com`, `user4@test.com` (free users)
- Test daily practice limit (5 sessions/day)
- Test free tier restrictions

### Testing Admin Features
- Sử dụng `admin@speakingpractice.com`
- Test admin endpoints
- Test user management

### Testing Streak System
- `user5@test.com` - có streak 20 ngày (active)
- `user1@test.com` - có streak 15 ngày (active)
- `user3@test.com` - streak đã bị mất (last practice 2 ngày trước)

---

## 📝 Next Steps

1. ✅ Chạy seed để tạo users
2. ⏳ Test login với các accounts
3. ⏳ Test các features với different user types
4. ⏳ Test streak system với users có streak khác nhau
5. ⏳ Test achievement system với users có XP khác nhau

---

## 🔄 Customization

Bạn có thể modify `UsersSeeder.cs` để:
- Thêm nhiều users hơn
- Thay đổi passwords
- Thay đổi subscription types
- Thay đổi levels và XP
- Thêm custom data (bio, avatar, etc.)

