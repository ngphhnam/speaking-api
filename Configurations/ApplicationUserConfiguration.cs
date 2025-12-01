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
        builder.Property(u => u.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(u => u.UpdatedAt).HasColumnName("updated_at").IsRequired();

        // Indexes
        builder.HasIndex(u => new { u.SubscriptionType, u.SubscriptionExpiresAt })
            .HasFilter("\"is_active\" = true");
    }
}

