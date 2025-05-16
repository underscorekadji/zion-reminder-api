using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsAndNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    From = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    To = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    For = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    FromName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ToName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    ForName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EventId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    Channel = table.Column<int>(type: "integer", nullable: false),
                    ChannelAddress = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Reminders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2025, 5, 16, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(6900), new DateTime(2025, 5, 23, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(7596) });

            migrationBuilder.UpdateData(
                table: "Reminders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2025, 5, 16, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(8129), new DateTime(2025, 5, 19, 10, 31, 19, 284, DateTimeKind.Utc).AddTicks(8131) });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_EventId",
                table: "Notifications",
                column: "EventId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.UpdateData(
                table: "Reminders",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2025, 5, 16, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(1604), new DateTime(2025, 5, 23, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2043) });

            migrationBuilder.UpdateData(
                table: "Reminders",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "CreatedAt", "DueDate" },
                values: new object[] { new DateTime(2025, 5, 16, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2352), new DateTime(2025, 5, 19, 9, 19, 58, 309, DateTimeKind.Utc).AddTicks(2353) });
        }
    }
}
