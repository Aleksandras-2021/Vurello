using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamCreator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "Teams",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Teams_CreatorId",
                table: "Teams",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Teams_AspNetUsers_CreatorId",
                table: "Teams",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Teams_AspNetUsers_CreatorId",
                table: "Teams");

            migrationBuilder.DropIndex(
                name: "IX_Teams_CreatorId",
                table: "Teams");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Teams");
        }
    }
}
