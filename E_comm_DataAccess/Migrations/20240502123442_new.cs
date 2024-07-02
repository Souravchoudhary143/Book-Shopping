using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_comm_DataAccess.Migrations
{
    public partial class @new : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ImageUrl",
                table: "Products",
                newName: "ImageURL");

            migrationBuilder.RenameColumn(
                name: "CoverTypeId",
                table: "Products",
                newName: "CTID");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products",
                newName: "IX_Products_CTID");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CTID",
                table: "Products",
                column: "CTID",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CTID",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "ImageURL",
                table: "Products",
                newName: "ImageUrl");

            migrationBuilder.RenameColumn(
                name: "CTID",
                table: "Products",
                newName: "CoverTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_CTID",
                table: "Products",
                newName: "IX_Products_CoverTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
