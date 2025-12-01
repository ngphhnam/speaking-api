using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class AnalysisResultConfiguration : IEntityTypeConfiguration<AnalysisResult>
{
    public void Configure(EntityTypeBuilder<AnalysisResult> builder)
    {
        builder.ToTable("analysis_results");

        builder.HasKey(ar => ar.Id);
        builder.Property(ar => ar.Id).HasColumnName("id");

        builder.Property(ar => ar.RecordingId).HasColumnName("recording_id").IsRequired();
        builder.Property(ar => ar.UserId).HasColumnName("user_id").IsRequired();
        builder.Property(ar => ar.OverallBandScore).HasColumnName("overall_band_score").HasPrecision(2, 1);
        builder.Property(ar => ar.FluencyScore).HasColumnName("fluency_score").HasPrecision(2, 1);
        builder.Property(ar => ar.VocabularyScore).HasColumnName("vocabulary_score").HasPrecision(2, 1);
        builder.Property(ar => ar.GrammarScore).HasColumnName("grammar_score").HasPrecision(2, 1);
        builder.Property(ar => ar.PronunciationScore).HasColumnName("pronunciation_score").HasPrecision(2, 1);
        builder.Property(ar => ar.Metrics).HasColumnName("metrics").HasColumnType("jsonb").IsRequired().HasDefaultValue("{}");
        builder.Property(ar => ar.FeedbackSummary).HasColumnName("feedback_summary");
        builder.Property(ar => ar.Strengths).HasColumnName("strengths").HasColumnType("jsonb[]");
        builder.Property(ar => ar.Improvements).HasColumnName("improvements").HasColumnType("jsonb[]");
        builder.Property(ar => ar.GrammarIssues).HasColumnName("grammar_issues").HasColumnType("jsonb[]");
        builder.Property(ar => ar.PronunciationIssues).HasColumnName("pronunciation_issues").HasColumnType("jsonb[]");
        builder.Property(ar => ar.VocabularySuggestions).HasColumnName("vocabulary_suggestions").HasColumnType("jsonb[]");
        builder.Property(ar => ar.WhisperModelVersion).HasColumnName("whisper_model_version").HasMaxLength(50);
        builder.Property(ar => ar.LlamaModelVersion).HasColumnName("llama_model_version").HasMaxLength(50);
        builder.Property(ar => ar.AnalysisEngineVersion).HasColumnName("analysis_engine_version").HasMaxLength(50);
        builder.Property(ar => ar.AnalyzedAt).HasColumnName("analyzed_at").IsRequired();
        builder.Property(ar => ar.CreatedAt).HasColumnName("created_at").IsRequired();

        builder.HasIndex(ar => ar.RecordingId);
        builder.HasIndex(ar => new { ar.UserId, ar.AnalyzedAt });

        builder.HasOne(ar => ar.Recording)
            .WithOne(r => r.AnalysisResult)
            .HasForeignKey<AnalysisResult>(ar => ar.RecordingId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ar => ar.User)
            .WithMany(u => u.AnalysisResults)
            .HasForeignKey(ar => ar.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

