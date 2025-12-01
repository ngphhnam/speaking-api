namespace SpeakingPractice.Api.Infrastructure.Clients;

public interface IMinioClientWrapper
{
    Task<string> UploadAudioAsync(Stream fileStream, string objectName, CancellationToken ct);
    Task<string> UploadAudioAsync(Stream fileStream, string objectName, Guid userId, CancellationToken ct);
    Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct);
}

