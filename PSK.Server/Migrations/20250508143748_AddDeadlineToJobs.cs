using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddDeadlineToJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Deadline",
                table: "Jobs",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Deadline",
                table: "Jobs");
        }
    }
}
