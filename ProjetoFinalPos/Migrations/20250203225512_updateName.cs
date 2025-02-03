using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjetoFinalPos.Migrations
{
    /// <inheritdoc />
    public partial class updateName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Clientes",
                newName: "Clients");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Clients",
                newName: "Clientes");
        }
    }
}
