using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_comm_DataAccess.Migrations
{
    public partial class newProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CTID",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CTID",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CTID",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "coverTypeId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_coverTypeId",
                table: "Products",
                column: "coverTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_coverTypeId",
                table: "Products",
                column: "coverTypeId",
                principalTable: "CoverTypes",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_coverTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_coverTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "coverTypeId",
                table: "Products");

            migrationBuilder.AddColumn<int>(
                name: "CTID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CTID",
                table: "Products",
                column: "CTID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CTID",
                table: "Products",
                column: "CTID",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
