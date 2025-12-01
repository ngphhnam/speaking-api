using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class TopicConfiguration : IEntityTypeConfiguration<Topic>
{
    public void Configure(EntityTypeBuilder<Topic> builder)
    {
        builder.ToTable("topics");

        builder.HasKey(t => t.Id);
        builder.Property(t => t.Id).HasColumnName("id");

        builder.Property(t => t.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(t => t.Slug).HasColumnName("slug").HasMaxLength(255).IsRequired();
        builder.Property(t => t.Description).HasColumnName("description");
        builder.Property(t => t.PartNumber).HasColumnName("part_number");
        builder.Property(t => t.DifficultyLevel).HasColumnName("difficulty_level").HasMaxLength(20);
        builder.Property(t => t.TopicCategory).HasColumnName("topic_category").HasMaxLength(100);
        builder.Property(t => t.Keywords).HasColumnName("keywords").HasColumnType("text[]");
        builder.Property(t => t.UsageCount).HasColumnName("usage_count").HasDefaultValue(0);
        builder.Property(t => t.AvgUserRating).HasColumnName("avg_user_rating").HasPrecision(3, 2);
        builder.Property(t => t.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(t => t.CreatedBy).HasColumnName("created_by");
        builder.Property(t => t.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(t => t.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.PartNumber).HasFilter("\"is_active\" = true");

        builder.HasMany(t => t.Questions)
            .WithOne(q => q.Topic)
            .HasForeignKey(q => q.TopicId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(t => t.PracticeSessions)
            .WithOne(ps => ps.Topic)
            .HasForeignKey(ps => ps.TopicId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

