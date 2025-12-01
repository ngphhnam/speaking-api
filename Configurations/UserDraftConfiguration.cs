using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class UserDraftConfiguration : IEntityTypeConfiguration<UserDraft>
{
    public void Configure(EntityTypeBuilder<UserDraft> builder)
    {
        builder.ToTable("user_drafts");

        builder.HasKey(d => d.Id);
        builder.Property(d => d.Id).HasColumnName("id");

        builder.Property(d => d.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(d => d.QuestionId).HasColumnName("question_id").IsRequired();
        builder.Property(d => d.DraftContent).HasColumnName("draft_content").IsRequired();
        builder.Property(d => d.OutlineStructure).HasColumnName("outline_structure").HasColumnType("jsonb");
        builder.Property(d => d.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(d => d.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasOne(d => d.User)
            .WithMany()
            .HasForeignKey(d => d.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(d => d.Question)
            .WithMany()
            .HasForeignKey(d => d.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(d => new { d.UserId, d.QuestionId }).IsUnique();
    }
}

