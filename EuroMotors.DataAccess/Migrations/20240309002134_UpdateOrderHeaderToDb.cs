using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrderHeaderToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionId",
                table: "OrderHeaders",
                newName: "Signature");

            migrationBuilder.RenameColumn(
                name: "PaymentIntentId",
                table: "OrderHeaders",
                newName: "Data");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Signature",
                table: "OrderHeaders",
                newName: "SessionId");

            migrationBuilder.RenameColumn(
                name: "Data",
                table: "OrderHeaders",
                newName: "PaymentIntentId");
        }
    }
}
