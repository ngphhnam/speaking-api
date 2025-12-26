using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class StreakHistoryConfiguration : IEntityTypeConfiguration<StreakHistory>
{
    public void Configure(EntityTypeBuilder<StreakHistory> builder)
    {
        builder.ToTable("streak_history");

        builder.HasKey(sh => sh.Id);
        builder.Property(sh => sh.Id).HasColumnName("id");

        builder.Property(sh => sh.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(sh => sh.StreakLength).HasColumnName("streak_length").IsRequired();
        builder.Property(sh => sh.StartDate).HasColumnName("start_date").IsRequired();
        builder.Property(sh => sh.EndDate).HasColumnName("end_date");
        builder.Property(sh => sh.IsActive).HasColumnName("is_active").HasDefaultValue(false);
        builder.Property(sh => sh.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(sh => sh.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(sh => sh.UserId).HasDatabaseName("idx_streak_history_user_id");
        builder.HasIndex(sh => new { sh.UserId, sh.IsActive }).HasDatabaseName("idx_streak_history_user_active");
        builder.HasIndex(sh => sh.StartDate).HasDatabaseName("idx_streak_history_start_date");

        builder.HasOne(sh => sh.User)
            .WithMany()
            .HasForeignKey(sh => sh.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

