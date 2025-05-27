using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixBoardRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardColumn_Boards_BoardId",
                table: "BoardColumn");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_BoardColumn_ColumnId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardColumn",
                table: "BoardColumn");

            migrationBuilder.RenameTable(
                name: "BoardColumn",
                newName: "BoardColumns");

            migrationBuilder.RenameIndex(
                name: "IX_BoardColumn_BoardId",
                table: "BoardColumns",
                newName: "IX_BoardColumns_BoardId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BoardColumns",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardColumns",
                table: "BoardColumns",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardColumns_Boards_BoardId",
                table: "BoardColumns",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_BoardColumns_ColumnId",
                table: "Jobs",
                column: "ColumnId",
                principalTable: "BoardColumns",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BoardColumns_Boards_BoardId",
                table: "BoardColumns");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_BoardColumns_ColumnId",
                table: "Jobs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BoardColumns",
                table: "BoardColumns");

            migrationBuilder.RenameTable(
                name: "BoardColumns",
                newName: "BoardColumn");

            migrationBuilder.RenameIndex(
                name: "IX_BoardColumns_BoardId",
                table: "BoardColumn",
                newName: "IX_BoardColumn_BoardId");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "BoardColumn",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BoardColumn",
                table: "BoardColumn",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BoardColumn_Boards_BoardId",
                table: "BoardColumn",
                column: "BoardId",
                principalTable: "Boards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_BoardColumn_ColumnId",
                table: "Jobs",
                column: "ColumnId",
                principalTable: "BoardColumn",
                principalColumn: "Id");
        }
    }
}
