using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SpeakingPractice.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddSubscriptionPlanColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "subscription_plan_code",
                table: "users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "subscription_plan_days",
                table: "users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "plan_code",
                table: "payments",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "plan_days",
                table: "payments",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "plan_price",
                table: "payments",
                type: "bigint",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "subscription_plan_code",
                table: "users");

            migrationBuilder.DropColumn(
                name: "subscription_plan_days",
                table: "users");

            migrationBuilder.DropColumn(
                name: "plan_code",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "plan_days",
                table: "payments");

            migrationBuilder.DropColumn(
                name: "plan_price",
                table: "payments");
        }
    }
}
