using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CarModels_CarModelId",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelId",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImageUrl",
                value: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImageUrl",
                value: "");

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Brand", "CarModelId", "CategoryId", "Desctiption", "ImageUrl", "ListPrice", "Price", "Title", "VendorCode" },
                values: new object[] { 4, "Bosch", null, 3, "Lorem Ipsum is simply dummy text of the printing and ty +pesetting industry.", "", 507.0, 500.0, "Свічка розжарювання", "159Rer0080" });

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CarModels_CarModelId",
                table: "Products",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CarModels_CarModelId",
                table: "Products");

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Products");

            migrationBuilder.AlterColumn<int>(
                name: "CarModelId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CarModels_CarModelId",
                table: "Products",
                column: "CarModelId",
                principalTable: "CarModels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
