using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class UserProgressConfiguration : IEntityTypeConfiguration<UserProgress>
{
    public void Configure(EntityTypeBuilder<UserProgress> builder)
    {
        builder.ToTable("user_progress");

        builder.HasKey(up => up.Id);
        builder.Property(up => up.Id).HasColumnName("id");

        builder.Property(up => up.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(up => up.PeriodType).HasColumnName("period_type").HasMaxLength(20).IsRequired();
        builder.Property(up => up.PeriodStart).HasColumnName("period_start").IsRequired();
        builder.Property(up => up.PeriodEnd).HasColumnName("period_end").IsRequired();

        builder.Property(up => up.TotalSessions).HasColumnName("total_sessions").HasDefaultValue(0);
        builder.Property(up => up.TotalRecordings).HasColumnName("total_recordings").HasDefaultValue(0);
        builder.Property(up => up.TotalPracticeMinutes).HasColumnName("total_practice_minutes").HasDefaultValue(0);

        builder.Property(up => up.AvgOverallScore).HasColumnName("avg_overall_score").HasPrecision(2, 1);
        builder.Property(up => up.AvgFluencyScore).HasColumnName("avg_fluency_score").HasPrecision(2, 1);
        builder.Property(up => up.AvgVocabularyScore).HasColumnName("avg_vocabulary_score").HasPrecision(2, 1);
        builder.Property(up => up.AvgGrammarScore).HasColumnName("avg_grammar_score").HasPrecision(2, 1);
        builder.Property(up => up.AvgPronunciationScore).HasColumnName("avg_pronunciation_score").HasPrecision(2, 1);

        builder.Property(up => up.ScoreImprovement).HasColumnName("score_improvement").HasPrecision(2, 1);
        builder.Property(up => up.ConsistencyScore).HasColumnName("consistency_score").HasPrecision(3, 2);

        builder.Property(up => up.TopicsPracticed).HasColumnName("topics_practiced").HasColumnType("jsonb");
        builder.Property(up => up.WeakestAreas).HasColumnName("weakest_areas").HasColumnType("jsonb[]");
        builder.Property(up => up.StrongestAreas).HasColumnName("strongest_areas").HasColumnType("jsonb[]");

        builder.Property(up => up.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(up => up.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(up => new { up.UserId, up.PeriodStart }).HasDatabaseName("idx_progress_user_period");
        builder.HasIndex(up => new { up.PeriodType, up.PeriodStart }).HasDatabaseName("idx_progress_period");

        builder.HasOne(up => up.User)
            .WithMany()
            .HasForeignKey(up => up.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasAlternateKey(up => new { up.UserId, up.PeriodType, up.PeriodStart });
    }
}

