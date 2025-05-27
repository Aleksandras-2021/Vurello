using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class ReSeedPermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow creating, editing, deleting jobs", "Job" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow creating, editing, deleting board", "Board" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow creating, editing, deleting labels", "Labels" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow creating, editing, deleting roles and assign roles to users", "Roles" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Allow inviting and removing users from team", "TeamUsers" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Can create new jobs", "JobCreate" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000002"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Can update jobs", "JobUpdate" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000003"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Can delete jobs", "JobDelete" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000004"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Can assign labels to job", "JobLabelAssign" });

            migrationBuilder.UpdateData(
                table: "Permission",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000005"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Can create new board", "BoardCreate" });

            migrationBuilder.InsertData(
                table: "Permission",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
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
    }
}
