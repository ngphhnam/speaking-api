using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddStreakToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "current_streak",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateOnly>(
                name: "last_practice_date",
                table: "users",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "longest_streak",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "total_practice_days",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "idx_users_current_streak",
                table: "users",
                column: "current_streak",
                descending: new bool[0],
                filter: "\"is_active\" = true AND \"current_streak\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_users_current_streak",
                table: "users");

            migrationBuilder.DropColumn(
                name: "current_streak",
                table: "users");

            migrationBuilder.DropColumn(
                name: "last_practice_date",
                table: "users");

            migrationBuilder.DropColumn(
                name: "longest_streak",
                table: "users");

            migrationBuilder.DropColumn(
                name: "total_practice_days",
                table: "users");
        }
    }
}
