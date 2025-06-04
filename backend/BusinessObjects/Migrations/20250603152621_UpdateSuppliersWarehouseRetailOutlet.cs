using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BusinessObjects.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSuppliersWarehouseRetailOutlet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Warehouses",
                newName: "WarehouseLocation");

            migrationBuilder.RenameColumn(
                name: "ContactInfo",
                table: "Suppliers",
                newName: "SupplierPhoneNumber");

            migrationBuilder.RenameColumn(
                name: "OutletName",
                table: "RetailOutlets",
                newName: "RetailOutletName");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "RetailOutlets",
                newName: "RetailOutletLocation");

            migrationBuilder.AddColumn<string>(
                name: "SupplierEmail",
                table: "Suppliers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupplierEmail",
                table: "Suppliers");

            migrationBuilder.RenameColumn(
                name: "WarehouseLocation",
                table: "Warehouses",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "SupplierPhoneNumber",
                table: "Suppliers",
                newName: "ContactInfo");

            migrationBuilder.RenameColumn(
                name: "RetailOutletName",
                table: "RetailOutlets",
                newName: "OutletName");

            migrationBuilder.RenameColumn(
                name: "RetailOutletLocation",
                table: "RetailOutlets",
                newName: "Location");
        }
    }
}
