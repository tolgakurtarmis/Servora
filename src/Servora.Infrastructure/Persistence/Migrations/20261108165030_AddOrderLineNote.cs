using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Servora.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddOrderLineNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "note",
                table: "order_lines",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "note",
                table: "order_lines");
        }
    }
}
