using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Zion.Reminder.Migrations
{
    /// <inheritdoc />
    public partial class AddCorrelationIdToEvent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CorrelationId",
                table: "Events",
                type: "uuid",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrelationId",
                table: "Events");
        }
    }
}
