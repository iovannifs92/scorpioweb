using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace scorpioweb.Migrations
{
    public partial class Local : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Familiares",
                table: "persona",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenciasPersonales",
                table: "persona",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "victima",
                columns: table => new
                {
                    idVictima = table.Column<int>(nullable: false),
                    Supervision_idSupervision = table.Column<int>(nullable: false),
                    ConoceDetenido = table.Column<string>(maxLength: 2, nullable: true),
                    Direccion = table.Column<string>(maxLength: 300, nullable: true),
                    Edad = table.Column<string>(maxLength: 45, nullable: true),
                    Nombre = table.Column<string>(maxLength: 150, nullable: true),
                    Telefono = table.Column<string>(maxLength: 10, nullable: true),
                    TiempoConocerlo = table.Column<string>(maxLength: 75, nullable: true),
                    TipoRelacion = table.Column<string>(maxLength: 100, nullable: true),
                    victimacol = table.Column<string>(maxLength: 45, nullable: true),
                    ViveSupervisado = table.Column<string>(maxLength: 45, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_victima", x => new { x.idVictima, x.Supervision_idSupervision });
                });

            migrationBuilder.CreateIndex(
                name: "fk_Victima_Supervision_idx",
                table: "victima",
                column: "Supervision_idSupervision");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "victima");

            migrationBuilder.DropColumn(
                name: "Familiares",
                table: "persona");

            migrationBuilder.DropColumn(
                name: "ReferenciasPersonales",
                table: "persona");
        }
    }
}
