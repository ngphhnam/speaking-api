namespace SpeakingPractice.Api.DTOs.Common;

/// <summary>
/// Standard error codes for multi-language support
/// Frontend can map these codes to localized messages
/// </summary>
public static class ErrorCodes
{
    // Authentication & Authorization (1000-1999)
    public const string UNAUTHORIZED = "AUTH_001";
    public const string FORBIDDEN = "AUTH_002";
    public const string TOKEN_EXPIRED = "AUTH_003";
    public const string TOKEN_INVALID = "AUTH_004";
    public const string INVALID_CREDENTIALS = "AUTH_005";
    public const string USER_NOT_FOUND = "AUTH_006";
    public const string USER_ALREADY_EXISTS = "AUTH_007";
    public const string EMAIL_NOT_VERIFIED = "AUTH_008";

    // Validation Errors (2000-2999)
    public const string VALIDATION_ERROR = "VAL_001";
    public const string REQUIRED_FIELD_MISSING = "VAL_002";
    public const string INVALID_FORMAT = "VAL_003";
    public const string INVALID_VALUE = "VAL_004";

    // Resource Not Found (3000-3999)
    public const string NOT_FOUND = "RES_001";
    public const string QUESTION_NOT_FOUND = "RES_002";
    public const string TOPIC_NOT_FOUND = "RES_003";
    public const string SESSION_NOT_FOUND = "RES_004";
    public const string RECORDING_NOT_FOUND = "RES_005";
    public const string USER_NOT_FOUND_RESOURCE = "RES_006";

    // Business Logic Errors (4000-4999)
    public const string QUESTION_NOT_ACTIVE = "BIZ_001";
    public const string SESSION_ALREADY_COMPLETED = "BIZ_002";
    public const string SESSION_NOT_BELONGS_TO_USER = "BIZ_003";
    public const string INVALID_SESSION_STATUS = "BIZ_004";
    public const string TOPIC_NOT_ACTIVE = "BIZ_005";

    // File/Upload Errors (5000-5999)
    public const string FILE_REQUIRED = "FILE_001";
    public const string FILE_TOO_LARGE = "FILE_002";
    public const string INVALID_FILE_TYPE = "FILE_003";
    public const string UPLOAD_FAILED = "FILE_004";

    // External Service Errors (6000-6999)
    public const string EXTERNAL_SERVICE_UNAVAILABLE = "EXT_001";
    public const string TRANSCRIPTION_FAILED = "EXT_002";
    public const string SCORING_FAILED = "EXT_003";
    public const string GRAMMAR_CHECK_FAILED = "EXT_004";

    // Database Errors (7000-7999)
    public const string DATABASE_ERROR = "DB_001";
    public const string FOREIGN_KEY_VIOLATION = "DB_002";
    public const string UNIQUE_CONSTRAINT_VIOLATION = "DB_003";

    // Generic Server Errors (9000-9999)
    public const string INTERNAL_SERVER_ERROR = "SRV_001";
    public const string OPERATION_FAILED = "SRV_002";
    public const string TIMEOUT = "SRV_003";
}















