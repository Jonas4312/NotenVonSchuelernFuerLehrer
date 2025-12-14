using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Migrations
{
    /// <inheritdoc />
    public partial class NotizenZuNote : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Notiz",
                table: "Note",
                type: "varchar(255)",
                maxLength: 255,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Notiz",
                table: "Note");
        }
    }
}
