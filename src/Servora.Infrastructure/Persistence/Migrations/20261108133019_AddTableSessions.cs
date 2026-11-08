using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTableSessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "table_sessions",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    branch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    table_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    create_by = table.Column<Guid>(type: "uuid", nullable: true),
                    create_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    modify_by = table.Column<Guid>(type: "uuid", nullable: true),
                    modify_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_table_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_table_sessions_branches_branch_id",
                        column: x => x.branch_id,
                        principalTable: "branches",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_table_sessions_tables_table_id",
                        column: x => x.table_id,
                        principalTable: "tables",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_table_sessions_tenants_tenant_id",
                        column: x => x.tenant_id,
                        principalTable: "tenants",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_table_sessions_branch_id",
                table: "table_sessions",
                column: "branch_id");

            migrationBuilder.CreateIndex(
                name: "ix_table_sessions_is_deleted",
                table: "table_sessions",
                column: "is_deleted");

            migrationBuilder.CreateIndex(
                name: "ix_table_sessions_table_id",
                table: "table_sessions",
                column: "table_id",
                unique: true,
                filter: "\"status\" = 1 AND \"is_deleted\" = false");

            migrationBuilder.CreateIndex(
                name: "ix_table_sessions_tenant_id",
                table: "table_sessions",
                column: "tenant_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "table_sessions");
        }
    }
}
