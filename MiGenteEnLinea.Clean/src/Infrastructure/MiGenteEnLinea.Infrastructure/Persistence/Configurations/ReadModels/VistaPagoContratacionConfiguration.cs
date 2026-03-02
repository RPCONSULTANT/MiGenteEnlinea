using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiGenteEnLinea.Domain.ReadModels;

namespace MiGenteEnLinea.Infrastructure.Persistence.Configurations.ReadModels;

/// <summary>
/// Configuración de Entity Framework para VistaPagoContratacion
/// Mapea al read model con la vista legacy "VPagosContrataciones"
/// </summary>
public class VistaPagoContratacionConfiguration : IEntityTypeConfiguration<VistaPagoContratacion>
{
    public void Configure(EntityTypeBuilder<VistaPagoContratacion> builder)
    {
        builder.ToView("VPagosContrataciones");
        builder.HasNoKey();

        builder.Property(v => v.PagoId).HasColumnName("pagoID");
        builder.Property(v => v.UserId).HasColumnName("userID").IsUnicode(false);
        builder.Property(v => v.FechaRegistro).HasColumnName("fechaRegistro").HasColumnType("datetime");
        builder.Property(v => v.FechaPago).HasColumnName("fechaPago").HasColumnType("datetime");
        builder.Property(v => v.Expr1).HasMaxLength(18).IsUnicode(false);
        builder.Property(v => v.Monto).HasColumnType("decimal(38, 2)");
        builder.Property(v => v.ContratacionId).HasColumnName("contratacionID");
        builder.Property(v => v.DetalleId).HasColumnName("detalleID");
    }
}
