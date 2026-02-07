using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiGenteEnLinea.Infrastructure.Persistence.Migrations;

/// <summary>
/// Migración para eliminar la FK_Suscripciones_Planes_empleadores.
/// 
/// RAZÓN:
/// La tabla Suscripciones.planID puede referenciar IDs de dos tablas diferentes:
/// - Planes_empleadores (para empleadores)
/// - Planes_contratistas (para contratistas)
/// 
/// La FK original solo permitía referencias a Planes_empleadores, causando errores
/// cuando un contratista intentaba adquirir un plan (planID de Planes_contratistas).
/// 
/// SOLUCIÓN:
/// Eliminar la FK constraint y validar la existencia del plan en la capa de aplicación
/// (ProcesarVentaCommandHandler) antes de crear la suscripción.
/// 
/// FECHA: 2026-02-07
/// ISSUE: FK constraint error al procesar pago de Plan Ofertantes (ID 4 para Contratistas)
/// </summary>
public partial class RemoveSuscripcionesPlanFK : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Eliminar la FK constraint que impide que Contratistas adquieran planes
        migrationBuilder.DropForeignKey(
            name: "FK_Suscripciones_Planes_empleadores",
            table: "Suscripciones");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        // Restaurar la FK constraint solo hacia Planes_empleadores
        // NOTA: Esto causará errores si existen suscripciones con planID de Planes_contratistas
        migrationBuilder.AddForeignKey(
            name: "FK_Suscripciones_Planes_empleadores",
            table: "Suscripciones",
            column: "planID",
            principalTable: "Planes_empleadores",
            principalColumn: "planID",
            onDelete: ReferentialAction.Restrict);
    }
}
