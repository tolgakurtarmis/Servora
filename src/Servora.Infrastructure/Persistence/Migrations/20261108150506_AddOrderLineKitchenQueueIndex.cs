using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLineKitchenQueueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_order_lines_branch_id_kitchen_status",
                table: "order_lines",
                columns: new[] { "branch_id", "kitchen_status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_order_lines_branch_id_kitchen_status",
                table: "order_lines");
        }
    }
}
