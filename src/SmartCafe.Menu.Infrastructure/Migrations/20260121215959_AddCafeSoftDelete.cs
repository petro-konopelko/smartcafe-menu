using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCafe.Menu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCafeSoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Cafes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Cafes",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Cafes");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Cafes");
        }
    }
}
