using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCafe.Menu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameDiscountToDiscountPercent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MenuItems_Discount_Valid",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PriceDiscount",
                table: "MenuItems");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceDiscountPercent",
                table: "MenuItems",
                type: "numeric(4,2)",
                precision: 4,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MenuItems_Discount_Valid",
                table: "MenuItems",
                sql: "\"PriceDiscountPercent\" >= 0 AND \"PriceDiscountPercent\" < 100");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_MenuItems_Discount_Valid",
                table: "MenuItems");

            migrationBuilder.DropColumn(
                name: "PriceDiscountPercent",
                table: "MenuItems");

            migrationBuilder.AddColumn<decimal>(
                name: "PriceDiscount",
                table: "MenuItems",
                type: "numeric(3,2)",
                precision: 3,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddCheckConstraint(
                name: "CK_MenuItems_Discount_Valid",
                table: "MenuItems",
                sql: "\"PriceDiscount\" >= 0 AND \"PriceDiscount\" <= 1");
        }
    }
}
