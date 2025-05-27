using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class SeedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Permission",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldDefaultValueSql: "gen_random_uuid()");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Permission",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000001"), "Can create new jobs", "JobCreate" },
                    { new Guid("00000000-0000-0000-0000-000000000002"), "Can update jobs", "JobUpdate" },
                    { new Guid("00000000-0000-0000-0000-000000000003"), "Can delete jobs", "JobDelete" },
                    { new Guid("00000000-0000-0000-0000-000000000004"), "Can assign labels to job", "JobLabelAssign" },
                    { new Guid("00000000-0000-0000-0000-000000000005"), "Can create new board", "BoardCreate" },
                    { new Guid("00000000-0000-0000-0000-000000000006"), "Can update boards", "BoardUpdate" },
                    { new Guid("00000000-0000-0000-0000-000000000007"), "Can delete boards", "BoardDelete" },
                    { new Guid("00000000-0000-0000-0000-000000000008"), "Can create new label", "LabelCreate" },
                    { new Guid("00000000-0000-0000-0000-000000000009"), "Can update labels", "LabelUpdate" },
                    { new Guid("00000000-0000-0000-0000-000000000010"), "Can delete labels", "LabelDelete" },
                    { new Guid("00000000-0000-0000-0000-000000000011"), "Can invite people to team", "InvitationCreate" },
                    { new Guid("00000000-0000-0000-0000-000000000012"), "Can remove people from team", "TeamRemove" },
                    { new Guid("00000000-0000-0000-0000-000000000013"), "Can create new role", "RoleCreate" },
                    { new Guid("00000000-0000-0000-0000-000000000014"), "Can update roles", "RoleUpdate" },
                    { new Guid("00000000-0000-0000-0000-000000000015"), "Can delete roles", "RoleDelete" },
                    { new Guid("00000000-0000-0000-0000-000000000016"), "Can assign roles", "RoleAssign" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000007"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000008"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000009"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000010"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000011"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000012"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000013"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000014"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000015"));

            migrationBuilder.DeleteData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000016"));

            migrationBuilder.DropColumn(
                name: "Description",
                table: "Permission");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "Permission",
                type: "uuid",
                nullable: false,
                defaultValueSql: "gen_random_uuid()",
                oldClrType: typeof(Guid),
                oldType: "uuid");
        }
    }
}
