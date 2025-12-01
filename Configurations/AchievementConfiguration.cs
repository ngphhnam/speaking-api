using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class AchievementConfiguration : IEntityTypeConfiguration<Achievement>
{
    public void Configure(EntityTypeBuilder<Achievement> builder)
    {
        builder.ToTable("achievements");

        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("id");

        builder.Property(a => a.Title).HasColumnName("title").HasMaxLength(255).IsRequired();
        builder.Property(a => a.Description).HasColumnName("description");
        builder.Property(a => a.AchievementType).HasColumnName("achievement_type").HasMaxLength(50).IsRequired();
        builder.Property(a => a.RequirementCriteria).HasColumnName("requirement_criteria").HasColumnType("jsonb").IsRequired().HasDefaultValue("{}");
        builder.Property(a => a.Points).HasColumnName("points").HasDefaultValue(0);
        builder.Property(a => a.BadgeIconUrl).HasColumnName("badge_icon_url").HasMaxLength(500);
        builder.Property(a => a.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(a => a.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(a => a.AchievementType).HasDatabaseName("idx_achievements_type").HasFilter("\"is_active\" = true");

        builder.HasMany(a => a.UserAchievements)
            .WithOne(ua => ua.Achievement)
            .HasForeignKey(ua => ua.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

