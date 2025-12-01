using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class UserAchievementConfiguration : IEntityTypeConfiguration<UserAchievement>
{
    public void Configure(EntityTypeBuilder<UserAchievement> builder)
    {
        builder.ToTable("user_achievements");

        builder.HasKey(ua => ua.Id);
        builder.Property(ua => ua.Id).HasColumnName("id");

        builder.Property(ua => ua.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(ua => ua.AchievementId).HasColumnName("achievement_id").IsRequired();
        builder.Property(ua => ua.Progress).HasColumnName("progress").HasColumnType("jsonb");
        builder.Property(ua => ua.IsCompleted).HasColumnName("is_completed").HasDefaultValue(false);
        builder.Property(ua => ua.EarnedAt).HasColumnName("earned_at");
        builder.Property(ua => ua.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(ua => new { ua.UserId, ua.IsCompleted }).HasDatabaseName("idx_user_achievements");

        builder.HasOne(ua => ua.User)
            .WithMany()
            .HasForeignKey(ua => ua.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ua => ua.Achievement)
            .WithMany(a => a.UserAchievements)
            .HasForeignKey(ua => ua.AchievementId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasAlternateKey(ua => new { ua.UserId, ua.AchievementId });
    }
}

