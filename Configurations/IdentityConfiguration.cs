using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SpeakingPractice.Api.Domain.Entities;

namespace SpeakingPractice.Api.Configurations;

public static class IdentityConfiguration
{
    public static void ConfigureIdentityTables(ModelBuilder builder)
    {
        // Rename Identity tables to snake_case
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.ToTable("users");
        });

        builder.Entity<IdentityRole<Guid>>(entity =>
        {
            entity.ToTable("roles");
        });

        builder.Entity<IdentityUserRole<Guid>>(entity =>
        {
            entity.ToTable("user_roles");
        });

        builder.Entity<IdentityUserClaim<Guid>>(entity =>
        {
            entity.ToTable("user_claims");
        });

        builder.Entity<IdentityUserLogin<Guid>>(entity =>
        {
            entity.ToTable("user_logins");
        });

        builder.Entity<IdentityUserToken<Guid>>(entity =>
        {
            entity.ToTable("user_tokens");
        });

        builder.Entity<IdentityRoleClaim<Guid>>(entity =>
        {
            entity.ToTable("role_claims");
        });
    }
}

