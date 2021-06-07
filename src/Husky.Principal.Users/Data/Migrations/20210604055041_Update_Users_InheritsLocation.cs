using Microsoft.EntityFrameworkCore.Migrations;

namespace Husky.Principal.Users.Data.Migrations
{
    public partial class Update_Users_InheritsLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location_Lon",
                table: "UserAddresses",
                newName: "Lon");

            migrationBuilder.RenameColumn(
                name: "Location_LatLonType",
                table: "UserAddresses",
                newName: "LatLonType");

            migrationBuilder.RenameColumn(
                name: "Location_Lat",
                table: "UserAddresses",
                newName: "Lat");

            migrationBuilder.AlterColumn<decimal>(
                name: "Lon",
                table: "UserAddresses",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "LatLonType",
                table: "UserAddresses",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Lat",
                table: "UserAddresses",
                type: "decimal(9,6)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Lon",
                table: "UserAddresses",
                newName: "Location_Lon");

            migrationBuilder.RenameColumn(
                name: "LatLonType",
                table: "UserAddresses",
                newName: "Location_LatLonType");

            migrationBuilder.RenameColumn(
                name: "Lat",
                table: "UserAddresses",
                newName: "Location_Lat");

            migrationBuilder.AlterColumn<decimal>(
                name: "Location_Lon",
                table: "UserAddresses",
                type: "decimal(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");

            migrationBuilder.AlterColumn<int>(
                name: "Location_LatLonType",
                table: "UserAddresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "Location_Lat",
                table: "UserAddresses",
                type: "decimal(9,6)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)");
        }
    }
}
