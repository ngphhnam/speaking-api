using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SpeakingPractice.Api.Configurations;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Infrastructure.Persistence;

public class ApplicationDbContext(
    DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<PracticeSession> PracticeSessions => Set<PracticeSession>();
    public DbSet<Topic> Topics => Set<Topic>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<Recording> Recordings => Set<Recording>();
    public DbSet<AnalysisResult> AnalysisResults => Set<AnalysisResult>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserDraft> UserDrafts => Set<UserDraft>();
    
    // Progress tracking
    public DbSet<UserProgress> UserProgresses => Set<UserProgress>();
    public DbSet<Achievement> Achievements => Set<Achievement>();
    public DbSet<UserAchievement> UserAchievements => Set<UserAchievement>();
    
    // Vocabulary & learning
    public DbSet<Vocabulary> Vocabularies => Set<Vocabulary>();
    public DbSet<UserVocabulary> UserVocabularies => Set<UserVocabulary>();
    
    // System tables
    public DbSet<ApiUsageLog> ApiUsageLogs => Set<ApiUsageLog>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Rename Identity tables to snake_case
        IdentityConfiguration.ConfigureIdentityTables(builder);
        
        // Apply other entity configurations
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}

