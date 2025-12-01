using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class RecordingConfiguration : IEntityTypeConfiguration<Recording>
{
    public void Configure(EntityTypeBuilder<Recording> builder)
    {
        builder.ToTable("recordings");

        builder.HasKey(r => r.Id);
        builder.Property(r => r.Id).HasColumnName("id");

        builder.Property(r => r.SessionId).HasColumnName("session_id").IsRequired();
        builder.Property(r => r.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(r => r.QuestionId).HasColumnName("question_id");
        builder.Property(r => r.AudioUrl).HasColumnName("audio_url").HasMaxLength(500).IsRequired();
        builder.Property(r => r.AudioFormat).HasColumnName("audio_format").HasMaxLength(10);
        builder.Property(r => r.FileSizeBytes).HasColumnName("file_size_bytes");
        builder.Property(r => r.DurationSeconds).HasColumnName("duration_seconds").HasPrecision(10, 2);
        builder.Property(r => r.TranscriptionText).HasColumnName("transcription_text");
        builder.Property(r => r.TranscriptionConfidence).HasColumnName("transcription_confidence").HasPrecision(3, 2);
        builder.Property(r => r.TranscriptionLanguage).HasColumnName("transcription_language").HasMaxLength(10).HasDefaultValue("en");
        builder.Property(r => r.WordTimestamps).HasColumnName("word_timestamps").HasColumnType("jsonb");
        builder.Property(r => r.ProcessingStatus).HasColumnName("processing_status").HasMaxLength(20).HasDefaultValue("pending");
        builder.Property(r => r.ErrorMessage).HasColumnName("error_message");
        builder.Property(r => r.RecordedAt).HasColumnName("recorded_at").IsRequired();
        builder.Property(r => r.ProcessedAt).HasColumnName("processed_at");
        builder.Property(r => r.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(r => r.SessionId);
        builder.HasIndex(r => new { r.UserId, r.RecordedAt });
        builder.HasIndex(r => r.ProcessingStatus).HasFilter("\"processing_status\" != 'completed'");

        builder.HasOne(r => r.Session)
            .WithMany(ps => ps.Recordings)
            .HasForeignKey(r => r.SessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.User)
            .WithMany(u => u.Recordings)
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(r => r.Question)
            .WithMany(q => q.Recordings)
            .HasForeignKey(r => r.QuestionId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(r => r.AnalysisResult)
            .WithOne(ar => ar.Recording)
            .HasForeignKey<AnalysisResult>(ar => ar.RecordingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

