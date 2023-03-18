using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sestio.Usuarios.Startup.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class TabelasColunasSemUnderline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_token_sessao",
                table: "token_sessao");

            migrationBuilder.RenameTable(
                name: "token_sessao",
                newName: "tokensessao");

            migrationBuilder.RenameColumn(
                name: "id_usuario",
                table: "sessao",
                newName: "idusuario");

            migrationBuilder.RenameColumn(
                name: "data_validade",
                table: "sessao",
                newName: "datavalidade");

            migrationBuilder.RenameColumn(
                name: "data_criacao",
                table: "sessao",
                newName: "datacriacao");

            migrationBuilder.RenameColumn(
                name: "id_sessao",
                table: "tokensessao",
                newName: "idsessao");

            migrationBuilder.RenameIndex(
                name: "IX_token_sessao_token",
                table: "tokensessao",
                newName: "IX_tokensessao_token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tokensessao",
                table: "tokensessao",
                column: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_tokensessao",
                table: "tokensessao");

            migrationBuilder.RenameTable(
                name: "tokensessao",
                newName: "token_sessao");

            migrationBuilder.RenameColumn(
                name: "idusuario",
                table: "sessao",
                newName: "id_usuario");

            migrationBuilder.RenameColumn(
                name: "datavalidade",
                table: "sessao",
                newName: "data_validade");

            migrationBuilder.RenameColumn(
                name: "datacriacao",
                table: "sessao",
                newName: "data_criacao");

            migrationBuilder.RenameColumn(
                name: "idsessao",
                table: "token_sessao",
                newName: "id_sessao");

            migrationBuilder.RenameIndex(
                name: "IX_tokensessao_token",
                table: "token_sessao",
                newName: "IX_token_sessao_token");

            migrationBuilder.AddPrimaryKey(
                name: "PK_token_sessao",
                table: "token_sessao",
                column: "id");
        }
    }
}
