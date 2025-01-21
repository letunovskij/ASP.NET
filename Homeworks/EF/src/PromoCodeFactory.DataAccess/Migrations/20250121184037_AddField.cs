using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PromoCodeFactory.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Scale",
                table: "Customers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Scale",
                table: "Customers");
        }
    }
}
