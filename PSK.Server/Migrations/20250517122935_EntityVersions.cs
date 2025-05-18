using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class EntityVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Teams",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Labels",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Jobs",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "Boards",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Labels");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "Boards");
        }
    }
}
