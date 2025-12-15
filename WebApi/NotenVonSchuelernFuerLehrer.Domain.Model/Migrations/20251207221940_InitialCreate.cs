using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotenVonSchuelernFuerLehrer.Domain.Model.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fach",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Bezeichnung = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Kurzbezeichnung = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fach", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Klasse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Bezeichnung = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Kurzbezeichnung = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Klasse", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lehrer",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Vorname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Nachname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Benutzername = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    PasswortHash = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    BildByteArray = table.Column<byte[]>(type: "longblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lehrer", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LehrerKlasse",
                columns: table => new
                {
                    LehrerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    KlasseId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LehrerKlasse", x => new { x.LehrerId, x.KlasseId });
                    table.ForeignKey(
                        name: "FK_LehrerKlasse_Klasse",
                        column: x => x.KlasseId,
                        principalTable: "Klasse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LehrerKlasse_Lehrer",
                        column: x => x.LehrerId,
                        principalTable: "Lehrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Schueler",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    KlasseId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Vorname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    Nachname = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false),
                    BildByteArray = table.Column<byte[]>(type: "longblob", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schueler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Schueler_Klasse",
                        column: x => x.KlasseId,
                        principalTable: "Klasse",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "LehrerFach",
                columns: table => new
                {
                    LehrerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FachId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LehrerFach", x => new { x.LehrerId, x.FachId });
                    table.ForeignKey(
                        name: "FK_LehrerFach_Fach",
                        column: x => x.FachId,
                        principalTable: "Fach",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LehrerFach_Lehrer",
                        column: x => x.LehrerId,
                        principalTable: "Lehrer",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Note",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    SchuelerId = table.Column<Guid>(type: "char(36)", nullable: false),
                    FachId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Wert = table.Column<int>(type: "int", nullable: false),
                    ErstelltAm = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    AngepasstAm = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Note", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Note_Fach",
                        column: x => x.FachId,
                        principalTable: "Fach",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Note_Schueler",
                        column: x => x.SchuelerId,
                        principalTable: "Schueler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_LehrerKlasse_KlasseId",
                table: "LehrerKlasse",
                column: "KlasseId");

            migrationBuilder.CreateIndex(
                name: "IX_LehrerFach_FachId",
                table: "LehrerFach",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_FachId",
                table: "Note",
                column: "FachId");

            migrationBuilder.CreateIndex(
                name: "IX_Note_SchuelerId",
                table: "Note",
                column: "SchuelerId");

            migrationBuilder.CreateIndex(
                name: "IX_Schueler_KlasseId",
                table: "Schueler",
                column: "KlasseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LehrerKlasse");

            migrationBuilder.DropTable(
                name: "LehrerFach");

            migrationBuilder.DropTable(
                name: "Note");

            migrationBuilder.DropTable(
                name: "Lehrer");

            migrationBuilder.DropTable(
                name: "Fach");

            migrationBuilder.DropTable(
                name: "Schueler");

            migrationBuilder.DropTable(
                name: "Klasse");
        }
    }
}
