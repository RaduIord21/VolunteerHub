using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerHub.DataModels.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "project_ownerid_foreign",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "projecttask_assigneeid_foreign",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "userstats_userid_foreign",
                table: "UserStats");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_AssigneeId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_Project_OwnerId",
                table: "Project");

            migrationBuilder.AlterColumn<string>(
                name: "AssigneeId",
                table: "ProjectTask",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId1",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "id",
                table: "Organization",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_UserId",
                table: "ProjectTask",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_UserId1",
                table: "Project",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_UserId1",
                table: "Project",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_AspNetUsers_UserId",
                table: "ProjectTask",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserStats_AspNetUsers_UserId",
                table: "UserStats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_UserId1",
                table: "Project");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTask_AspNetUsers_UserId",
                table: "ProjectTask");

            migrationBuilder.DropForeignKey(
                name: "FK_UserStats_AspNetUsers_UserId",
                table: "UserStats");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_UserId",
                table: "ProjectTask");

            migrationBuilder.DropIndex(
                name: "IX_Project_UserId1",
                table: "Project");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "Project");

            migrationBuilder.AlterColumn<string>(
                name: "AssigneeId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<long>(
                name: "id",
                table: "Organization",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<long>(
                name: "OrganizationId",
                table: "AspNetUsers",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_AssigneeId",
                table: "ProjectTask",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OwnerId",
                table: "Project",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "project_ownerid_foreign",
                table: "Project",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "projecttask_assigneeid_foreign",
                table: "ProjectTask",
                column: "AssigneeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "userstats_userid_foreign",
                table: "UserStats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
