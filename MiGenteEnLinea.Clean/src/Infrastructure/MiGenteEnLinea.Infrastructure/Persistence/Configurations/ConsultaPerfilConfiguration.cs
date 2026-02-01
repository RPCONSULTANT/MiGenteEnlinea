using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiGenteEnLinea.Domain.Entities.Consultas;

namespace MiGenteEnLinea.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración EF Core para la entidad ConsultaPerfil
/// Tabla: ConsultasPerfil (nueva tabla para tracking de visitas)
/// </summary>
public class ConsultaPerfilConfiguration : IEntityTypeConfiguration<ConsultaPerfil>
{
    public void Configure(EntityTypeBuilder<ConsultaPerfil> builder)
    {
        builder.ToTable("ConsultasPerfil");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Id)
            .HasColumnName("id")
            .ValueGeneratedOnAdd();

        builder.Property(c => c.EmpleadorUserId)
            .IsRequired()
            .HasMaxLength(450)
            .HasColumnName("empleadorUserId");

        builder.Property(c => c.ContratistaIdentificacion)
            .IsRequired()
            .HasMaxLength(20)
            .HasColumnName("contratistaIdentificacion");

        builder.Property(c => c.FechaConsulta)
            .IsRequired()
            .HasColumnName("fechaConsulta");

        builder.Property(c => c.IpAddress)
            .HasMaxLength(50)
            .HasColumnName("ipAddress");

        // Campos de auditoría (heredados de AuditableEntity)
        builder.Property(c => c.CreatedAt)
            .HasColumnName("createdAt");

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updatedAt");

        // Índices para optimizar consultas
        builder.HasIndex(c => c.EmpleadorUserId)
            .HasDatabaseName("IX_ConsultasPerfil_EmpleadorUserId");

        builder.HasIndex(c => c.ContratistaIdentificacion)
            .HasDatabaseName("IX_ConsultasPerfil_ContratistaIdentificacion");

        builder.HasIndex(c => c.FechaConsulta)
            .HasDatabaseName("IX_ConsultasPerfil_FechaConsulta");

        // Índice compuesto para evitar duplicados recientes
        builder.HasIndex(c => new { c.EmpleadorUserId, c.ContratistaIdentificacion, c.FechaConsulta })
            .HasDatabaseName("IX_ConsultasPerfil_Composite");
    }
}
