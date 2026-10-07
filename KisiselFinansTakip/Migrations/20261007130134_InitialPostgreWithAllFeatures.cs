using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace KisiselFinansTakip.Migrations
{
    /// <inheritdoc />
    public partial class InitialPostgreWithAllFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Kategoriler",
                columns: table => new
                {
                    KategoriID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KategoriAdi = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tip = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kategoriler", x => x.KategoriID);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    KullaniciID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Ad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Sifre = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rol = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    KayitTarihi = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.KullaniciID);
                });

            migrationBuilder.CreateTable(
                name: "ButceHedefleri",
                columns: table => new
                {
                    HedefID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    KullaniciID = table.Column<int>(type: "integer", nullable: false),
                    KategoriID = table.Column<int>(type: "integer", nullable: false),
                    Ay = table.Column<int>(type: "integer", nullable: false),
                    Yil = table.Column<int>(type: "integer", nullable: false),
                    HedefTutar = table.Column<decimal>(type: "numeric(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ButceHedefleri", x => x.HedefID);
                    table.ForeignKey(
                        name: "FK_ButceHedefleri_Kategoriler_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ButceHedefleri_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Islemler",
                columns: table => new
                {
                    IslemID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Aciklama = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Tutar = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Tarih = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    KategoriID = table.Column<int>(type: "integer", nullable: false),
                    KullaniciID = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Islemler", x => x.IslemID);
                    table.ForeignKey(
                        name: "FK_Islemler_Kategoriler_KategoriID",
                        column: x => x.KategoriID,
                        principalTable: "Kategoriler",
                        principalColumn: "KategoriID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Islemler_Kullanicilar_KullaniciID",
                        column: x => x.KullaniciID,
                        principalTable: "Kullanicilar",
                        principalColumn: "KullaniciID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ButceHedefleri_KategoriID",
                table: "ButceHedefleri",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_ButceHedefleri_KullaniciID",
                table: "ButceHedefleri",
                column: "KullaniciID");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_KategoriID",
                table: "Islemler",
                column: "KategoriID");

            migrationBuilder.CreateIndex(
                name: "IX_Islemler_KullaniciID",
                table: "Islemler",
                column: "KullaniciID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ButceHedefleri");

            migrationBuilder.DropTable(
                name: "Islemler");

            migrationBuilder.DropTable(
                name: "Kategoriler");

            migrationBuilder.DropTable(
                name: "Kullanicilar");
        }
    }
}
