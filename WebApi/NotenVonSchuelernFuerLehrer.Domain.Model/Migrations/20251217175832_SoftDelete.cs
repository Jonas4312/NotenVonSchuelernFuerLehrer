using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Migrations
{
    /// <inheritdoc />
    public partial class SoftDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Schueler",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Note",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Lehrer",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Klasse",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Fach",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Schueler");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Note");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Lehrer");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Klasse");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Fach");
        }
    }
}
