# API Response Wrapper - Hướng dẫn sử dụng

## Tổng quan

Tất cả API trong project này đều trả về format chuẩn để dễ dàng xử lý ở frontend và hỗ trợ đa ngôn ngữ (multi-language).

## Cấu trúc Response

### Success Response (có data)
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully",
  "timestamp": "2024-12-02T10:30:00Z"
}
```

### Success Response (không có data)
```json
{
  "success": true,
  "message": "Operation completed successfully",
  "timestamp": "2024-12-02T10:30:00Z"
}
```

### Error Response
```json
{
  "success": false,
  "errorCode": "AUTH_001",
  "message": "User not authenticated",
  "metadata": {
    "field": "email",
    "reason": "Invalid format"
  },
  "timestamp": "2024-12-02T10:30:00Z"
}
```

## Cách sử dụng trong Controller

### 1. Import namespace
```csharp
using SpeakingPractice.Api.DTOs.Common;
using SpeakingPractice.Api.Infrastructure.Extensions;
```

### 2. Success responses

**Có data:**
```csharp
return this.ApiOk(data, "Message tùy chọn");
```

**Không có data:**
```csharp
return this.ApiOk("Message tùy chọn");
```

**Created (201):**
```csharp
return this.ApiCreated(nameof(GetById), new { id = result.Id }, result, "Created successfully");
// hoặc
return this.ApiCreated("/api/sessions/123", result, "Created successfully");
```

### 3. Error responses

**Bad Request (400):**
```csharp
return this.ApiBadRequest(ErrorCodes.VALIDATION_ERROR, "Invalid input", metadata);
```

**Unauthorized (401):**
```csharp
return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
```

**Forbidden (403):**
```csharp
return this.ApiForbid(ErrorCodes.FORBIDDEN, "Access denied");
```

**Not Found (404):**
```csharp
return this.ApiNotFound(ErrorCodes.SESSION_NOT_FOUND, "Session not found");
```

**Internal Server Error (500):**
```csharp
return this.ApiInternalServerError(ErrorCodes.INTERNAL_SERVER_ERROR, "An error occurred");
```

**Custom status code:**
```csharp
return this.ApiStatusCode(503, ErrorCodes.EXTERNAL_SERVICE_UNAVAILABLE, "Service unavailable");
```

## Error Codes

Tất cả error codes được định nghĩa trong `ErrorCodes` class:

- **AUTH_001 - AUTH_008**: Authentication & Authorization errors
- **VAL_001 - VAL_004**: Validation errors
- **RES_001 - RES_006**: Resource not found errors
- **BIZ_001 - BIZ_005**: Business logic errors
- **FILE_001 - FILE_004**: File/Upload errors
- **EXT_001 - EXT_004**: External service errors
- **DB_001 - DB_003**: Database errors
- **SRV_001 - SRV_003**: Server errors

## Ví dụ hoàn chỉnh

```csharp
[HttpPost]
public async Task<IActionResult> Create([FromBody] CreateRequest request, CancellationToken ct)
{
    var userId = GetUserId();
    if (userId is null)
    {
        return this.ApiUnauthorized(ErrorCodes.UNAUTHORIZED, "User not authenticated");
    }

    try
    {
        var result = await service.CreateAsync(request, userId.Value, ct);
        return this.ApiCreated(nameof(GetById), new { id = result.Id }, result, "Created successfully");
    }
    catch (NotFoundException ex)
    {
        return this.ApiNotFound(ErrorCodes.RESOURCE_NOT_FOUND, ex.Message);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error creating resource");
        return this.ApiInternalServerError(ErrorCodes.OPERATION_FAILED, "Failed to create resource");
    }
}
```

## Frontend Integration

Frontend có thể map error codes sang message theo ngôn ngữ:

```typescript
const errorMessages: Record<string, Record<string, string>> = {
  vi: {
    'AUTH_001': 'Người dùng chưa đăng nhập',
    'AUTH_002': 'Không có quyền truy cập',
    'RES_001': 'Không tìm thấy tài nguyên',
    // ...
  },
  en: {
    'AUTH_001': 'User not authenticated',
    'AUTH_002': 'Access denied',
    'RES_001': 'Resource not found',
    // ...
  }
};

// Sử dụng
const message = errorMessages[locale][response.errorCode] || response.message;
```
















