using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerHub.DataModels.Migrations
{
    /// <inheritdoc />
    public partial class _080520241607 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropIndex(
                name: "IX_ProjectTask_UserId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "ProjectTask");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ProjectTask");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "ProjectTask",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "ProjectTask",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTask_UserId",
                table: "ProjectTask",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTask_AspNetUsers_UserId",
                table: "ProjectTask",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
