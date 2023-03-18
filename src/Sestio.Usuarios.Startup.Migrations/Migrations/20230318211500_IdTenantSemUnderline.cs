using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sestio.Usuarios.Startup.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class IdTenantSemUnderline : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id_tenant",
                table: "usuario",
                newName: "idtenant");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "idtenant",
                table: "usuario",
                newName: "id_tenant");
        }
    }
}
