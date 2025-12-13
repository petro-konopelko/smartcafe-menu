using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SmartCafe.Menu.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cafes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactInfo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cafes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CafeId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    State = table.Column<int>(type: "integer", nullable: false),
                    PublishedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ActivatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Menus", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Menus_Cafes_CafeId",
                        column: x => x.CafeId,
                        principalTable: "Cafes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sections",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MenuId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    AvailableFrom = table.Column<TimeSpan>(type: "interval", nullable: true),
                    AvailableTo = table.Column<TimeSpan>(type: "interval", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sections_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MenuItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SectionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PriceAmount = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    PriceUnit = table.Column<int>(type: "integer", nullable: false),
                    PriceDiscount = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: false),
                    ImageOriginalPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    ImageThumbnailPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IngredientOptions = table.Column<string>(type: "jsonb", nullable: false),
                    Position = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuItems", x => x.Id);
                    table.CheckConstraint("CK_MenuItems_Discount_Valid", "\"PriceDiscount\" >= 0 AND \"PriceDiscount\" <= 1");
                    table.CheckConstraint("CK_MenuItems_Price_Positive", "\"PriceAmount\" > 0");
                    table.ForeignKey(
                        name: "FK_MenuItems_Sections_SectionId",
                        column: x => x.SectionId,
                        principalTable: "Sections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cafes_Name",
                table: "Cafes",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_CreatedAt",
                table: "MenuItems",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_IngredientOptions",
                table: "MenuItems",
                column: "IngredientOptions")
                .Annotation("Npgsql:IndexMethod", "gin");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_SectionId",
                table: "MenuItems",
                column: "SectionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuItems_SectionId_Position",
                table: "MenuItems",
                columns: new[] { "SectionId", "Position" });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_CafeId_CreatedAt",
                table: "Menus",
                columns: new[] { "CafeId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Menus_CafeId_State",
                table: "Menus",
                columns: new[] { "CafeId", "State" });

            migrationBuilder.CreateIndex(
                name: "UX_Menus_CafeId_Active",
                table: "Menus",
                column: "CafeId",
                unique: true,
                filter: "\"State\" = 2");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_MenuId",
                table: "Sections",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Sections_MenuId_Position",
                table: "Sections",
                columns: new[] { "MenuId", "Position" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MenuItems");

            migrationBuilder.DropTable(
                name: "Sections");

            migrationBuilder.DropTable(
                name: "Menus");

            migrationBuilder.DropTable(
                name: "Cafes");
        }
    }
}
