using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class AssignMemberToJob : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssignedMemberId",
                table: "Jobs",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_AssignedMemberId",
                table: "Jobs",
                column: "AssignedMemberId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_AspNetUsers_AssignedMemberId",
                table: "Jobs",
                column: "AssignedMemberId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_AspNetUsers_AssignedMemberId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_AssignedMemberId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "AssignedMemberId",
                table: "Jobs");
        }
    }
}
