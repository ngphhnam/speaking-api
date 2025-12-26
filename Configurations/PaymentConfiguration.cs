using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.OrderCode)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(p => p.ClientReference)
            .HasMaxLength(128);

        builder.Property(p => p.PaymentLinkId)
            .HasMaxLength(128);

        builder.Property(p => p.PaymentId)
            .HasMaxLength(128);

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(16);

        builder.Property(p => p.Description)
            .IsRequired()
            .HasMaxLength(128);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(p => p.Provider)
            .IsRequired()
            .HasMaxLength(64);

        builder.Property(p => p.CheckoutUrl)
            .HasMaxLength(512);

        builder.Property(p => p.QrCode)
            .HasMaxLength(512);

        builder.Property(p => p.QrImageUrl)
            .HasMaxLength(512);

        builder.Property(p => p.ProviderResponse)
            .HasColumnType("text");

        builder.HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(p => p.OrderCode).IsUnique();
        builder.HasIndex(p => p.PaymentLinkId);
    }
}





