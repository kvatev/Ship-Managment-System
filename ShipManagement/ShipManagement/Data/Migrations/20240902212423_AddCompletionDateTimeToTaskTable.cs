using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCompletionDateTimeToTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedDateTime",
                table: "Tasks",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedDateTime",
                table: "Tasks");
        }
    }
}
