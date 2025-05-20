using Microsoft.EntityFrameworkCore.Migrations;

namespace WanderlustJournal.Data.Migrations
{
    public class AddCoordinatesFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Latitude",
                table: "JournalEntries",
                type: "decimal(10, 7)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Longitude",
                table: "JournalEntries",
                type: "decimal(10, 7)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "JournalEntries");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "JournalEntries");
        }
    }
}