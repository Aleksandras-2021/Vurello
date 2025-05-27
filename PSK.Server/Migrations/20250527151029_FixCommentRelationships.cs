using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixCommentRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserComment_AspNetUsers_CreatorId",
                table: "UserComment");

            migrationBuilder.DropForeignKey(
                name: "FK_UserComment_Jobs_JobId",
                table: "UserComment");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserComment",
                table: "UserComment");

            migrationBuilder.RenameTable(
                name: "UserComment",
                newName: "Comments");

            migrationBuilder.RenameIndex(
                name: "IX_UserComment_JobId",
                table: "Comments",
                newName: "IX_Comments_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_UserComment_CreatorId",
                table: "Comments",
                newName: "IX_Comments_CreatorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Comments",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comments",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_CreatorId",
                table: "Comments",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_Jobs_JobId",
                table: "Comments",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_CreatorId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Jobs_JobId",
                table: "Comments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Comments",
                table: "Comments");

            migrationBuilder.RenameTable(
                name: "Comments",
                newName: "UserComment");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_JobId",
                table: "UserComment",
                newName: "IX_UserComment_JobId");

            migrationBuilder.RenameIndex(
                name: "IX_Comments_CreatorId",
                table: "UserComment",
                newName: "IX_UserComment_CreatorId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "UserComment",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserComment",
                table: "UserComment",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserComment_AspNetUsers_CreatorId",
                table: "UserComment",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserComment_Jobs_JobId",
                table: "UserComment",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
