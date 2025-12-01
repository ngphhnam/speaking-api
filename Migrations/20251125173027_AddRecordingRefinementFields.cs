using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddRecordingRefinementFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "refined_text",
                table: "recordings",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refinement_metadata",
                table: "recordings",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "estimated_band_requirement",
                table: "questions",
                type: "numeric(3,1)",
                precision: 3,
                scale: 1,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,1)",
                oldPrecision: 2,
                oldScale: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "avg_score",
                table: "questions",
                type: "numeric(3,1)",
                precision: 3,
                scale: 1,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(2,1)",
                oldPrecision: 2,
                oldScale: 1,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "comparison_analysis",
                table: "analysis_results",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "refinement_suggestions",
                table: "analysis_results",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "user_drafts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    question_id = table.Column<Guid>(type: "uuid", nullable: false),
                    draft_content = table.Column<string>(type: "text", nullable: false),
                    outline_structure = table.Column<string>(type: "jsonb", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_drafts", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_drafts_questions_question_id",
                        column: x => x.question_id,
                        principalTable: "questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_drafts_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_drafts_question_id",
                table: "user_drafts",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_drafts_user_id_question_id",
                table: "user_drafts",
                columns: new[] { "user_id", "question_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_drafts");

            migrationBuilder.DropColumn(
                name: "refined_text",
                table: "recordings");

            migrationBuilder.DropColumn(
                name: "refinement_metadata",
                table: "recordings");

            migrationBuilder.DropColumn(
                name: "comparison_analysis",
                table: "analysis_results");

            migrationBuilder.DropColumn(
                name: "refinement_suggestions",
                table: "analysis_results");

            migrationBuilder.AlterColumn<decimal>(
                name: "estimated_band_requirement",
                table: "questions",
                type: "numeric(2,1)",
                precision: 2,
                scale: 1,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,1)",
                oldPrecision: 3,
                oldScale: 1,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "avg_score",
                table: "questions",
                type: "numeric(2,1)",
                precision: 2,
                scale: 1,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(3,1)",
                oldPrecision: 3,
                oldScale: 1,
                oldNullable: true);
        }
    }
}
