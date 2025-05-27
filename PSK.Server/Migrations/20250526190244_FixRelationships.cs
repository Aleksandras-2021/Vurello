using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PSK.Server.Migrations
{
    /// <inheritdoc />
    public partial class FixRelationships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Role_RolesId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Role_Teams_TeamId",
                table: "Role");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRole_AspNetUsers_UserId",
                table: "UserTeamRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRole_Role_RoleId",
                table: "UserTeamRole");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRole_Teams_TeamId",
                table: "UserTeamRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTeamRole",
                table: "UserTeamRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Role",
                table: "Role");

            migrationBuilder.RenameTable(
                name: "UserTeamRole",
                newName: "UserTeamRoles");

            migrationBuilder.RenameTable(
                name: "Role",
                newName: "Roles");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeamRole_TeamId",
                table: "UserTeamRoles",
                newName: "IX_UserTeamRoles_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeamRole_RoleId",
                table: "UserTeamRoles",
                newName: "IX_UserTeamRoles_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Role_TeamId",
                table: "Roles",
                newName: "IX_Roles_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTeamRoles",
                table: "UserTeamRoles",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Roles",
                table: "Roles",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Roles_RolesId",
                table: "PermissionRole",
                column: "RolesId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Teams_TeamId",
                table: "Roles",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRoles_AspNetUsers_UserId",
                table: "UserTeamRoles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRoles_Roles_RoleId",
                table: "UserTeamRoles",
                column: "RoleId",
                principalTable: "Roles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRoles_Teams_TeamId",
                table: "UserTeamRoles",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PermissionRole_Roles_RolesId",
                table: "PermissionRole");

            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Teams_TeamId",
                table: "Roles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRoles_AspNetUsers_UserId",
                table: "UserTeamRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRoles_Roles_RoleId",
                table: "UserTeamRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserTeamRoles_Teams_TeamId",
                table: "UserTeamRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserTeamRoles",
                table: "UserTeamRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Roles",
                table: "Roles");

            migrationBuilder.RenameTable(
                name: "UserTeamRoles",
                newName: "UserTeamRole");

            migrationBuilder.RenameTable(
                name: "Roles",
                newName: "Role");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeamRoles_TeamId",
                table: "UserTeamRole",
                newName: "IX_UserTeamRole_TeamId");

            migrationBuilder.RenameIndex(
                name: "IX_UserTeamRoles_RoleId",
                table: "UserTeamRole",
                newName: "IX_UserTeamRole_RoleId");

            migrationBuilder.RenameIndex(
                name: "IX_Roles_TeamId",
                table: "Role",
                newName: "IX_Role_TeamId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserTeamRole",
                table: "UserTeamRole",
                columns: new[] { "UserId", "TeamId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Role",
                table: "Role",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Role_RolesId",
                table: "PermissionRole",
                column: "RolesId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Role_Teams_TeamId",
                table: "Role",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRole_AspNetUsers_UserId",
                table: "UserTeamRole",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRole_Role_RoleId",
                table: "UserTeamRole",
                column: "RoleId",
                principalTable: "Role",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserTeamRole_Teams_TeamId",
                table: "UserTeamRole",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
