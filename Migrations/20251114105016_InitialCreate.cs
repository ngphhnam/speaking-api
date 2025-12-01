using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "roles",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    full_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    avatar_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    date_of_birth = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    target_band_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    current_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    exam_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    subscription_type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "free"),
                    subscription_expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    email_verified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    last_login_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "topics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    title = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    slug = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    part_number = table.Column<int>(type: "integer", nullable: true),
                    difficulty_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    topic_category = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    keywords = table.Column<string[]>(type: "text[]", nullable: true),
                    usage_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    avg_user_rating = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_by = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_topics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claims_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claims_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_logins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_logins_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_roles",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_user_roles_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_roles_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_tokens",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "practice_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    part_number = table.Column<int>(type: "integer", nullable: true),
                    topic_id = table.Column<Guid>(type: "uuid", nullable: true),
                    questions_attempted = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_duration_seconds = table.Column<int>(type: "integer", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "in_progress"),
                    overall_band_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    fluency_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    vocabulary_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    grammar_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    pronunciation_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    device_info = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_practice_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_practice_sessions_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_practice_sessions_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "questions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    topic_id = table.Column<Guid>(type: "uuid", nullable: true),
                    question_text = table.Column<string>(type: "text", nullable: false),
                    question_type = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    suggested_structure = table.Column<string>(type: "jsonb", nullable: true),
                    sample_answers = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    key_vocabulary = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    estimated_band_requirement = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    time_limit_seconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 120),
                    attempts_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    avg_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_questions", x => x.id);
                    table.ForeignKey(
                        name: "fk_questions_topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "recordings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: true),
                    audio_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    audio_format = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    file_size_bytes = table.Column<long>(type: "bigint", nullable: true),
                    duration_seconds = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    transcription_text = table.Column<string>(type: "text", nullable: true),
                    transcription_confidence = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    transcription_language = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false, defaultValue: "en"),
                    word_timestamps = table.Column<string>(type: "jsonb", nullable: true),
                    processing_status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    error_message = table.Column<string>(type: "text", nullable: true),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recordings", x => x.id);
                    table.ForeignKey(
                        name: "fk_recordings_practice_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "practice_sessions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_recordings_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_recordings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "analysis_results",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recording_id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    overall_band_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    fluency_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    vocabulary_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    grammar_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    pronunciation_score = table.Column<decimal>(type: "numeric(2,1)", precision: 2, scale: 1, nullable: true),
                    metrics = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "{}"),
                    feedback_summary = table.Column<string>(type: "text", nullable: true),
                    strengths = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    improvements = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    grammar_issues = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    pronunciation_issues = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    vocabulary_suggestions = table.Column<string[]>(type: "jsonb[]", nullable: true),
                    whisper_model_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    llama_model_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    analysis_engine_version = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    analyzed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_analysis_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_analysis_results_recordings_recording_id",
                        column: x => x.recording_id,
                        principalTable: "recordings",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_analysis_results_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_analysis_results_recording_id",
                table: "analysis_results",
                column: "recording_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_analysis_results_user_id_analyzed_at",
                table: "analysis_results",
                columns: new[] { "user_id", "analyzed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_role_claims_role_id",
                table: "role_claims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claims_user_id",
                table: "user_claims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_logins_user_id",
                table: "user_logins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_roles_role_id",
                table: "user_roles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_users_subscription_type_subscription_expires_at",
                table: "users",
                columns: new[] { "subscription_type", "subscription_expires_at" },
                filter: "\"is_active\" = true");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_practice_sessions_status_user_id",
                table: "practice_sessions",
                columns: new[] { "status", "user_id" });

            migrationBuilder.CreateIndex(
                name: "ix_practice_sessions_topic_id",
                table: "practice_sessions",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "ix_practice_sessions_user_id_created_at",
                table: "practice_sessions",
                columns: new[] { "user_id", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_questions_topic_id",
                table: "questions",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "ix_recordings_processing_status",
                table: "recordings",
                column: "processing_status",
                filter: "\"processing_status\" != 'completed'");

            migrationBuilder.CreateIndex(
                name: "ix_recordings_question_id",
                table: "recordings",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_recordings_session_id",
                table: "recordings",
                column: "session_id");

            migrationBuilder.CreateIndex(
                name: "ix_recordings_user_id_recorded_at",
                table: "recordings",
                columns: new[] { "user_id", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id_token",
                table: "refresh_tokens",
                columns: new[] { "user_id", "token" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_topics_part_number",
                table: "topics",
                column: "part_number",
                filter: "\"is_active\" = true");

            migrationBuilder.CreateIndex(
                name: "ix_topics_slug",
                table: "topics",
                column: "slug",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "analysis_results");

            migrationBuilder.DropTable(
                name: "role_claims");

            migrationBuilder.DropTable(
                name: "user_claims");

            migrationBuilder.DropTable(
                name: "user_logins");

            migrationBuilder.DropTable(
                name: "user_roles");

            migrationBuilder.DropTable(
                name: "user_tokens");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "recordings");

            migrationBuilder.DropTable(
                name: "roles");

            migrationBuilder.DropTable(
                name: "practice_sessions");

            migrationBuilder.DropTable(
                name: "questions");

            migrationBuilder.DropTable(
                name: "users");

            migrationBuilder.DropTable(
                name: "topics");
        }
    }
}
