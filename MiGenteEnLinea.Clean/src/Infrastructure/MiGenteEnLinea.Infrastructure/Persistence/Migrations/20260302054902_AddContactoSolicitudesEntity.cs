using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiGenteEnLinea.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddContactoSolicitudesEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID('dbo.ContactoSolicitudes', 'U') IS NULL
                BEGIN
                    CREATE TABLE dbo.ContactoSolicitudes
                    (
                        solicitudId INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
                        contratistaUserId NVARCHAR(100) NOT NULL,
                        empleadorId INT NOT NULL,
                        mensaje NVARCHAR(500) NULL,
                        canalPreferido NVARCHAR(20) NULL,
                        estatus NVARCHAR(20) NOT NULL,
                        createdAt DATETIME NULL,
                        createdBy NVARCHAR(100) NULL,
                        updatedAt DATETIME NULL,
                        updatedBy NVARCHAR(100) NULL
                    );
                END
                """);

            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1
                    FROM sys.indexes
                    WHERE name = 'IX_ContactoSolicitudes_Contratista_Empleador_Estatus'
                      AND object_id = OBJECT_ID('dbo.ContactoSolicitudes')
                )
                BEGIN
                    CREATE INDEX IX_ContactoSolicitudes_Contratista_Empleador_Estatus
                        ON dbo.ContactoSolicitudes (contratistaUserId, empleadorId, estatus);
                END
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF OBJECT_ID('dbo.ContactoSolicitudes', 'U') IS NOT NULL
                BEGIN
                    DROP TABLE dbo.ContactoSolicitudes;
                END
                """);
        }
    }
}
