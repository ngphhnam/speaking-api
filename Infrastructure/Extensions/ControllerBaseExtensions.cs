using Microsoft.AspNetCore.Mvc;
using SpeakingPractice.Api.DTOs.Common;

namespace SpeakingPractice.Api.Infrastructure.Extensions;

/// <summary>
/// Extension methods for ControllerBase to easily return standardized API responses
/// </summary>
public static class ControllerBaseExtensions
{
    /// <summary>
    /// Returns a successful response with data
    /// </summary>
    public static IActionResult ApiOk<T>(this ControllerBase controller, T data, string? message = null)
    {
        return controller.Ok(ApiResponse<T>.Ok(data, message));
    }

    /// <summary>
    /// Returns a successful response without data
    /// </summary>
    public static IActionResult ApiOk(this ControllerBase controller, string? message = null)
    {
        return controller.Ok(ApiResponse.Ok(message));
    }

    /// <summary>
    /// Returns a 201 Created response with data
    /// </summary>
    public static IActionResult ApiCreated<T>(this ControllerBase controller, string actionName, object routeValues, T data, string? message = null)
    {
        return controller.CreatedAtAction(actionName, routeValues, ApiResponse<T>.Ok(data, message));
    }

    /// <summary>
    /// Returns a 201 Created response with data (using URL)
    /// </summary>
    public static IActionResult ApiCreated<T>(this ControllerBase controller, string uri, T data, string? message = null)
    {
        return controller.Created(uri, ApiResponse<T>.Ok(data, message));
    }

    /// <summary>
    /// Returns a 400 Bad Request with error code
    /// </summary>
    public static IActionResult ApiBadRequest(this ControllerBase controller, string errorCode, string? message = null, Dictionary<string, object>? metadata = null)
    {
        return controller.BadRequest(ApiResponse.Error(errorCode, message, metadata));
    }

    /// <summary>
    /// Returns a 401 Unauthorized with error code
    /// </summary>
    public static IActionResult ApiUnauthorized(this ControllerBase controller, string errorCode = ErrorCodes.UNAUTHORIZED, string? message = null)
    {
        return controller.Unauthorized(ApiResponse.Error(errorCode, message));
    }

    /// <summary>
    /// Returns a 403 Forbidden with error code
    /// </summary>
    public static IActionResult ApiForbid(this ControllerBase controller, string errorCode = ErrorCodes.FORBIDDEN, string? message = null)
    {
        return controller.StatusCode(403, ApiResponse.Error(errorCode, message));
    }

    /// <summary>
    /// Returns a 404 Not Found with error code
    /// </summary>
    public static IActionResult ApiNotFound(this ControllerBase controller, string errorCode = ErrorCodes.NOT_FOUND, string? message = null)
    {
        return controller.NotFound(ApiResponse.Error(errorCode, message));
    }

    /// <summary>
    /// Returns a 500 Internal Server Error with error code
    /// </summary>
    public static IActionResult ApiInternalServerError(this ControllerBase controller, string errorCode = ErrorCodes.INTERNAL_SERVER_ERROR, string? message = null, Dictionary<string, object>? metadata = null)
    {
        return controller.StatusCode(500, ApiResponse.Error(errorCode, message, metadata));
    }

    /// <summary>
    /// Returns a custom status code with error code
    /// </summary>
    public static IActionResult ApiStatusCode(this ControllerBase controller, int statusCode, string errorCode, string? message = null, Dictionary<string, object>? metadata = null)
    {
        return controller.StatusCode(statusCode, ApiResponse.Error(errorCode, message, metadata));
    }
}

