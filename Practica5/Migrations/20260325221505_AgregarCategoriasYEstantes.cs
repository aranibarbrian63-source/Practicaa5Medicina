using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Practica5.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCategoriasYEstantes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Estantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Estantes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_CategoriaId",
                table: "Medicamentos",
                column: "CategoriaId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicamentos_EstanteId",
                table: "Medicamentos",
                column: "EstanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Medicamentos_Categorias_CategoriaId",
                table: "Medicamentos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Medicamentos_Estantes_EstanteId",
                table: "Medicamentos",
                column: "EstanteId",
                principalTable: "Estantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Medicamentos_Categorias_CategoriaId",
                table: "Medicamentos");

            migrationBuilder.DropForeignKey(
                name: "FK_Medicamentos_Estantes_EstanteId",
                table: "Medicamentos");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Estantes");

            migrationBuilder.DropIndex(
                name: "IX_Medicamentos_CategoriaId",
                table: "Medicamentos");

            migrationBuilder.DropIndex(
                name: "IX_Medicamentos_EstanteId",
                table: "Medicamentos");
        }
    }
}
