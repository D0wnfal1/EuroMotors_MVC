using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EuroMotors.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGuestIdColumnInOrderHeaderTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GuestId",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestId",
                table: "OrderHeaders");
        }
    }
}
