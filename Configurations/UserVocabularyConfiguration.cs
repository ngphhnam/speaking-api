using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class UserVocabularyConfiguration : IEntityTypeConfiguration<UserVocabulary>
{
    public void Configure(EntityTypeBuilder<UserVocabulary> builder)
    {
        builder.ToTable("user_vocabulary");

        builder.HasKey(uv => uv.Id);
        builder.Property(uv => uv.Id).HasColumnName("id");

        builder.Property(uv => uv.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(uv => uv.VocabularyId).HasColumnName("vocabulary_id").IsRequired();
        builder.Property(uv => uv.LearningStatus).HasColumnName("learning_status").HasMaxLength(20).HasDefaultValue("learning");
        builder.Property(uv => uv.NextReviewAt).HasColumnName("next_review_at");
        builder.Property(uv => uv.ReviewCount).HasColumnName("review_count").HasDefaultValue(0);
        builder.Property(uv => uv.SuccessCount).HasColumnName("success_count").HasDefaultValue(0);
        builder.Property(uv => uv.PersonalNotes).HasColumnName("personal_notes");
        builder.Property(uv => uv.ExampleUsage).HasColumnName("example_usage");
        builder.Property(uv => uv.FirstEncounteredAt).HasColumnName("first_encountered_at").IsRequired();
        builder.Property(uv => uv.LastReviewedAt).HasColumnName("last_reviewed_at");
        builder.Property(uv => uv.MasteredAt).HasColumnName("mastered_at");
        builder.Property(uv => uv.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(uv => new { uv.UserId, uv.LearningStatus }).HasDatabaseName("idx_user_vocab_status");
        builder.HasIndex(uv => uv.NextReviewAt).HasDatabaseName("idx_user_vocab_review").HasFilter("\"learning_status\" IN ('learning', 'reviewing')");

        builder.HasOne(uv => uv.User)
            .WithMany()
            .HasForeignKey(uv => uv.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(uv => uv.Vocabulary)
            .WithMany(v => v.UserVocabularies)
            .HasForeignKey(uv => uv.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasAlternateKey(uv => new { uv.UserId, uv.VocabularyId });
    }
}

