using System.Text.Json.Serialization;

namespace MiGenteEnLinea.Application.Features.Calificaciones.DTOs;

/// <summary>
/// DTO: Representa una calificación con 4 dimensiones de evaluación
/// Mapea desde: Domain.Entities.Calificaciones.Calificacion
/// 
/// DIMENSIONES DE EVALUACIÓN (Legacy):
/// - Puntualidad: ¿Llegó a tiempo el contratista?
/// - Cumplimiento: ¿Cumplió con lo acordado?
/// - Conocimientos: ¿Tenía las habilidades necesarias?
/// - Recomendación: ¿Lo recomendaría a otros empleadores?
/// 
/// Cada dimensión se califica de 1 a 5 estrellas.
/// El promedio general se calcula automáticamente.
/// </summary>
public class CalificacionDto
{
    /// <summary>
    /// ID de la calificación
    /// </summary>
    [JsonPropertyName("calificacionId")]
    public int CalificacionId { get; set; }

    /// <summary>
    /// ID del empleador que realizó la calificación
    /// </summary>
    [JsonPropertyName("empleadorUserId")]
    public string EmpleadorUserId { get; set; } = string.Empty;

    /// <summary>
    /// Identificación del contratista calificado (RNC o Cédula)
    /// </summary>
    [JsonPropertyName("contratistaIdentificacion")]
    public string ContratistaIdentificacion { get; set; } = string.Empty;

    /// <summary>
    /// Nombre completo del contratista calificado
    /// </summary>
    [JsonPropertyName("contratistaNombre")]
    public string ContratistaNombre { get; set; } = string.Empty;

    /// <summary>
    /// Calificación de puntualidad (1-5 estrellas)
    /// </summary>
    [JsonPropertyName("puntualidad")]
    public int Puntualidad { get; set; }

    /// <summary>
    /// Calificación de cumplimiento (1-5 estrellas)
    /// </summary>
    [JsonPropertyName("cumplimiento")]
    public int Cumplimiento { get; set; }

    /// <summary>
    /// Calificación de conocimientos (1-5 estrellas)
    /// </summary>
    [JsonPropertyName("conocimientos")]
    public int Conocimientos { get; set; }

    /// <summary>
    /// Calificación de recomendación (1-5 estrellas)
    /// </summary>
    [JsonPropertyName("recomendacion")]
    public int Recomendacion { get; set; }

    /// <summary>
    /// Promedio general de las 4 dimensiones (1-5)
    /// </summary>
    [JsonPropertyName("promedioGeneral")]
    public decimal PromedioGeneral { get; set; }

    /// <summary>
    /// Categoría de la calificación: "Excelente", "Buena", "Regular", "Mala"
    /// </summary>
    [JsonPropertyName("categoria")]
    public string Categoria { get; set; } = string.Empty;

    /// <summary>
    /// Fecha en que se realizó la calificación
    /// </summary>
    [JsonPropertyName("fecha")]
    public DateTime Fecha { get; set; }

    /// <summary>
    /// Indica si la calificación fue creada recientemente (últimas 24 horas)
    /// </summary>
    [JsonPropertyName("esReciente")]
    public bool EsReciente => (DateTime.Now - Fecha).TotalHours <= 24;

    /// <summary>
    /// Tiempo transcurrido desde la calificación (formato legible)
    /// Ej: "hace 2 horas", "hace 3 días"
    /// </summary>
    [JsonPropertyName("tiempoTranscurrido")]
    public string TiempoTranscurrido
    {
        get
        {
            var tiempo = DateTime.Now - Fecha;
            
            if (tiempo.TotalMinutes < 1)
                return "justo ahora";
            
            if (tiempo.TotalMinutes < 60)
                return $"hace {(int)tiempo.TotalMinutes} minuto{((int)tiempo.TotalMinutes > 1 ? "s" : "")}";
            
            if (tiempo.TotalHours < 24)
                return $"hace {(int)tiempo.TotalHours} hora{((int)tiempo.TotalHours > 1 ? "s" : "")}";
            
            if (tiempo.TotalDays < 30)
                return $"hace {(int)tiempo.TotalDays} día{((int)tiempo.TotalDays > 1 ? "s" : "")}";
            
            if (tiempo.TotalDays < 365)
            {
                int meses = (int)(tiempo.TotalDays / 30);
                return $"hace {meses} mes{(meses > 1 ? "es" : "")}";
            }
            
            int años = (int)(tiempo.TotalDays / 365);
            return $"hace {años} año{(años > 1 ? "s" : "")}";
        }
    }
}
