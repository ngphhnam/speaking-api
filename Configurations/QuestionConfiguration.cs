using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("questions");

        builder.HasKey(q => q.Id);
        builder.Property(q => q.Id).HasColumnName("id");

        builder.Property(q => q.TopicId).HasColumnName("topic_id");
        builder.Property(q => q.QuestionText).HasColumnName("question_text").IsRequired();
        builder.Property(q => q.QuestionType).HasColumnName("question_type").HasMaxLength(50);
        builder.Property(q => q.SuggestedStructure).HasColumnName("suggested_structure").HasColumnType("jsonb");
        builder.Property(q => q.SampleAnswers)
            .HasColumnName("sample_answers")
            .HasColumnType("text[]");
        builder.Property(q => q.KeyVocabulary)
            .HasColumnName("key_vocabulary")
            .HasColumnType("text[]");
        builder.Property(q => q.EstimatedBandRequirement).HasColumnName("estimated_band_requirement").HasPrecision(3, 1);
        builder.Property(q => q.TimeLimitSeconds).HasColumnName("time_limit_seconds").HasDefaultValue(120);
        builder.Property(q => q.AttemptsCount).HasColumnName("attempts_count").HasDefaultValue(0);
        builder.Property(q => q.AvgScore).HasColumnName("avg_score").HasPrecision(3, 1);
        builder.Property(q => q.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(q => q.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(q => q.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne(q => q.Topic)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

