using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Branches.Migrations
{
    /// <inheritdoc />
    public partial class BranchCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "branch_menus",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceMenuTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SourceVersion = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branch_menus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "branch_categories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BranchMenuId = table.Column<Guid>(type: "uuid", nullable: false),
                    SourceCategoryTemplateId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    DisplayOrder = table.Column<int>(type: "integer", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_branch_categories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_branch_categories_branch_menus_BranchMenuId",
                        column: x => x.BranchMenuId,
                        principalTable: "branch_menus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BranchProducts_BranchCategoryId",
                table: "BranchProducts",
                column: "BranchCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_branch_categories_BranchMenuId",
                table: "branch_categories",
                column: "BranchMenuId");

            migrationBuilder.AddForeignKey(
                name: "FK_BranchProducts_branch_categories_BranchCategoryId",
                table: "BranchProducts",
                column: "BranchCategoryId",
                principalTable: "branch_categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BranchProducts_branch_categories_BranchCategoryId",
                table: "BranchProducts");

            migrationBuilder.DropTable(
                name: "branch_categories");

            migrationBuilder.DropTable(
                name: "branch_menus");

            migrationBuilder.DropIndex(
                name: "IX_BranchProducts_BranchCategoryId",
                table: "BranchProducts");
        }
    }
}
