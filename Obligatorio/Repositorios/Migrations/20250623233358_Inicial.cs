using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Repositorios.Migrations
{
    /// <inheritdoc />
    public partial class Inicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaNacimiento = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EsAdministradorSistema = table.Column<bool>(type: "bit", nullable: false),
                    EsAdministradorProyecto = table.Column<bool>(type: "bit", nullable: false),
                    EstaAdministrandoUnProyecto = table.Column<bool>(type: "bit", nullable: false),
                    CantidadProyectosAsignados = table.Column<int>(type: "int", nullable: false),
                    EsLider = table.Column<bool>(type: "bit", nullable: false),
                    CantidadProyectosLiderando = table.Column<int>(type: "int", nullable: false),
                    ContrasenaEncriptada = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Notificacion",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Mensaje = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsuarioId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notificacion", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notificacion_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Proyectos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(400)", maxLength: 400, nullable: false),
                    AdministradorId = table.Column<int>(type: "int", nullable: false),
                    LiderId = table.Column<int>(type: "int", nullable: true),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinMasTemprana = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Proyectos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Proyectos_Usuarios_AdministradorId",
                        column: x => x.AdministradorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Proyectos_Usuarios_LiderId",
                        column: x => x.LiderId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProyectoUsuario",
                columns: table => new
                {
                    MiembrosId = table.Column<int>(type: "int", nullable: false),
                    ProyectoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProyectoUsuario", x => new { x.MiembrosId, x.ProyectoId });
                    table.ForeignKey(
                        name: "FK_ProyectoUsuario_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProyectoUsuario_Usuarios_MiembrosId",
                        column: x => x.MiembrosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Recursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProyectoAsociadoId = table.Column<int>(type: "int", nullable: true),
                    Capacidad = table.Column<int>(type: "int", nullable: false),
                    CantidadDeTareasUsandolo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recursos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recursos_Proyectos_ProyectoAsociadoId",
                        column: x => x.ProyectoAsociadoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tarea",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Titulo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuracionEnDias = table.Column<int>(type: "int", nullable: false),
                    FechaInicioMasTemprana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFinMasTemprana = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaDeEjecucion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Holgura = table.Column<int>(type: "int", nullable: false),
                    FechaInicioFijadaManualmente = table.Column<bool>(type: "bit", nullable: false),
                    ProyectoId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tarea", x => x.Id);
                    table.CheckConstraint("CK_Tarea_DuracionMayorACero", "[DuracionEnDias] > 0");
                    table.ForeignKey(
                        name: "FK_Tarea_Proyectos_ProyectoId",
                        column: x => x.ProyectoId,
                        principalTable: "Proyectos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RangoDeUso",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CantidadDeUsos = table.Column<int>(type: "int", nullable: false),
                    RecursoId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RangoDeUso", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RangoDeUso_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Dependencia",
                columns: table => new
                {
                    Tipo = table.Column<string>(type: "nvarchar(2)", maxLength: 2, nullable: false),
                    TareaDuenaId = table.Column<int>(type: "int", nullable: false),
                    TareaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dependencia", x => new { x.TareaDuenaId, x.TareaId, x.Tipo });
                    table.CheckConstraint("CK_Dependencia_Tipo", "[Tipo] IN ('SS', 'FS')");
                    table.ForeignKey(
                        name: "FK_Dependencia_Tarea_TareaDuenaId",
                        column: x => x.TareaDuenaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Dependencia_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RecursoNecesario",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RecursoId = table.Column<int>(type: "int", nullable: false),
                    Cantidad = table.Column<int>(type: "int", maxLength: 100, nullable: false),
                    TareaId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecursoNecesario", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecursoNecesario_Recursos_RecursoId",
                        column: x => x.RecursoId,
                        principalTable: "Recursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RecursoNecesario_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TareaUsuario",
                columns: table => new
                {
                    TareaId = table.Column<int>(type: "int", nullable: false),
                    UsuariosAsignadosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TareaUsuario", x => new { x.TareaId, x.UsuariosAsignadosId });
                    table.ForeignKey(
                        name: "FK_TareaUsuario_Tarea_TareaId",
                        column: x => x.TareaId,
                        principalTable: "Tarea",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TareaUsuario_Usuarios_UsuariosAsignadosId",
                        column: x => x.UsuariosAsignadosId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Apellido", "CantidadProyectosAsignados", "CantidadProyectosLiderando", "Email", "EsAdministradorProyecto", "EsAdministradorSistema", "EsLider", "EstaAdministrandoUnProyecto", "FechaNacimiento", "Nombre", "ContrasenaEncriptada" },
                values: new object[] { 1, "Sistema", 0, 0, "admin@sistema.com", false, true, false, false, new DateTime(1999, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Admin", "$2a$11$yaViT1GVglgMH3lqbDewwu8BXgasf.BqkyaazVfcUEDmydBlh4rzO" });

            migrationBuilder.CreateIndex(
                name: "IX_Dependencia_TareaId",
                table: "Dependencia",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notificacion_UsuarioId",
                table: "Notificacion",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_AdministradorId",
                table: "Proyectos",
                column: "AdministradorId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proyectos_LiderId",
                table: "Proyectos",
                column: "LiderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProyectoUsuario_ProyectoId",
                table: "ProyectoUsuario",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_RangoDeUso_RecursoId",
                table: "RangoDeUso",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursoNecesario_RecursoId",
                table: "RecursoNecesario",
                column: "RecursoId");

            migrationBuilder.CreateIndex(
                name: "IX_RecursoNecesario_TareaId",
                table: "RecursoNecesario",
                column: "TareaId");

            migrationBuilder.CreateIndex(
                name: "IX_Recursos_ProyectoAsociadoId",
                table: "Recursos",
                column: "ProyectoAsociadoId");

            migrationBuilder.CreateIndex(
                name: "IX_Tarea_ProyectoId",
                table: "Tarea",
                column: "ProyectoId");

            migrationBuilder.CreateIndex(
                name: "IX_TareaUsuario_UsuariosAsignadosId",
                table: "TareaUsuario",
                column: "UsuariosAsignadosId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dependencia");

            migrationBuilder.DropTable(
                name: "Notificacion");

            migrationBuilder.DropTable(
                name: "ProyectoUsuario");

            migrationBuilder.DropTable(
                name: "RangoDeUso");

            migrationBuilder.DropTable(
                name: "RecursoNecesario");

            migrationBuilder.DropTable(
                name: "TareaUsuario");

            migrationBuilder.DropTable(
                name: "Recursos");

            migrationBuilder.DropTable(
                name: "Tarea");

            migrationBuilder.DropTable(
                name: "Proyectos");

            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
