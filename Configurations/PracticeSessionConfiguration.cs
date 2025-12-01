using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class PracticeSessionConfiguration : IEntityTypeConfiguration<PracticeSession>
{
    public void Configure(EntityTypeBuilder<PracticeSession> builder)
    {
        builder.ToTable("practice_sessions");

        builder.HasKey(ps => ps.Id);
        builder.Property(ps => ps.Id).HasColumnName("id");

        builder.Property(ps => ps.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(ps => ps.SessionType).HasColumnName("session_type").HasMaxLength(50);
        builder.Property(ps => ps.PartNumber).HasColumnName("part_number");
        builder.Property(ps => ps.TopicId).HasColumnName("topic_id");
        builder.Property(ps => ps.QuestionsAttempted).HasColumnName("questions_attempted").HasDefaultValue(0);
        builder.Property(ps => ps.TotalDurationSeconds).HasColumnName("total_duration_seconds");
        builder.Property(ps => ps.StartedAt).HasColumnName("started_at").IsRequired();
        builder.Property(ps => ps.CompletedAt).HasColumnName("completed_at");
        builder.Property(ps => ps.Status).HasColumnName("status").HasMaxLength(20).HasDefaultValue("in_progress");
        builder.Property(ps => ps.OverallBandScore).HasColumnName("overall_band_score").HasPrecision(2, 1);
        builder.Property(ps => ps.FluencyScore).HasColumnName("fluency_score").HasPrecision(2, 1);
        builder.Property(ps => ps.VocabularyScore).HasColumnName("vocabulary_score").HasPrecision(2, 1);
        builder.Property(ps => ps.GrammarScore).HasColumnName("grammar_score").HasPrecision(2, 1);
        builder.Property(ps => ps.PronunciationScore).HasColumnName("pronunciation_score").HasPrecision(2, 1);
        builder.Property(ps => ps.DeviceInfo).HasColumnName("device_info").HasColumnType("jsonb");
        builder.Property(ps => ps.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(ps => ps.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(ps => new { ps.UserId, ps.CreatedAt });
        builder.HasIndex(ps => new { ps.Status, ps.UserId });

        builder.HasOne(ps => ps.User)
            .WithMany(u => u.PracticeSessions)
            .HasForeignKey(ps => ps.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ps => ps.Topic)
            .WithMany(t => t.PracticeSessions)
            .HasForeignKey(ps => ps.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

