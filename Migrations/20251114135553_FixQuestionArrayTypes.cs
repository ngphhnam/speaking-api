using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class FixQuestionArrayTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "sample_answers",
                table: "questions",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "jsonb[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "key_vocabulary",
                table: "questions",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "jsonb[]",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string[]>(
                name: "sample_answers",
                table: "questions",
                type: "jsonb[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "key_vocabulary",
                table: "questions",
                type: "jsonb[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);
        }
    }
}
