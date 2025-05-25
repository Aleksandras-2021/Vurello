using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class AddBoardColumnsSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ColumnId",
                table: "Jobs",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BoardColumn",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Color = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    BoardId = table.Column<Guid>(type: "uuid", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    IsDefault = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoardColumn", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoardColumn_Boards_BoardId",
                        column: x => x.BoardId,
                        principalTable: "Boards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ColumnId",
                table: "Jobs",
                column: "ColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_BoardColumn_BoardId",
                table: "BoardColumn",
                column: "BoardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_BoardColumn_ColumnId",
                table: "Jobs",
                column: "ColumnId",
                principalTable: "BoardColumn",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_BoardColumn_ColumnId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "BoardColumn");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ColumnId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ColumnId",
                table: "Jobs");
        }
    }
}
