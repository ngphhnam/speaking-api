using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddMockTestEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mock_tests",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    part1question_ids = table.Column<string>(type: "text", nullable: false),
                    part2question_ids = table.Column<string>(type: "text", nullable: false),
                    part3question_ids = table.Column<string>(type: "text", nullable: false),
                    part1completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    part2completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    part3completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    overall_score = table.Column<decimal>(type: "numeric", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_mock_tests", x => x.id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "mock_tests");
        }
    }
}
