using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddBioToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "bio",
                table: "users",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "bio",
                table: "users");
        }
    }
}
