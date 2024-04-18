using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerHub.DataModels.Migrations
{
    /// <inheritdoc />
    public partial class _040920241946 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                table: "Project");

            
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Project");

            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

      

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_AspNetUsers_OwnerId",
                table: "Project");

           
            migrationBuilder.AlterColumn<string>(
                name: "OwnerId",
                table: "Project",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Project",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Project_AspNetUsers_UserId",
                table: "Project",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
