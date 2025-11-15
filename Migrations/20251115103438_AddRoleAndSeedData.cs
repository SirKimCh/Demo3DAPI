using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Demo3DAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleAndSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RoleID",
                table: "PlayerAccounts",
                type: "int",
                nullable: false,
                defaultValue: 2);

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.ID);
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "ID", "Name" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "User" }
                });

            migrationBuilder.InsertData(
                table: "PlayerAccounts",
                columns: new[] { "ID", "FullName", "Password", "PhoneNumber", "RoleID", "UserName" },
                values: new object[] { 1, "Admin", "$2a$11$eBFOGWYh8ztLXOf/8Dae3O7xruSaZdu9JFzuzeIIST5boNz/lkZ9S", null, 1, "admin" });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerAccounts_RoleID",
                table: "PlayerAccounts",
                column: "RoleID");

            migrationBuilder.AddForeignKey(
                name: "FK_PlayerAccounts_Roles_RoleID",
                table: "PlayerAccounts",
                column: "RoleID",
                principalTable: "Roles",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlayerAccounts_Roles_RoleID",
                table: "PlayerAccounts");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_PlayerAccounts_RoleID",
                table: "PlayerAccounts");

            migrationBuilder.DeleteData(
                table: "PlayerAccounts",
                keyColumn: "ID",
                keyValue: 1);

            migrationBuilder.DropColumn(
                name: "RoleID",
                table: "PlayerAccounts");
        }
    }
}
