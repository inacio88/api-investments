using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace api.Migrations
{
    /// <inheritdoc />
    public partial class Investment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Investment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    OwnerId = table.Column<string>(type: "varchar(128)", nullable: false),
                    CreationDate = table.Column<DateTime>(type: "timestamptz", nullable: false),
                    InitialAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WithdrawalDate = table.Column<DateTime>(type: "timestamptz", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Investment", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Investment_OwnerId",
                table: "Investment",
                column: "OwnerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Investment");
        }
    }
}
