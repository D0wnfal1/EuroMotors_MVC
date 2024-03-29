using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddNewColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CarModels",
                columns: new[] { "Id", "Brand", "DisplayOrder", "Model", "Year" },
                values: new object[] { 4, "Audi", 4, "Q7", 2005 });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "DisplayOrder", "Name" },
                values: new object[] { 4, 4, "Інструменти" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Desctiption", "ListPrice", "Price", "Title", "VendorCode" },
                values: new object[] { 1, "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took", 1699.0, 1699.0, "Моторна олива MANNOL ELITE SAE 5W-40 4л (MN7903-4)", "1287435" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CarModels",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "CategoryId", "Desctiption", "ListPrice", "Price", "Title", "VendorCode" },
                values: new object[] { 3, "Lorem Ipsum is simply dummy text of the printing and ty +pesetting industry.", 507.0, 500.0, "Свічка розжарювання", "159Rer0080" });
        }
    }
}
