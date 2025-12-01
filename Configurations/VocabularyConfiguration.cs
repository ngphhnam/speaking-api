using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public class VocabularyConfiguration : IEntityTypeConfiguration<Vocabulary>
{
    public void Configure(EntityTypeBuilder<Vocabulary> builder)
    {
        builder.ToTable("vocabulary");

        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).HasColumnName("id");

        builder.Property(v => v.Word).HasColumnName("word").HasMaxLength(255).IsRequired();
        builder.Property(v => v.Phonetic).HasColumnName("phonetic").HasMaxLength(255);
        builder.Property(v => v.PartOfSpeech).HasColumnName("part_of_speech").HasMaxLength(50);
        builder.Property(v => v.DefinitionEn).HasColumnName("definition_en").IsRequired();
        builder.Property(v => v.DefinitionVi).HasColumnName("definition_vi");
        builder.Property(v => v.IeltsBandLevel).HasColumnName("ielts_band_level").HasPrecision(2, 1);
        builder.Property(v => v.TopicCategories).HasColumnName("topic_categories").HasColumnType("text[]");
        builder.Property(v => v.ExampleSentences).HasColumnName("example_sentences").HasColumnType("jsonb[]");
        builder.Property(v => v.Synonyms).HasColumnName("synonyms").HasColumnType("text[]");
        builder.Property(v => v.Antonyms).HasColumnName("antonyms").HasColumnType("text[]");
        builder.Property(v => v.Collocations).HasColumnName("collocations").HasColumnType("text[]");
        builder.Property(v => v.UsageFrequency).HasColumnName("usage_frequency").HasDefaultValue(0);
        builder.Property(v => v.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(v => v.UpdatedAt).HasColumnName("updated_at").IsRequired();

        builder.HasIndex(v => v.Word).IsUnique().HasDatabaseName("idx_vocabulary_word");
        builder.HasIndex(v => v.IeltsBandLevel).HasDatabaseName("idx_vocabulary_band");
        builder.HasIndex(v => v.TopicCategories).HasDatabaseName("idx_vocabulary_categories").HasMethod("gin");

        builder.HasMany(v => v.UserVocabularies)
            .WithOne(uv => uv.Vocabulary)
            .HasForeignKey(uv => uv.VocabularyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

