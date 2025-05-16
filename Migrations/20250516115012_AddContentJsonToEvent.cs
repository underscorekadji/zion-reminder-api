using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class AddContentJsonToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ContentJson",
                table: "Events",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ContentJson",
                table: "Events");
        }
    }
}
