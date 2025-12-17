using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Migrations
{
    /// <inheritdoc />
    public partial class BenutzernameUnique : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Lehrer_Benutzername",
                table: "Lehrer",
                column: "Benutzername",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Lehrer_Benutzername",
                table: "Lehrer");
        }
    }
}
