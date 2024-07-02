using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace E_comm_DataAccess.Migrations
{
    public partial class checkbox : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "selectedItems",
                table: "ShoppingCarts",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "selectedItems",
                table: "ShoppingCarts");
        }
    }
}
