using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumnsToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CarModelId", "CategoryId" },
                values: new object[] { 4, 4 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CarModelId", "CategoryId" },
                values: new object[] { null, 1 });
        }
    }
}
