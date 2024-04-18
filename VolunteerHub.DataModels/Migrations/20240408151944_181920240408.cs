using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VolunteerHub.DataModels.Migrations
{
    /// <inheritdoc />
    public partial class _181920240408 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
       

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationId",
                table: "Project",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Project_Organization_OrganizationId",
                table: "Project",
                column: "OrganizationId",
                principalTable: "Organization",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Project_Organization_OrganizationId",
                table: "Project");

            migrationBuilder.DropIndex(
                name: "IX_Project_OrganizationId",
                table: "Project");
        }
    }
}
