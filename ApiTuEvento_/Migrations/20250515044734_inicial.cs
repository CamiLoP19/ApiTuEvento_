using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiTuEvento_.Migrations
{
    /// <inheritdoc />
    public partial class inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "carritos",
                columns: table => new
                {
                    IdCarrito = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_carritos", x => x.IdCarrito);
                });

            migrationBuilder.CreateTable(
                name: "categoriaEventos",
                columns: table => new
                {
                    IdCategoriaEvento = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_categoriaEventos", x => x.IdCategoriaEvento);
                });

            migrationBuilder.CreateTable(
                name: "personas",
                columns: table => new
                {
                    PersonaId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cedula = table.Column<double>(type: "float", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Contraseña = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CarritoIdCarrito = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_personas", x => x.PersonaId);
                    table.ForeignKey(
                        name: "FK_personas_carritos_CarritoIdCarrito",
                        column: x => x.CarritoIdCarrito,
                        principalTable: "carritos",
                        principalColumn: "IdCarrito");
                });

            migrationBuilder.CreateTable(
                name: "eventos",
                columns: table => new
                {
                    EventoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NombreEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEvento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LugarEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aforo = table.Column<int>(type: "int", nullable: false),
                    CategoriaEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DescripcionEvento = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImagenUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstadoEventoActivo = table.Column<bool>(type: "bit", nullable: false),
                    CategoriaEventoIdCategoriaEvento = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_eventos", x => x.EventoId);
                    table.ForeignKey(
                        name: "FK_eventos_categoriaEventos_CategoriaEventoIdCategoriaEvento",
                        column: x => x.CategoriaEventoIdCategoriaEvento,
                        principalTable: "categoriaEventos",
                        principalColumn: "IdCategoriaEvento");
                });

            migrationBuilder.CreateTable(
                name: "boletos",
                columns: table => new
                {
                    BoletoId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TipoBoleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Precio = table.Column<double>(type: "float", nullable: false),
                    EstadoVenta = table.Column<bool>(type: "bit", nullable: false),
                    CodigoQR = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodigoAN = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    PersonaId = table.Column<int>(type: "int", nullable: true),
                    UsuarioPersonaId = table.Column<int>(type: "int", nullable: true),
                    CarritoIdCarrito = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_boletos", x => x.BoletoId);
                    table.ForeignKey(
                        name: "FK_boletos_carritos_CarritoIdCarrito",
                        column: x => x.CarritoIdCarrito,
                        principalTable: "carritos",
                        principalColumn: "IdCarrito");
                    table.ForeignKey(
                        name: "FK_boletos_eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "eventos",
                        principalColumn: "EventoId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_boletos_personas_UsuarioPersonaId",
                        column: x => x.UsuarioPersonaId,
                        principalTable: "personas",
                        principalColumn: "PersonaId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_boletos_CarritoIdCarrito",
                table: "boletos",
                column: "CarritoIdCarrito");

            migrationBuilder.CreateIndex(
                name: "IX_boletos_EventoId",
                table: "boletos",
                column: "EventoId");

            migrationBuilder.CreateIndex(
                name: "IX_boletos_UsuarioPersonaId",
                table: "boletos",
                column: "UsuarioPersonaId");

            migrationBuilder.CreateIndex(
                name: "IX_eventos_CategoriaEventoIdCategoriaEvento",
                table: "eventos",
                column: "CategoriaEventoIdCategoriaEvento");

            migrationBuilder.CreateIndex(
                name: "IX_personas_CarritoIdCarrito",
                table: "personas",
                column: "CarritoIdCarrito");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "boletos");

            migrationBuilder.DropTable(
                name: "eventos");

            migrationBuilder.DropTable(
                name: "personas");

            migrationBuilder.DropTable(
                name: "categoriaEventos");

            migrationBuilder.DropTable(
                name: "carritos");
        }
    }
}
