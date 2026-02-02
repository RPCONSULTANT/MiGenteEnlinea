using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiGenteEnLinea.Domain.Entities.Suscripciones;

namespace MiGenteEnLinea.Infrastructure.Persistence.Configurations;

/// <summary>
/// Configuración de Entity Framework Core para la entidad Suscripcion
/// 
/// MAPEO CON LEGACY:
/// - Tabla destino: "Suscripciones" (nombre original de la base de datos)
/// - Columnas mapeadas a nombres legacy españoles
/// - PK: suscripcionID
/// - FKs: userID (a Credenciales), planID (a Planes_empleadores o Planes_Contratistas)
/// 
/// CONVERSIONES:
/// - DateOnly (domain) ↔ DateTime (database)
/// - Propiedades calculadas (EstaActiva, DiasRestantes) no se mapean
/// 
/// ÍNDICES ESTRATÉGICOS:
/// - IX_Suscripciones_UserId: Búsqueda de suscripciones por usuario
/// - IX_Suscripciones_Vencimiento: Detección de suscripciones vencidas
/// - IX_Suscripciones_PlanId: Reporting por tipo de plan
/// - IX_Suscripciones_Cancelada: Filtrado de cancelaciones
/// </summary>
public class SuscripcionConfiguration : IEntityTypeConfiguration<Suscripcion>
{
    public void Configure(EntityTypeBuilder<Suscripcion> builder)
    {
        // ========================================
        // CONFIGURACIÓN DE TABLA
        // ========================================
        builder.ToTable("Suscripciones");

        // ========================================
        // CLAVE PRIMARIA
        // ========================================
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("suscripcionID")
            .ValueGeneratedOnAdd()
            .IsRequired();

        // ========================================
        // PROPIEDADES BÁSICAS
        // ========================================

        // UserId - FK a Credenciales
        builder.Property(s => s.UserId)
            .HasColumnName("userID")
            .HasMaxLength(250) // Debe coincidir con Credenciales.userID
            .IsRequired();

        // PlanId - FK a Planes_empleadores o Planes_Contratistas
        builder.Property(s => s.PlanId)
            .HasColumnName("planID")
            .IsRequired();

        // Vencimiento - DateOnly → DateTime
        builder.Property(s => s.Vencimiento)
            .HasColumnName("vencimiento")
            .HasConversion(
                dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue), // DateOnly → DateTime
                dateTime => DateOnly.FromDateTime(dateTime))        // DateTime → DateOnly
            .HasColumnType("datetime")
            .IsRequired();

        // FechaInicio - Nueva columna para auditoría
        builder.Property(s => s.FechaInicio)
            .HasColumnName("fechaInicio")
            .HasColumnType("datetime")
            .IsRequired();

        // Cancelada - Nueva columna
        builder.Property(s => s.Cancelada)
            .HasColumnName("cancelada")
            .HasDefaultValue(false)
            .IsRequired();

        // FechaCancelacion - Nueva columna opcional
        builder.Property(s => s.FechaCancelacion)
            .HasColumnName("fechaCancelacion")
            .HasColumnType("datetime")
            .IsRequired(false);

        // RazonCancelacion - Nueva columna opcional
        builder.Property(s => s.RazonCancelacion)
            .HasColumnName("razonCancelacion")
            .HasMaxLength(500)
            .IsRequired(false);

        // ========================================
        // RELACIONES (FKs)
        // ========================================

        // FK: Suscripcion -> Credencial (UserId)
        // IMPORTANTE: FK apunta a Credencial.UserId (string), NO a Credencial.Id (int)
        builder.HasOne<Domain.Entities.Authentication.Credencial>()
            .WithMany()
            .HasForeignKey(s => s.UserId)
            .HasPrincipalKey(cr => cr.UserId) // Especifica UserId como clave principal
            .HasConstraintName("FK_Suscripciones_Credenciales")
            .OnDelete(DeleteBehavior.Cascade); // Si se elimina el usuario, se eliminan sus suscripciones

        // ⚠️ NOTA: PlanId puede referirse a Planes_empleadores O Planes_Contratistas
        // La base de datos legacy tiene FK hacia Planes_empleadores, pero esto causa problemas
        // cuando un contratista adquiere un plan (PlanId de Planes_Contratistas no existe en Planes_empleadores).
        // 
        // SOLUCIÓN: No configuramos FK en EF Core. La validación de que el plan existe
        // se hace en ProcesarVentaCommandHandler antes de crear la suscripción.
        // 
        // El PlanId se almacena como un entero simple sin navegación:
        builder.Property(s => s.PlanId)
            .HasColumnName("planID")
            .IsRequired();
        
        // NO crear FK con HasOne<PlanEmpleador> ya que el PlanId puede venir de Planes_Contratistas

        // ========================================
        // ÍNDICES PARA OPTIMIZACIÓN
        // ========================================

        // Índice: Búsqueda por UserId (muy frecuente)
        builder.HasIndex(s => s.UserId)
            .HasDatabaseName("IX_Suscripciones_UserId")
            .IsUnique(false); // Un usuario podría tener historial de suscripciones

        // Índice: Búsqueda por Vencimiento (para detectar vencidas)
        builder.HasIndex(s => s.Vencimiento)
            .HasDatabaseName("IX_Suscripciones_Vencimiento")
            .IsUnique(false);

        // Índice: Búsqueda por PlanId (reporting)
        builder.HasIndex(s => s.PlanId)
            .HasDatabaseName("IX_Suscripciones_PlanId")
            .IsUnique(false);

        // Índice: Filtrado por Cancelada
        builder.HasIndex(s => s.Cancelada)
            .HasDatabaseName("IX_Suscripciones_Cancelada")
            .IsUnique(false);

        // Índice compuesto: UserId + Vencimiento (queries comunes)
        builder.HasIndex(s => new { s.UserId, s.Vencimiento })
            .HasDatabaseName("IX_Suscripciones_UserId_Vencimiento")
            .IsUnique(false);

        // Índice compuesto: Cancelada + Vencimiento (suscripciones activas)
        builder.HasIndex(s => new { s.Cancelada, s.Vencimiento })
            .HasDatabaseName("IX_Suscripciones_Cancelada_Vencimiento")
            .IsUnique(false);
    }
}
