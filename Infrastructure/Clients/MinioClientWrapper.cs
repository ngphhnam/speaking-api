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
    private readonly HashSet<string> _verifiedBuckets = new();
    private readonly HashSet<string> _publicPolicyBuckets = new();

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
        // Use the shared audio bucket for all users
        var bucketName = _options.Bucket;
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

    public async Task<string> UploadImageAsync(Stream fileStream, string objectName, Guid userId, CancellationToken ct)
    {
        var bucketName = "avatars";
        await EnsureBucketExistsAsync(bucketName, ct);

        // Determine content type from file extension
        var contentType = objectName.ToLower() switch
        {
            var name when name.EndsWith(".jpg") || name.EndsWith(".jpeg") => "image/jpeg",
            var name when name.EndsWith(".png") => "image/png",
            var name when name.EndsWith(".gif") => "image/gif",
            var name when name.EndsWith(".webp") => "image/webp",
            _ => "image/jpeg"
        };

        var args = new PutObjectArgs()
            .WithBucket(bucketName)
            .WithObject(objectName)
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType(contentType);

        await minioClient.PutObjectAsync(args, ct);
        logger.LogInformation("Uploaded image {ObjectName} to MinIO bucket {Bucket} for user {UserId}", objectName, bucketName, userId);

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

        await EnsurePublicReadPolicyAsync(bucketName, ct);
        _verifiedBuckets.Add(bucketName);
    }

    private async Task EnsurePublicReadPolicyAsync(string bucketName, CancellationToken ct)
    {
        if (_publicPolicyBuckets.Contains(bucketName))
        {
            return;
        }

        // Allow public read on all objects in the bucket
        var policyJson = $$"""
        {
          "Version": "2012-10-17",
          "Statement": [
            {
              "Effect": "Allow",
              "Principal": { "AWS": ["*"] },
              "Action": [ "s3:GetObject" ],
              "Resource": [ "arn:aws:s3:::{{bucketName}}/*" ]
            }
          ]
        }
        """;

        await minioClient.SetPolicyAsync(new SetPolicyArgs()
            .WithBucket(bucketName)
            .WithPolicy(policyJson), ct);

        _publicPolicyBuckets.Add(bucketName);
        logger.LogInformation("Ensured public-read policy for MinIO bucket {Bucket}", bucketName);
    }
}

