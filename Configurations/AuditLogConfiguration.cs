using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(al => al.Id);
        builder.Property(al => al.Id).HasColumnName("id");

        builder.Property(al => al.UserId).HasColumnName("user_id");
        builder.Property(al => al.Action).HasColumnName("action").HasMaxLength(100).IsRequired();
        builder.Property(al => al.EntityType).HasColumnName("entity_type").HasMaxLength(50);
        builder.Property(al => al.EntityId).HasColumnName("entity_id");
        builder.Property(al => al.OldValues).HasColumnName("old_values").HasColumnType("jsonb");
        builder.Property(al => al.NewValues).HasColumnName("new_values").HasColumnType("jsonb");
        builder.Property(al => al.IpAddress).HasColumnName("ip_address");
        builder.Property(al => al.UserAgent).HasColumnName("user_agent");
        builder.Property(al => al.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(al => new { al.UserId, al.CreatedAt }).HasDatabaseName("idx_audit_user");
        builder.HasIndex(al => new { al.Action, al.CreatedAt }).HasDatabaseName("idx_audit_action");

        builder.HasOne(al => al.User)
            .WithMany()
            .HasForeignKey(al => al.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

