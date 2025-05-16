using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class RemoveReminderModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    Priority = table.Column<int>(type: "integer", nullable: false),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reminders", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Reminders",
                columns: new[] { "Id", "CompletedAt", "CreatedAt", "Description", "DueDate", "IsCompleted", "Priority", "Title" },
                values: new object[,]
                {
                    { 1, null, new DateTime(2025, 5, 16, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(6900), "Finish the API documentation for the reminder service", new DateTime(2025, 5, 23, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(7596), false, 2, "Complete project documentation" },
                    { 2, null, new DateTime(2025, 5, 16, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(8129), "Discuss project progress and roadblocks", new DateTime(2025, 5, 19, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(8131), false, 1, "Weekly team meeting" }
                });
        }
    }
}
