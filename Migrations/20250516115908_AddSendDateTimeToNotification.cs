using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class AddSendDateTimeToNotification : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SendDateTime",
                table: "Notifications",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SendDateTime",
                table: "Notifications");
        }
    }
}
