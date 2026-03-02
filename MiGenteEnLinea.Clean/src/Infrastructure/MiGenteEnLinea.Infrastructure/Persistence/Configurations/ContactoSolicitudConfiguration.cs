using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiGenteEnLinea.Domain.Entities.Contactos;

namespace MiGenteEnLinea.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración EF Core para solicitudes de contacto entre contratistas y empleadores.
/// </summary>
public sealed class ContactoSolicitudConfiguration : IEntityTypeConfiguration<ContactoSolicitud>
{
    public void Configure(EntityTypeBuilder<ContactoSolicitud> builder)
    {
        builder.ToTable("ContactoSolicitudes");

        builder.HasKey(x => x.SolicitudId);

        builder.Property(x => x.SolicitudId)
            .HasColumnName("solicitudId")
            .ValueGeneratedOnAdd();

        builder.Property(x => x.ContratistaUserId)
            .HasColumnName("contratistaUserId")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(x => x.EmpleadorId)
            .HasColumnName("empleadorId")
            .IsRequired();

        builder.Property(x => x.Mensaje)
            .HasColumnName("mensaje")
            .HasMaxLength(500)
            .IsRequired(false);

        builder.Property(x => x.CanalPreferido)
            .HasColumnName("canalPreferido")
            .HasMaxLength(20)
            .IsRequired(false);

        builder.Property(x => x.Estatus)
            .HasColumnName("estatus")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("createdAt")
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.Property(x => x.UpdatedAt)
            .HasColumnName("updatedAt")
            .HasColumnType("datetime")
            .IsRequired(false);

        builder.Property(x => x.CreatedBy)
            .HasColumnName("createdBy")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.Property(x => x.UpdatedBy)
            .HasColumnName("updatedBy")
            .HasMaxLength(100)
            .IsRequired(false);

        builder.HasIndex(x => new { x.ContratistaUserId, x.EmpleadorId, x.Estatus })
            .HasDatabaseName("IX_ContactoSolicitudes_Contratista_Empleador_Estatus");
    }
}
