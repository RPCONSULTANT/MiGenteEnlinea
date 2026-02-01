using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiGenteEnLinea.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Tipo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreCompleto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlanID = table.Column<int>(type: "int", nullable: false),
                    VencimientoPlan = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UltimoLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Config_Correo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    email = table.Column<string>(type: "nvarchar(70)", maxLength: 70, nullable: false),
                    pass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    servidor = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    puerto = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Config_Correo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ConsultasPerfil",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    empleadorUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    contratistaIdentificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    fechaConsulta = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ipAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsultasPerfil", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Credenciales",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    password = table.Column<string>(type: "varchar(max)", unicode: false, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fecha_activacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ultimo_acceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ultima_ip = table.Column<string>(type: "varchar(45)", unicode: false, maxLength: 45, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    created_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credenciales", x => x.id);
                    table.UniqueConstraint("AK_Credenciales_userID", x => x.userID);
                });

            migrationBuilder.CreateTable(
                name: "Deducciones_TSS",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    porcentaje = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    tope_salarial = table.Column<decimal>(type: "decimal(12,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deducciones_TSS", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Empleados",
                columns: table => new
                {
                    empleadoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    fechaInicio = table.Column<DateOnly>(type: "date", nullable: true),
                    identificacion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    alias = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    nacimiento = table.Column<DateOnly>(type: "date", nullable: true),
                    estadoCivil = table.Column<int>(type: "int", nullable: true),
                    direccion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    provincia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    municipio = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    telefono1 = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    telefono2 = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    posicion = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    salario = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    periodoPago = table.Column<int>(type: "int", nullable: false),
                    contactoEmergencia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    telefonoEmergencia = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    contrato = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    remuneracionExtra1 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    montoExtra1 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    remuneracionExtra2 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    montoExtra2 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    remuneracionExtra3 = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    montoExtra3 = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    tss = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    diasPago = table.Column<int>(type: "int", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    fechaSalida = table.Column<DateTime>(type: "datetime", nullable: true),
                    motivoBaja = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    prestaciones = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados", x => x.empleadoID);
                });

            migrationBuilder.CreateTable(
                name: "Empleados_Notas",
                columns: table => new
                {
                    notaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    empleadoID = table.Column<int>(type: "int", nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    nota = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    eliminada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados_Notas", x => x.notaID);
                });

            migrationBuilder.CreateTable(
                name: "Empleados_Temporales",
                columns: table => new
                {
                    contratacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    nombreComercial = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    rnc = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    nombreRepresentante = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    cedulaRepresentante = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    identificacion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    alias = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    direccion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    provincia = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    municipio = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    telefono1 = table.Column<string>(type: "varchar(18)", unicode: false, maxLength: 18, nullable: true),
                    telefono2 = table.Column<string>(type: "varchar(18)", unicode: false, maxLength: 18, nullable: true),
                    foto = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleados_Temporales", x => x.contratacionID);
                });

            migrationBuilder.CreateTable(
                name: "OpenAi_Config",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpenAIApiKey = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    OpenAIApiUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpenAi_Config", x => x.id);
                },
                comment: "Configuración del bot OpenAI para el 'abogado virtual'. ⚠️ Contiene información sensible.");

            migrationBuilder.CreateTable(
                name: "PasswordResetTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    token = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    expiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    usedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    createdAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    createdBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetTokens", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaymentGateway",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    test = table.Column<bool>(type: "bit", nullable: false),
                    productionURL = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    testURL = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: false),
                    merchantID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    terminalID = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaymentGateway", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Perfiles",
                columns: table => new
                {
                    perfilID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaCreacion = table.Column<DateTime>(type: "datetime", nullable: false),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    telefono1 = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    telefono2 = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    usuario = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfiles", x => x.perfilID);
                });

            migrationBuilder.CreateTable(
                name: "Permisos",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    atributos = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permisos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Planes_Contratistas",
                columns: table => new
                {
                    planID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombrePlan = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    precio = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes_Contratistas", x => x.planID);
                });

            migrationBuilder.CreateTable(
                name: "Planes_empleadores",
                columns: table => new
                {
                    planID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    empleados = table.Column<int>(type: "int", nullable: false),
                    historico = table.Column<int>(type: "int", nullable: false),
                    nomina = table.Column<bool>(type: "bit", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Planes_empleadores", x => x.planID);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    provinciaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    nombre = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.provinciaID);
                });

            migrationBuilder.CreateTable(
                name: "Remuneraciones",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    empleadoID = table.Column<int>(type: "int", nullable: false),
                    descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    monto = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remuneraciones", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    sectorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sector = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    codigo = table.Column<string>(type: "varchar(10)", unicode: false, maxLength: 10, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    orden = table.Column<int>(type: "int", nullable: false, defaultValue: 999),
                    grupo = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.sectorID);
                });

            migrationBuilder.CreateTable(
                name: "Servicios",
                columns: table => new
                {
                    servicioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    descripcion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    orden = table.Column<int>(type: "int", nullable: false, defaultValue: 999),
                    categoria = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    icono = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicios", x => x.servicioID);
                });

            migrationBuilder.CreateTable(
                name: "Ventas",
                columns: table => new
                {
                    ventaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    metodo = table.Column<int>(type: "int", nullable: false),
                    planID = table.Column<int>(type: "int", nullable: false),
                    precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    comentario = table.Column<string>(type: "varchar(max)", unicode: false, nullable: true),
                    idTransaccion = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    idempotencyKey = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    card = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    ip = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ventas", x => x.ventaID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Token = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Expires = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Revoked = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ReplacedByToken = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReasonRevoked = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RevokedByIp = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    calificacionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fecha = table.Column<DateTime>(type: "datetime", nullable: false),
                    userID = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    tipo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    identificacion = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    puntualidad = table.Column<int>(type: "int", nullable: false),
                    cumplimiento = table.Column<int>(type: "int", nullable: false),
                    conocimientos = table.Column<int>(type: "int", nullable: false),
                    recomendacion = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.calificacionID);
                    table.CheckConstraint("CK_Calificaciones_Conocimientos_Rango", "conocimientos >= 1 AND conocimientos <= 5");
                    table.CheckConstraint("CK_Calificaciones_Cumplimiento_Rango", "cumplimiento >= 1 AND cumplimiento <= 5");
                    table.CheckConstraint("CK_Calificaciones_Puntualidad_Rango", "puntualidad >= 1 AND puntualidad <= 5");
                    table.CheckConstraint("CK_Calificaciones_Recomendacion_Rango", "recomendacion >= 1 AND recomendacion <= 5");
                    table.ForeignKey(
                        name: "FK_Calificaciones_Credenciales_Empleador",
                        column: x => x.userID,
                        principalTable: "Credenciales",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contratistas",
                columns: table => new
                {
                    contratistaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaIngreso = table.Column<DateTime>(type: "datetime", nullable: true),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    titulo = table.Column<string>(type: "varchar(70)", unicode: false, maxLength: 70, nullable: true),
                    tipo = table.Column<int>(type: "int", nullable: false),
                    identificacion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Nombre = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    Apellido = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    sector = table.Column<string>(type: "varchar(40)", unicode: false, maxLength: 40, nullable: true),
                    experiencia = table.Column<int>(type: "int", nullable: true),
                    presentacion = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    telefono1 = table.Column<string>(type: "varchar(16)", unicode: false, maxLength: 16, nullable: true),
                    whatsapp1 = table.Column<bool>(type: "bit", nullable: false),
                    telefono2 = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    whatsapp2 = table.Column<bool>(type: "bit", nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false),
                    provincia = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    nivelNacional = table.Column<bool>(type: "bit", nullable: false),
                    imagenURL = table.Column<string>(type: "varchar(150)", unicode: false, maxLength: 150, nullable: true),
                    email = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratistas", x => x.contratistaID);
                    table.ForeignKey(
                        name: "FK_Contratistas_Credenciales",
                        column: x => x.userID,
                        principalTable: "Credenciales",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Ofertantes",
                columns: table => new
                {
                    ofertanteID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    fechaPublicacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    habilidades = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    experiencia = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    foto = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ofertantes", x => x.ofertanteID);
                    table.ForeignKey(
                        name: "FK_Ofertantes_Credenciales",
                        column: x => x.userID,
                        principalTable: "Credenciales",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empleador_Recibos_Header",
                columns: table => new
                {
                    pagoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    empleadoID = table.Column<int>(type: "int", nullable: false),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: false),
                    fechaPago = table.Column<DateTime>(type: "datetime", nullable: true),
                    conceptoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false),
                    tipo = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    estado = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    periodo_inicio = table.Column<DateOnly>(type: "date", nullable: true),
                    periodo_fin = table.Column<DateOnly>(type: "date", nullable: true),
                    total_ingresos = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    total_deducciones = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    neto_pagar = table.Column<decimal>(type: "decimal(12,2)", nullable: false, defaultValue: 0m),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleador_Recibos_Header", x => x.pagoID);
                    table.ForeignKey(
                        name: "FK_Empleador_Recibos_Header_Empleados",
                        column: x => x.empleadoID,
                        principalTable: "Empleados",
                        principalColumn: "empleadoID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Detalle_Contrataciones",
                columns: table => new
                {
                    detalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contratacionID = table.Column<int>(type: "int", nullable: true),
                    descripcionCorta = table.Column<string>(type: "varchar(60)", unicode: false, maxLength: 60, nullable: false),
                    descripcionAmpliada = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    fechaInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    fechaFinal = table.Column<DateOnly>(type: "date", nullable: false),
                    montoAcordado = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    esquemaPagos = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    estatus = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    calificado = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    calificacionID = table.Column<int>(type: "int", nullable: true),
                    notas = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    motivo_cancelacion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    fecha_inicio_real = table.Column<DateTime>(type: "datetime", nullable: true),
                    fecha_finalizacion_real = table.Column<DateTime>(type: "datetime", nullable: true),
                    porcentaje_avance = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detalle_Contrataciones", x => x.detalleID);
                    table.ForeignKey(
                        name: "FK_DetalleContrataciones_EmpleadosTemporales",
                        column: x => x.contratacionID,
                        principalTable: "Empleados_Temporales",
                        principalColumn: "contratacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Empleador_Recibos_Header_Contrataciones",
                columns: table => new
                {
                    pagoID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    contratacionID = table.Column<int>(type: "int", nullable: true),
                    fechaRegistro = table.Column<DateTime>(type: "datetime", nullable: true),
                    fechaPago = table.Column<DateTime>(type: "datetime", nullable: true),
                    conceptoPago = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    tipo = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleador_Recibos_Header_Contrataciones", x => x.pagoID);
                    table.ForeignKey(
                        name: "FK_Empleador_Recibos_Header_Contrataciones_EmpleadosTemporales",
                        column: x => x.contratacionID,
                        principalTable: "Empleados_Temporales",
                        principalColumn: "contratacionID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "perfilesInfo",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    perfilID = table.Column<int>(type: "int", nullable: true),
                    userID = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    tipoIdentificacion = table.Column<int>(type: "int", nullable: true),
                    nombreComercial = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    identificacion = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    direccion = table.Column<string>(type: "text", nullable: true),
                    fotoPerfil = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    presentacion = table.Column<string>(type: "text", nullable: true),
                    cedulaGerente = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: true),
                    nombreGerente = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    apellidoGerente = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    direccionGerente = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(450)", unicode: false, maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_perfilesInfo", x => x.id);
                    table.ForeignKey(
                        name: "FK_perfilesInfo_Perfiles",
                        column: x => x.perfilID,
                        principalTable: "Perfiles",
                        principalColumn: "perfilID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Suscripciones",
                columns: table => new
                {
                    suscripcionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    userID = table.Column<string>(type: "varchar(250)", maxLength: 250, nullable: false),
                    planID = table.Column<int>(type: "int", nullable: false),
                    vencimiento = table.Column<DateTime>(type: "datetime", nullable: false),
                    fechaInicio = table.Column<DateTime>(type: "datetime", nullable: false),
                    cancelada = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    fechaCancelacion = table.Column<DateTime>(type: "datetime", nullable: true),
                    razonCancelacion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suscripciones", x => x.suscripcionID);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Credenciales",
                        column: x => x.userID,
                        principalTable: "Credenciales",
                        principalColumn: "userID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Suscripciones_Planes_empleadores",
                        column: x => x.planID,
                        principalTable: "Planes_empleadores",
                        principalColumn: "planID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contratistas_Fotos",
                columns: table => new
                {
                    imagenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contratistaID = table.Column<int>(type: "int", nullable: false),
                    imagenURL = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    tipo_foto = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: true),
                    descripcion = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    orden = table.Column<int>(type: "int", nullable: false, defaultValue: 999),
                    activa = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    es_principal = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    tags = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    fecha_trabajo = table.Column<DateTime>(type: "datetime", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratistas_Fotos", x => x.imagenID);
                    table.ForeignKey(
                        name: "FK_Contratistas_Fotos_Contratistas",
                        column: x => x.contratistaID,
                        principalTable: "Contratistas",
                        principalColumn: "contratistaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contratistas_Servicios",
                columns: table => new
                {
                    servicioID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    contratistaID = table.Column<int>(type: "int", nullable: false),
                    detalleServicio = table.Column<string>(type: "varchar(250)", unicode: false, maxLength: 250, nullable: false),
                    activo = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    anios_experiencia = table.Column<int>(type: "int", nullable: true),
                    tarifa_base = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    orden = table.Column<int>(type: "int", nullable: false, defaultValue: 999),
                    certificaciones = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contratistas_Servicios", x => x.servicioID);
                    table.ForeignKey(
                        name: "FK_Contratistas_Servicios_Contratistas",
                        column: x => x.contratistaID,
                        principalTable: "Contratistas",
                        principalColumn: "contratistaID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empleador_Recibos_Detalle",
                columns: table => new
                {
                    detalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pagoID = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: false),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    tipo_concepto = table.Column<int>(type: "int", nullable: false, defaultValue: 1),
                    orden = table.Column<int>(type: "int", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    created_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true),
                    updated_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    updated_by = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleador_Recibos_Detalle", x => x.detalleID);
                    table.ForeignKey(
                        name: "FK_Empleador_Recibos_Detalle_Empleador_Recibos_Header_pagoID",
                        column: x => x.pagoID,
                        principalTable: "Empleador_Recibos_Header",
                        principalColumn: "pagoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Empleador_Recibos_Detalle_Contrataciones",
                columns: table => new
                {
                    detalleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    pagoID = table.Column<int>(type: "int", nullable: true),
                    Concepto = table.Column<string>(type: "varchar(90)", unicode: false, maxLength: 90, nullable: true),
                    Monto = table.Column<decimal>(type: "decimal(10,2)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Empleador_Recibos_Detalle_Contrataciones", x => x.detalleID);
                    table.ForeignKey(
                        name: "FK_Empleador_Recibos_Detalle_Contrataciones_Empleador_Recibos_Header_Contrataciones",
                        column: x => x.pagoID,
                        principalTable: "Empleador_Recibos_Header_Contrataciones",
                        principalColumn: "pagoID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_Contratista_Fecha",
                table: "Calificaciones",
                columns: new[] { "identificacion", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_ContratistaIdentificacion",
                table: "Calificaciones",
                column: "identificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_Empleador_Fecha",
                table: "Calificaciones",
                columns: new[] { "userID", "fecha" });

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_EmpleadorUserId",
                table: "Calificaciones",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_Fecha",
                table: "Calificaciones",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_Tipo",
                table: "Calificaciones",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigCorreo_Email",
                table: "Config_Correo",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_ConfigCorreo_Servidor",
                table: "Config_Correo",
                column: "servidor");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPerfil_Composite",
                table: "ConsultasPerfil",
                columns: new[] { "empleadorUserId", "contratistaIdentificacion", "fechaConsulta" });

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPerfil_ContratistaIdentificacion",
                table: "ConsultasPerfil",
                column: "contratistaIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPerfil_EmpleadorUserId",
                table: "ConsultasPerfil",
                column: "empleadorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ConsultasPerfil_FechaConsulta",
                table: "ConsultasPerfil",
                column: "fechaConsulta");

            migrationBuilder.CreateIndex(
                name: "IX_Contratistas_Activo",
                table: "Contratistas",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_Contratistas_FechaIngreso",
                table: "Contratistas",
                column: "fechaIngreso");

            migrationBuilder.CreateIndex(
                name: "IX_Contratistas_Provincia",
                table: "Contratistas",
                column: "provincia");

            migrationBuilder.CreateIndex(
                name: "IX_Contratistas_Sector_Provincia",
                table: "Contratistas",
                columns: new[] { "sector", "provincia" });

            migrationBuilder.CreateIndex(
                name: "IX_Contratistas_UserID",
                table: "Contratistas",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasFotos_Activa",
                table: "Contratistas_Fotos",
                column: "activa");

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasFotos_Contratista_Activa_Orden",
                table: "Contratistas_Fotos",
                columns: new[] { "contratistaID", "activa", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasFotos_Contratista_Principal",
                table: "Contratistas_Fotos",
                columns: new[] { "contratistaID", "es_principal" },
                unique: true,
                filter: "[es_principal] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasFotos_ContratistaId",
                table: "Contratistas_Fotos",
                column: "contratistaID");

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasFotos_EsPrincipal",
                table: "Contratistas_Fotos",
                column: "es_principal");

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasServicios_Activo",
                table: "Contratistas_Servicios",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasServicios_Contratista_Activo_Orden",
                table: "Contratistas_Servicios",
                columns: new[] { "contratistaID", "activo", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_ContratistasServicios_ContratistaId",
                table: "Contratistas_Servicios",
                column: "contratistaID");

            migrationBuilder.CreateIndex(
                name: "IX_Credenciales_Activo",
                table: "Credenciales",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_Credenciales_Email",
                table: "Credenciales",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Credenciales_UserID",
                table: "Credenciales",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeduccionesTss_Activa",
                table: "Deducciones_TSS",
                column: "activa");

            migrationBuilder.CreateIndex(
                name: "IX_DeduccionesTss_Descripcion",
                table: "Deducciones_TSS",
                column: "descripcion");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_CalificacionId",
                table: "Detalle_Contrataciones",
                column: "calificacionID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_Calificado",
                table: "Detalle_Contrataciones",
                column: "calificado");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_ContratacionId",
                table: "Detalle_Contrataciones",
                column: "contratacionID");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_Estatus",
                table: "Detalle_Contrataciones",
                column: "estatus");

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_Estatus_Calificado",
                table: "Detalle_Contrataciones",
                columns: new[] { "estatus", "calificado" });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_Estatus_FechaInicio",
                table: "Detalle_Contrataciones",
                columns: new[] { "estatus", "fechaInicio" });

            migrationBuilder.CreateIndex(
                name: "IX_DetalleContrataciones_Fechas",
                table: "Detalle_Contrataciones",
                columns: new[] { "fechaInicio", "fechaFinal" });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboDetalle_PagoId",
                table: "Empleador_Recibos_Detalle",
                column: "pagoID");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboDetalle_PagoId_Orden",
                table: "Empleador_Recibos_Detalle",
                columns: new[] { "pagoID", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboDetalle_TipoConcepto",
                table: "Empleador_Recibos_Detalle",
                column: "tipo_concepto");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosDetalle_Monto",
                table: "Empleador_Recibos_Detalle_Contrataciones",
                column: "Monto");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosDetalle_PagoId",
                table: "Empleador_Recibos_Detalle_Contrataciones",
                column: "pagoID");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_EmpleadoId",
                table: "Empleador_Recibos_Header",
                column: "empleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_EmpleadoId_FechaRegistro",
                table: "Empleador_Recibos_Header",
                columns: new[] { "empleadoID", "fechaRegistro" });

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_Estado",
                table: "Empleador_Recibos_Header",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_FechaPago",
                table: "Empleador_Recibos_Header",
                column: "fechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_FechaRegistro",
                table: "Empleador_Recibos_Header",
                column: "fechaRegistro");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_UserId",
                table: "Empleador_Recibos_Header",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_ReciboHeader_UserId_Estado",
                table: "Empleador_Recibos_Header",
                columns: new[] { "userID", "estado" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosHeader_ContratacionId",
                table: "Empleador_Recibos_Header_Contrataciones",
                column: "contratacionID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosHeader_FechaPago",
                table: "Empleador_Recibos_Header_Contrataciones",
                column: "fechaPago");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosHeader_UserId",
                table: "Empleador_Recibos_Header_Contrataciones",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadorRecibosHeader_UserId_FechaPago",
                table: "Empleador_Recibos_Header_Contrataciones",
                columns: new[] { "userID", "fechaPago" });

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Activo",
                table: "Empleados",
                column: "Activo");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_Identificacion",
                table: "Empleados",
                column: "identificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_UserId",
                table: "Empleados",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Empleados_UserId_Activo",
                table: "Empleados",
                columns: new[] { "userID", "Activo" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosNotas_Eliminada",
                table: "Empleados_Notas",
                column: "eliminada");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosNotas_EmpleadoId",
                table: "Empleados_Notas",
                column: "empleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosNotas_EmpleadoId_Eliminada",
                table: "Empleados_Notas",
                columns: new[] { "empleadoID", "eliminada" });

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosNotas_Fecha",
                table: "Empleados_Notas",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosNotas_UserId",
                table: "Empleados_Notas",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_Activo",
                table: "Empleados_Temporales",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_Identificacion",
                table: "Empleados_Temporales",
                column: "identificacion");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_Rnc",
                table: "Empleados_Temporales",
                column: "rnc");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_Tipo",
                table: "Empleados_Temporales",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_UserId",
                table: "Empleados_Temporales",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_EmpleadosTemporales_UserId_Activo",
                table: "Empleados_Temporales",
                columns: new[] { "userID", "activo" });

            migrationBuilder.CreateIndex(
                name: "IX_Ofertantes_FechaPublicacion",
                table: "Ofertantes",
                column: "fechaPublicacion");

            migrationBuilder.CreateIndex(
                name: "IX_Ofertantes_UserID",
                table: "Ofertantes",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Email",
                table: "PasswordResetTokens",
                column: "email");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_Token",
                table: "PasswordResetTokens",
                column: "token");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetTokens_UserId",
                table: "PasswordResetTokens",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateway_Activa",
                table: "PaymentGateway",
                column: "activa");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateway_MerchantId",
                table: "PaymentGateway",
                column: "merchantID");

            migrationBuilder.CreateIndex(
                name: "IX_PaymentGateway_ModoTest",
                table: "PaymentGateway",
                column: "test");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_Email",
                table: "Perfiles",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_Tipo",
                table: "Perfiles",
                column: "Tipo");

            migrationBuilder.CreateIndex(
                name: "IX_Perfiles_Tipo_FechaCreacion",
                table: "Perfiles",
                columns: new[] { "Tipo", "fechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "UX_Perfiles_UserId",
                table: "Perfiles",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesInfo_Identificacion",
                table: "perfilesInfo",
                column: "identificacion");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesInfo_PerfilId",
                table: "perfilesInfo",
                column: "perfilID");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesInfo_TipoIdentificacion",
                table: "perfilesInfo",
                column: "tipoIdentificacion");

            migrationBuilder.CreateIndex(
                name: "IX_PerfilesInfo_UserId_Identificacion",
                table: "perfilesInfo",
                columns: new[] { "userID", "identificacion" });

            migrationBuilder.CreateIndex(
                name: "UX_PerfilesInfo_UserId",
                table: "perfilesInfo",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permisos_Atributos",
                table: "Permisos",
                column: "atributos");

            migrationBuilder.CreateIndex(
                name: "UX_Permisos_UserId",
                table: "Permisos",
                column: "userID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlanesContratistas_Activo",
                table: "Planes_Contratistas",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesContratistas_NombrePlan",
                table: "Planes_Contratistas",
                column: "nombrePlan");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesContratistas_Precio",
                table: "Planes_Contratistas",
                column: "precio");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesEmpleadores_Activo",
                table: "Planes_empleadores",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesEmpleadores_Nombre",
                table: "Planes_empleadores",
                column: "nombre");

            migrationBuilder.CreateIndex(
                name: "IX_PlanesEmpleadores_Precio",
                table: "Planes_empleadores",
                column: "precio");

            migrationBuilder.CreateIndex(
                name: "UX_Provincias_Nombre",
                table: "Provincias",
                column: "nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_Token",
                table: "RefreshTokens",
                column: "Token",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Remuneraciones_EmpleadoId",
                table: "Remuneraciones",
                column: "empleadoID");

            migrationBuilder.CreateIndex(
                name: "IX_Remuneraciones_UserId",
                table: "Remuneraciones",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_Activo",
                table: "Sectores",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_Codigo",
                table: "Sectores",
                column: "codigo");

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_Grupo_Orden",
                table: "Sectores",
                columns: new[] { "grupo", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_Nombre",
                table: "Sectores",
                column: "sector");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Activo",
                table: "Servicios",
                column: "activo");

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Categoria_Orden",
                table: "Servicios",
                columns: new[] { "categoria", "orden" });

            migrationBuilder.CreateIndex(
                name: "IX_Servicios_Descripcion",
                table: "Servicios",
                column: "descripcion");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_Cancelada",
                table: "Suscripciones",
                column: "cancelada");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_Cancelada_Vencimiento",
                table: "Suscripciones",
                columns: new[] { "cancelada", "vencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_PlanId",
                table: "Suscripciones",
                column: "planID");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_UserId",
                table: "Suscripciones",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_UserId_Vencimiento",
                table: "Suscripciones",
                columns: new[] { "userID", "vencimiento" });

            migrationBuilder.CreateIndex(
                name: "IX_Suscripciones_Vencimiento",
                table: "Suscripciones",
                column: "vencimiento");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_Estado",
                table: "Ventas",
                column: "estado");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_FechaTransaccion",
                table: "Ventas",
                column: "fecha");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdempotencyKey",
                table: "Ventas",
                column: "idempotencyKey",
                unique: true,
                filter: "[idempotencyKey] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_IdTransaccion",
                table: "Ventas",
                column: "idTransaccion");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_PlanId",
                table: "Ventas",
                column: "planID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UserId",
                table: "Ventas",
                column: "userID");

            migrationBuilder.CreateIndex(
                name: "IX_Ventas_UserId_Fecha",
                table: "Ventas",
                columns: new[] { "userID", "fecha" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Calificaciones");

            migrationBuilder.DropTable(
                name: "Config_Correo");

            migrationBuilder.DropTable(
                name: "ConsultasPerfil");

            migrationBuilder.DropTable(
                name: "Contratistas_Fotos");

            migrationBuilder.DropTable(
                name: "Contratistas_Servicios");

            migrationBuilder.DropTable(
                name: "Deducciones_TSS");

            migrationBuilder.DropTable(
                name: "Detalle_Contrataciones");

            migrationBuilder.DropTable(
                name: "Empleador_Recibos_Detalle");

            migrationBuilder.DropTable(
                name: "Empleador_Recibos_Detalle_Contrataciones");

            migrationBuilder.DropTable(
                name: "Empleados_Notas");

            migrationBuilder.DropTable(
                name: "Ofertantes");

            migrationBuilder.DropTable(
                name: "OpenAi_Config");

            migrationBuilder.DropTable(
                name: "PasswordResetTokens");

            migrationBuilder.DropTable(
                name: "PaymentGateway");

            migrationBuilder.DropTable(
                name: "perfilesInfo");

            migrationBuilder.DropTable(
                name: "Permisos");

            migrationBuilder.DropTable(
                name: "Planes_Contratistas");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Remuneraciones");

            migrationBuilder.DropTable(
                name: "Sectores");

            migrationBuilder.DropTable(
                name: "Servicios");

            migrationBuilder.DropTable(
                name: "Suscripciones");

            migrationBuilder.DropTable(
                name: "Ventas");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Contratistas");

            migrationBuilder.DropTable(
                name: "Empleador_Recibos_Header");

            migrationBuilder.DropTable(
                name: "Empleador_Recibos_Header_Contrataciones");

            migrationBuilder.DropTable(
                name: "Perfiles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Planes_empleadores");

            migrationBuilder.DropTable(
                name: "Credenciales");

            migrationBuilder.DropTable(
                name: "Empleados");

            migrationBuilder.DropTable(
                name: "Empleados_Temporales");
        }
    }
}
