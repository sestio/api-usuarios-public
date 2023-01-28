using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sestio.Usuarios.Startup.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class CriaTabelasSessao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sessao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    Situacao = table.Column<int>(type: "integer", nullable: false),
                    idusuario = table.Column<Guid>(name: "id_usuario", type: "uuid", nullable: false),
                    datacriacao = table.Column<DateTime>(name: "data_criacao", type: "timestamp with time zone", nullable: false),
                    datavalidade = table.Column<DateTime>(name: "data_validade", type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sessao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "token_sessao",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    situacao = table.Column<int>(type: "integer", nullable: false),
                    idsessao = table.Column<Guid>(name: "id_sessao", type: "uuid", nullable: false),
                    token = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token_sessao", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_token_sessao_token",
                table: "token_sessao",
                column: "token",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "sessao");

            migrationBuilder.DropTable(
                name: "token_sessao");
        }
    }
}
