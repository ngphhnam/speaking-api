using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class ApiUsageLogConfiguration : IEntityTypeConfiguration<ApiUsageLog>
{
    public void Configure(EntityTypeBuilder<ApiUsageLog> builder)
    {
        builder.ToTable("api_usage_logs");

        builder.HasKey(aul => aul.Id);
        builder.Property(aul => aul.Id).HasColumnName("id");

        builder.Property(aul => aul.UserId).HasColumnName("user_id");
        builder.Property(aul => aul.ServiceName).HasColumnName("service_name").HasMaxLength(50).IsRequired();
        builder.Property(aul => aul.Endpoint).HasColumnName("endpoint").HasMaxLength(255);
        builder.Property(aul => aul.RequestSizeBytes).HasColumnName("request_size_bytes");
        builder.Property(aul => aul.ResponseSizeBytes).HasColumnName("response_size_bytes");
        builder.Property(aul => aul.ProcessingTimeMs).HasColumnName("processing_time_ms");
        builder.Property(aul => aul.EstimatedCost).HasColumnName("estimated_cost").HasPrecision(10, 6);
        builder.Property(aul => aul.StatusCode).HasColumnName("status_code");
        builder.Property(aul => aul.ErrorMessage).HasColumnName("error_message");
        builder.Property(aul => aul.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(aul => new { aul.ServiceName, aul.CreatedAt }).HasDatabaseName("idx_api_logs_service");
        builder.HasIndex(aul => new { aul.UserId, aul.CreatedAt }).HasDatabaseName("idx_api_logs_user");

        builder.HasOne(aul => aul.User)
            .WithMany()
            .HasForeignKey(aul => aul.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

