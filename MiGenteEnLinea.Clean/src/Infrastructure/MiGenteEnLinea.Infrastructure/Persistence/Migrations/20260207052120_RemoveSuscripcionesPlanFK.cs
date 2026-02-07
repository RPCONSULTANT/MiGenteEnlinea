using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiGenteEnLinea.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSuscripcionesPlanFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Empleador_Recibos_Detalle_Empleador_Recibos_Header_pagoID",
                table: "Empleador_Recibos_Detalle");

            migrationBuilder.DropForeignKey(
                name: "FK_Suscripciones_Planes_empleadores",
                table: "Suscripciones");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddForeignKey(
                name: "FK_Empleador_Recibos_Detalle_Empleador_Recibos_Header_pagoID",
                table: "Empleador_Recibos_Detalle",
                column: "pagoID",
                principalTable: "Empleador_Recibos_Header",
                principalColumn: "pagoID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Suscripciones_Planes_empleadores",
                table: "Suscripciones",
                column: "planID",
                principalTable: "Planes_empleadores",
                principalColumn: "planID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
