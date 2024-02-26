using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedCarModelTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CarModel",
                columns: new[] { "Id", "Brand", "Model", "Year" },
                values: new object[,]
                {
                    { 1, "Toyota", "Carolla", 2018 },
                    { 2, "Honda", "Civic", 2020 },
                    { 3, "Chevrolet", "Camaro", 2015 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CarModel",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CarModel",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CarModel",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
