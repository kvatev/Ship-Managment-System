using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShipManagement.Data.Migrations
{
    /// <inheritdoc />
    public partial class AlterTaskTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedToId",
                table: "Tasks");

            migrationBuilder.CreateTable(
                name: "UserViewModel",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Roles = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserViewModel", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserViewModel_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "UserViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserViewModel_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "UserViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserViewModel_AssignedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserViewModel_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "UserViewModel");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedById",
                table: "Tasks",
                column: "AssignedById",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_AspNetUsers_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
