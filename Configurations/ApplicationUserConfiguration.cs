using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Note: Most Identity properties are configured by base Identity configuration
        // We only configure additional custom properties here

        builder.Property(u => u.FullName).HasColumnName("full_name").HasMaxLength(255).IsRequired();
        builder.Property(u => u.Bio).HasColumnName("bio").HasMaxLength(1000);
        builder.Property(u => u.AvatarUrl).HasColumnName("avatar_url").HasMaxLength(500);
        builder.Property(u => u.Phone).HasColumnName("phone").HasMaxLength(20);
        builder.Property(u => u.DateOfBirth).HasColumnName("date_of_birth");
        builder.Property(u => u.TargetBandScore).HasColumnName("target_band_score").HasPrecision(2, 1);
        builder.Property(u => u.CurrentLevel).HasColumnName("current_level").HasMaxLength(20);
        builder.Property(u => u.ExamDate).HasColumnName("exam_date");
        builder.Property(u => u.SubscriptionType).HasColumnName("subscription_type").HasMaxLength(20).HasDefaultValue("free");
        builder.Property(u => u.SubscriptionExpiresAt).HasColumnName("subscription_expires_at");
        builder.Property(u => u.EmailVerified).HasColumnName("email_verified").HasDefaultValue(false);
        builder.Property(u => u.IsActive).HasColumnName("is_active").HasDefaultValue(true);
        builder.Property(u => u.LastLoginAt).HasColumnName("last_login_at");
        
        // Streak tracking
        builder.Property(u => u.CurrentStreak).HasColumnName("current_streak").HasDefaultValue(0);
        builder.Property(u => u.LongestStreak).HasColumnName("longest_streak").HasDefaultValue(0);
        builder.Property(u => u.LastPracticeDate).HasColumnName("last_practice_date");
        builder.Property(u => u.TotalPracticeDays).HasColumnName("total_practice_days").HasDefaultValue(0);
        
        // Level system
        builder.Property(u => u.Level).HasColumnName("level").HasDefaultValue(1);
        builder.Property(u => u.ExperiencePoints).HasColumnName("experience_points").HasDefaultValue(0);
        builder.Property(u => u.TotalPoints).HasColumnName("total_points").HasDefaultValue(0);
        
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(u => new { u.SubscriptionType, u.SubscriptionExpiresAt })
            .HasFilter("\"is_active\" = true");
        
        // Index for streak leaderboard (descending order, active users only)
        builder.HasIndex(u => u.CurrentStreak)
            .HasDatabaseName("idx_users_current_streak")
            .IsDescending()
            .HasFilter("\"is_active\" = true AND \"current_streak\" > 0");
    }
}

