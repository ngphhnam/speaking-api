using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionStyleAndUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // First, convert existing data from old format to new format
            // Also set default for NULL values
            migrationBuilder.Sql(@"
                UPDATE questions 
                SET question_type = 'PART1' 
                WHERE question_type = 'Part 1' OR question_type = 'part 1' OR question_type = 'PART 1' OR question_type IS NULL;
                
                UPDATE questions 
                SET question_type = 'PART2' 
                WHERE question_type = 'Part 2' OR question_type = 'part 2' OR question_type = 'PART 2';
                
                UPDATE questions 
                SET question_type = 'PART3' 
                WHERE question_type = 'Part 3' OR question_type = 'part 3' OR question_type = 'PART 3';
                
                -- Also handle old format like 'personal', 'cue_card', 'discussion'
                UPDATE questions 
                SET question_type = 'PART1' 
                WHERE question_type = 'personal';
                
                UPDATE questions 
                SET question_type = 'PART2' 
                WHERE question_type = 'cue_card' OR question_type = 'cue card';
                
                UPDATE questions 
                SET question_type = 'PART3' 
                WHERE question_type = 'discussion';
            ");

            // Add question_style column first (nullable)
            migrationBuilder.AddColumn<string>(
                name: "question_style",
                table: "questions",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            // Set default for existing Part 2 questions
            migrationBuilder.Sql(@"
                UPDATE questions 
                SET question_style = 'CueCard' 
                WHERE question_type = 'PART2';
            ");

            // Now alter question_type column
            migrationBuilder.AlterColumn<string>(
                name: "question_type",
                table: "questions",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "PART1",
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Convert back to old format (if needed)
            migrationBuilder.Sql(@"
                UPDATE questions 
                SET question_type = 'Part 1' 
                WHERE question_type = 'PART1';
                
                UPDATE questions 
                SET question_type = 'Part 2' 
                WHERE question_type = 'PART2';
                
                UPDATE questions 
                SET question_type = 'Part 3' 
                WHERE question_type = 'PART3';
            ");

            migrationBuilder.DropColumn(
                name: "question_style",
                table: "questions");

            migrationBuilder.AlterColumn<string>(
                name: "question_type",
                table: "questions",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldDefaultValue: "PART1");
        }
    }
}
