using Minio;
using Minio.DataModel;
using Microsoft.Extensions.Options;
using SpeakingPractice.Api.Options;

namespace SpeakingPractice.Api.Infrastructure.Clients;

public class MinioClientWrapper(
    MinioClient minioClient,
    IOptions<MinioOptions> options,
    ILogger<MinioClientWrapper> logger) : IMinioClientWrapper
{
    private readonly MinioOptions _options = options.Value;
    private bool _bucketVerified;
    private readonly HashSet<string> _verifiedBuckets = new();

    public async Task<string> UploadAudioAsync(Stream fileStream, string objectName, CancellationToken ct)
    {
        // Default bucket for backward compatibility
        await EnsureBucketExistsAsync(_options.Bucket, ct);

        var args = new PutObjectArgs()
            .WithBucket(_options.Bucket)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("audio/wav");

        await minioClient.PutObjectAsync(args, ct);
        logger.LogInformation("Uploaded {ObjectName} to MinIO bucket {Bucket}", objectName, _options.Bucket);

        var endpoint = _options.Endpoint.TrimEnd('/');
        var url = $"{endpoint}/{_options.Bucket}/{objectName}";
        return url;
    }

    public async Task<string> UploadAudioAsync(Stream fileStream, string objectName, Guid userId, CancellationToken ct)
    {
        // Create bucket name based on user ID
        var bucketName = GetUserBucketName(userId);
        await EnsureBucketExistsAsync(bucketName, ct);

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("audio/wav");

        await minioClient.PutObjectAsync(args, ct);
        logger.LogInformation("Uploaded {ObjectName} to MinIO bucket {Bucket} for user {UserId}", objectName, bucketName, userId);

        var endpoint = _options.Endpoint.TrimEnd('/');
        var url = $"{endpoint}/{bucketName}/{objectName}";
        return url;
    }

    public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct)
    {
        // Check if bucket already verified in this session
        if (_verifiedBuckets.Contains(bucketName))
        {
            return;
        }

        var exists = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucketName), ct);
        if (!exists)
        {
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucketName), ct);
            logger.LogInformation("Created MinIO bucket {Bucket}", bucketName);
        }

        _verifiedBuckets.Add(bucketName);
    }

    private string GetUserBucketName(Guid userId)
    {
        // Format: speaking-audio-user-{userId}
        // MinIO bucket names must be lowercase and can contain hyphens
        return $"{_options.Bucket}-user-{userId.ToString().ToLowerInvariant()}";
    }
}

