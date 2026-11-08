using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "menu_categories",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modify_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modify_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_menu_categories", x => x.id);
                    table.ForeignKey(
                        name: "fk_menu_categories_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_menu_categories_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_menu_categories_branch_id",
                table: "menu_categories",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_menu_categories_branch_id_name",
                table: "menu_categories",
                columns: new[] { "branch_id", "name" },
                unique: true,
                filter: "\"is_deleted\" = false");

            migrationBuilder.CreateIndex(
                name: "ix_menu_categories_is_deleted",
                table: "menu_categories",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_menu_categories_tenant_id",
                table: "menu_categories",
                column: "tenant_id");

            migrationBuilder.Sql(
                """
                INSERT INTO menu_categories (id, tenant_id, branch_id, name, sort_order, create_date, is_active, is_deleted)
                SELECT gen_random_uuid(), b.tenant_id, b.id, 'General', 0, NOW(), TRUE, FALSE
                FROM branches b
                WHERE EXISTS (
                    SELECT 1 FROM menu_items mi
                    WHERE mi.branch_id = b.id AND mi.is_deleted = FALSE
                );
                """);

            migrationBuilder.AddColumn<Guid>(
                name: "category_id",
                table: "menu_items",
                type: "uuid",
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE menu_items mi
                SET category_id = c.id
                FROM menu_categories c
                WHERE c.branch_id = mi.branch_id
                  AND c.name = 'General'
                  AND c.is_deleted = FALSE
                  AND mi.category_id IS NULL;
                """);

            migrationBuilder.AlterColumn<Guid>(
                name: "category_id",
                table: "menu_items",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_menu_items_category_id",
                table: "menu_items",
                column: "category_id");

            migrationBuilder.AddForeignKey(
                name: "fk_menu_items_menu_categories_category_id",
                table: "menu_items",
                column: "category_id",
                principalTable: "menu_categories",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_menu_items_menu_categories_category_id",
                table: "menu_items");

            migrationBuilder.DropTable(
                name: "menu_categories");

            migrationBuilder.DropIndex(
                name: "ix_menu_items_category_id",
                table: "menu_items");

            migrationBuilder.DropColumn(
                name: "category_id",
                table: "menu_items");
        }
    }
}
