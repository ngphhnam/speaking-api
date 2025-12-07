namespace SpeakingPractice.Api.DTOs.Common;

/// <summary>
/// Standardized API response wrapper for all endpoints
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Metadata { get; set; }

    public static ApiResponse<T> Ok(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse<T> Error(string errorCode, string? message = null, Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            Metadata = metadata,
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Non-generic response for operations without data
/// </summary>
public class ApiResponse
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public string? ErrorCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object>? Metadata { get; set; }

    public static ApiResponse Ok(string? message = null)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            Timestamp = DateTime.UtcNow
        };
    }

    public static ApiResponse Error(string errorCode, string? message = null, Dictionary<string, object>? metadata = null)
    {
        return new ApiResponse
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            Metadata = metadata,
            Timestamp = DateTime.UtcNow
        };
    }
}

