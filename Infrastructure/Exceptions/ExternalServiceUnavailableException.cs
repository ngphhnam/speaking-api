namespace SpeakingPractice.Api.Infrastructure.Exceptions;

public class ExternalServiceUnavailableException(string serviceName, string? message = null, Exception? innerException = null)
    : Exception(message ?? $"{serviceName} is currently unavailable. Please try again later.", innerException)
{
    public string ServiceName { get; } = serviceName;
}

