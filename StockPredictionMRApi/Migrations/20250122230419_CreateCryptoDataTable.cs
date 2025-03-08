using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StockPredictionMRApi.Migrations
{
    /// <inheritdoc />
    public partial class CreateCryptoDataTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CryptoData",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<float>(type: "real", nullable: false),
                    High = table.Column<float>(type: "real", nullable: false),
                    Low = table.Column<float>(type: "real", nullable: false),
                    Close = table.Column<float>(type: "real", nullable: false),
                    Volume = table.Column<float>(type: "real", nullable: false),
                    Symbol = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CryptoData", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CryptoData");
        }
    }
}
