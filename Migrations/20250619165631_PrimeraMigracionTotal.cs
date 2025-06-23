using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RIFA.Migrations
{
    /// <inheritdoc />
    public partial class PrimeraMigracionTotal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Administrador",
                columns: table => new
                {
                    AdministradorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Administrador", x => x.AdministradorID);
                });

            migrationBuilder.CreateTable(
                name: "MaterialReciclado",
                columns: table => new
                {
                    MaterialRecicladoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoMaterial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PuntosPorUnidad = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MaterialReciclado", x => x.MaterialRecicladoID);
                });

            migrationBuilder.CreateTable(
                name: "Producto",
                columns: table => new
                {
                    ProductoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PrecioPuntos = table.Column<int>(type: "int", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Producto", x => x.ProductoID);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Contrasena = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NumeroBoleta = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PuntosAcumulados = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    Rol = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Usuario")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioID);
                });

            migrationBuilder.CreateTable(
                name: "Registro",
                columns: table => new
                {
                    RegistroID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdministradorID = table.Column<int>(type: "int", nullable: false),
                    MaterialRecicladoID = table.Column<int>(type: "int", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CantidadRegistrada = table.Column<double>(type: "float", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Registro", x => x.RegistroID);
                    table.ForeignKey(
                        name: "FK_Registro_Administrador_AdministradorID",
                        column: x => x.AdministradorID,
                        principalTable: "Administrador",
                        principalColumn: "AdministradorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Registro_MaterialReciclado_MaterialRecicladoID",
                        column: x => x.MaterialRecicladoID,
                        principalTable: "MaterialReciclado",
                        principalColumn: "MaterialRecicladoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Canje",
                columns: table => new
                {
                    CanjeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    ProductoID = table.Column<int>(type: "int", nullable: false),
                    PuntosCanjeados = table.Column<int>(type: "int", nullable: false),
                    FechaCanje = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Canje", x => x.CanjeID);
                    table.ForeignKey(
                        name: "FK_Canje_Producto_ProductoID",
                        column: x => x.ProductoID,
                        principalTable: "Producto",
                        principalColumn: "ProductoID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Canje_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Historial",
                columns: table => new
                {
                    HistorialID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UsuarioID = table.Column<int>(type: "int", nullable: false),
                    MaterialRecicladoID = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<double>(type: "float", nullable: false),
                    PuntosGanados = table.Column<int>(type: "int", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false, defaultValueSql: "GETDATE()"),
                    TipoMaterial = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Historial", x => x.HistorialID);
                    table.ForeignKey(
                        name: "FK_Historial_MaterialReciclado_MaterialRecicladoID",
                        column: x => x.MaterialRecicladoID,
                        principalTable: "MaterialReciclado",
                        principalColumn: "MaterialRecicladoID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Historial_Usuario_UsuarioID",
                        column: x => x.UsuarioID,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Administrador",
                columns: new[] { "AdministradorID", "Contrasena", "Email", "Nombre" },
                values: new object[,]
                {
                    { 1, "66c6153d577b78384e7372eb047e7adcc5000af43977cce9053d187031ed7dea", "brunoarroyo1801@gmail.com", "Bruno Perez" },
                    { 2, "fe14fa851d8f1ce5bb7765ca2d3cc4129a0cf9d16b941f1e5b9771e0ca2a659a", "munozhuizardaniela@gmail.com", "Daniela Huizar" },
                    { 3, "c6ba922539eae0e7e0038af1ba16d35267dacc6ecc6ae2d2539f35907b151e86", "leyvahernandez310@gmail.com", "Maria Leyva" },
                    { 4, "7bd2b880ccc7ddd2e72ebf764d35908b1aff805961136b0c81f2a515e33361e0", "lilianlizetteramirezchaparro@gmail.com", "Lilian Ramirez" },
                    { 5, "9fbdffae9b43d5b3a37a7fd7cb51db3912693bd51b23a913d2bf12b805ddf44a", "enokvoca13@gmail.com", "Enok Lobato" }
                });

            migrationBuilder.InsertData(
                table: "MaterialReciclado",
                columns: new[] { "MaterialRecicladoID", "Descripcion", "ImagenUrl", "PuntosPorUnidad", "TipoMaterial" },
                values: new object[,]
                {
                    { 1, "Botellas de plástico PET", null, 5, "Botellas PET" },
                    { 2, "Tapas de plástico de botellas", null, 1, "Tapas" },
                    { 3, "Hojas de papel de desecho", null, 2, "Hojas de Papel" }
                });

            migrationBuilder.InsertData(
                table: "Producto",
                columns: new[] { "ProductoID", "Descripcion", "ImagenUrl", "Nombre", "PrecioPuntos", "Stock" },
                values: new object[,]
                {
                    { 1, "1 copia blanco y negro.", "/imagenes/copias.jpg", "Copias", 2, 900 },
                    { 2, "1 pluma negra.", "/imagenes/pluma.jpg", "Plumas", 10, 900 },
                    { 3, "1 impresión a b/n tamaño carta.", "/imagenes//impresion.jpg", "Impresiones", 4, 900 },
                    { 4, "1 lápiz de grafito HB.", "/imagenes/lapiz.jpg", "Lápices", 10, 900 },
                    { 5, "Tijeras de acero inoxidable.", "/imagenes/tijera.jpg", "Tijeras", 20, 900 },
                    { 6, "Cuaderno profesional de 100 hojas", "/imagenes/cuaderno.jpg", "Cuadernos", 60, 900 },
                    { 7, "Regla de plástico de 20cm.", "/imagenes/regla.jpg", "Reglas", 15, 900 },
                    { 8, "Folder marrón de cartón.", "/imagenes/folder.jpg", "Folders", 5, 900 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrador_Email",
                table: "Administrador",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Canje_ProductoID",
                table: "Canje",
                column: "ProductoID");

            migrationBuilder.CreateIndex(
                name: "IX_Canje_UsuarioID",
                table: "Canje",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_MaterialRecicladoID",
                table: "Historial",
                column: "MaterialRecicladoID");

            migrationBuilder.CreateIndex(
                name: "IX_Historial_UsuarioID",
                table: "Historial",
                column: "UsuarioID");

            migrationBuilder.CreateIndex(
                name: "IX_Registro_AdministradorID",
                table: "Registro",
                column: "AdministradorID");

            migrationBuilder.CreateIndex(
                name: "IX_Registro_MaterialRecicladoID",
                table: "Registro",
                column: "MaterialRecicladoID");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_Email",
                table: "Usuario",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_NumeroBoleta",
                table: "Usuario",
                column: "NumeroBoleta",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Canje");

            migrationBuilder.DropTable(
                name: "Historial");

            migrationBuilder.DropTable(
                name: "Registro");

            migrationBuilder.DropTable(
                name: "Producto");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "Administrador");

            migrationBuilder.DropTable(
                name: "MaterialReciclado");
        }
    }
}
