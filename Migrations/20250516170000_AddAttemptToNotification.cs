using Microsoft.EntityFrameworkCore.Migrations;

namespace Zion.Reminder.Migrations;

public partial class AddAttemptToNotification : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "Attempt",
            table: "Notifications",
            type: "integer",
            nullable: false,
            defaultValue: 0);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Attempt",
            table: "Notifications");
    }
}
