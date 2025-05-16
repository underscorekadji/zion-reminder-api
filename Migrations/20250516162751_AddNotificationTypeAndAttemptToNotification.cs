using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationTypeAndAttemptToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Attempt",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NotificationType",
                table: "Notifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attempt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "Notifications");
        }
    }
}
