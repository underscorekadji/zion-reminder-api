using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class RenamingModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Reminders",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsCompleted = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Priority = table.Column<int>(type: "integer", nullable: false)
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
                    { 1, null, new DateTime(2025, 5, 16, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(1604), "Finish the API documentation for the reminder service", new DateTime(2025, 5, 23, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2043), false, 2, "Complete project documentation" },
                    { 2, null, new DateTime(2025, 5, 16, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2352), "Discuss project progress and roadblocks", new DateTime(2025, 5, 19, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2353), false, 1, "Weekly team meeting" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Reminders");
        }
    }
}
