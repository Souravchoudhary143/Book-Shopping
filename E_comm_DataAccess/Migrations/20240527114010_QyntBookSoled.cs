using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_comm_DataAccess.Migrations
{
    public partial class QyntBookSoled : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "QyntBookSoled",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QyntBookSoled",
                table: "Products");
        }
    }
}
