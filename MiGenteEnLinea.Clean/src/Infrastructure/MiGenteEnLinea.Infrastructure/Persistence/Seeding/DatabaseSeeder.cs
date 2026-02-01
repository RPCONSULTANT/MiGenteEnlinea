using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeder para poblar la base de datos con datos iniciales requeridos.
/// Ejecuta durante el inicio de la aplicación si las tablas están vacías.
/// </summary>
public class DatabaseSeeder
{
    private readonly MiGenteDbContext _context;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(MiGenteDbContext context, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Ejecuta el seeding de todas las tablas de catálogo.
    /// Solo inserta datos si las tablas están vacías.
    /// </summary>
    public async Task SeedAsync()
    {
        _logger.LogInformation("Iniciando seeding de base de datos...");

        try
        {
            await SeedPlanesEmpleadoresAsync();
            await SeedPlanesContratistasAsync();
            await SeedProvinciasAsync();
            await SeedSectoresAsync();
            await SeedMissingEmpleadoresAsync();

            _logger.LogInformation("✅ Seeding completado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error durante el seeding de la base de datos");
            throw;
        }
    }

    /// <summary>
    /// Seed de planes para empleadores
    /// </summary>
    private async Task SeedPlanesEmpleadoresAsync()
    {
        if (await _context.PlanesEmpleadores.AnyAsync())
        {
            _logger.LogInformation("Planes de empleadores ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando planes de empleadores...");

        // Ejecutar INSERT directo porque la entidad tiene constructor privado
        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Planes_empleadores ON;
            
            INSERT INTO Planes_empleadores (planID, nombre, precio, empleados, historico, nomina)
            VALUES 
                (1, 'Mi Gente, Soy Yo', 495.00, 1, 12, 0),
                (2, 'Mi Gente en Familia', 1695.00, 5, 12, 0),
                (3, 'Mi Gente Somos Todos', 3750.00, 15, 12, 1);
            
            SET IDENTITY_INSERT Planes_empleadores OFF;
        ");

        _logger.LogInformation("✅ 3 planes de empleadores insertados");
    }

    /// <summary>
    /// Seed de planes para contratistas
    /// </summary>
    private async Task SeedPlanesContratistasAsync()
    {
        if (await _context.PlanesContratistas.AnyAsync())
        {
            _logger.LogInformation("Planes de contratistas ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando planes de contratistas...");

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Planes_Contratistas ON;
            
            INSERT INTO Planes_Contratistas (planID, nombrePlan, precio)
            VALUES (4, 'Plan Ofertantes', 499.00);
            
            SET IDENTITY_INSERT Planes_Contratistas OFF;
        ");

        _logger.LogInformation("✅ 1 plan de contratistas insertado");
    }

    /// <summary>
    /// Seed de provincias de República Dominicana
    /// </summary>
    private async Task SeedProvinciasAsync()
    {
        if (await _context.Provincias.AnyAsync())
        {
            _logger.LogInformation("Provincias ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando provincias de República Dominicana...");

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Provincias ON;
            
            INSERT INTO Provincias (provinciaID, nombre) VALUES
                (0, 'Cualquier Ubicacion'),
                (1, 'Azua'),
                (2, 'Bahoruco'),
                (3, 'Barahona'),
                (4, 'Dajabón'),
                (5, 'Distrito Nacional'),
                (6, 'Duarte'),
                (7, 'Elías Piña'),
                (8, 'El Seibo'),
                (9, 'Espaillat'),
                (10, 'Hato Mayor'),
                (11, 'Hermanas Mirabal'),
                (12, 'Independencia'),
                (13, 'La Altagracia'),
                (14, 'La Romana'),
                (15, 'La Vega'),
                (16, 'María Trinidad Sánchez'),
                (17, 'Monseñor Nouel'),
                (18, 'Monte Cristi'),
                (19, 'Monte Plata'),
                (20, 'Pedernales'),
                (21, 'Peravia'),
                (22, 'Puerto Plata'),
                (23, 'Samaná'),
                (24, 'San Cristóbal'),
                (25, 'San José de Ocoa'),
                (26, 'San Juan'),
                (27, 'San Pedro de Macorís'),
                (28, 'Sánchez Ramírez'),
                (29, 'Santiago'),
                (30, 'Santiago Rodríguez'),
                (31, 'Valverde'),
                (32, 'Santo Domingo Este'),
                (33, 'Santo Domingo Oeste'),
                (34, 'Santo Domingo Norte');
            
            SET IDENTITY_INSERT Provincias OFF;
        ");

        _logger.LogInformation("✅ 35 provincias insertadas");
    }

    /// <summary>
    /// Seed de sectores económicos/profesionales
    /// </summary>
    private async Task SeedSectoresAsync()
    {
        if (await _context.Sectores.AnyAsync())
        {
            _logger.LogInformation("Sectores ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando sectores...");

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Sectores ON;
            
            INSERT INTO Sectores (sectorID, sector) VALUES
                (1, 'Medicina y Salud'),
                (2, 'Tecnología de la Información'),
                (3, 'Educación y Docencia'),
                (4, 'Finanzas y Contabilidad'),
                (5, 'Marketing y Publicidad'),
                (6, 'Diseño Gráfico y Multimedia'),
                (7, 'Arquitectura y Construcción'),
                (8, 'Ingeniería'),
                (9, 'Derecho y Asesoría Legal'),
                (10, 'Recursos Humanos y Gestión de Personal'),
                (11, 'Consultoría Empresarial'),
                (12, 'Comunicación y Medios de Comunicación'),
                (13, 'Turismo y Hostelería'),
                (14, 'Arte y Cultura'),
                (15, 'Agricultura y Agroindustria'),
                (16, 'Ciencia y Investigación'),
                (17, 'Desarrollo Sostenible y Medio Ambiente'),
                (18, 'Deportes y Actividad Física'),
                (19, 'Alimentación y Gastronomía'),
                (20, 'Belleza y Estética'),
                (21, 'Fotografía y Videografía'),
                (22, 'Entretenimiento y Eventos'),
                (23, 'Reparaciones y Mantenimiento'),
                (24, 'Jardinería y Paisajismo'),
                (25, 'Peluquería y Barbería'),
                (26, 'Transporte y Logística'),
                (27, 'Artesanía y Manualidades'),
                (28, 'Escritura y Redacción'),
                (29, 'Traducción e Interpretación'),
                (30, 'Programación y Desarrollo de Software'),
                (31, 'Soporte Técnico y Reparación de Equipos'),
                (32, 'Diseño Web y Desarrollo Frontend'),
                (33, 'Ingeniería de Software'),
                (34, 'Ciberseguridad'),
                (35, 'Análisis de Datos y Business Intelligence'),
                (36, 'Redes y Comunicaciones'),
                (37, 'Administración de Sistemas'),
                (38, 'Robótica y Automatización'),
                (39, 'Electrónica y Hardware'),
                (40, 'Audio y Producción Musical'),
                (41, 'Ebanistería');
            
            SET IDENTITY_INSERT Sectores OFF;
        ");

        _logger.LogInformation("✅ 41 sectores insertados");
    }

    /// <summary>
    /// Crea registros de Empleador para usuarios tipo 1 que no tienen uno.
    /// Esto corrige usuarios registrados antes de que se implementara esta funcionalidad.
    /// </summary>
    private async Task SeedMissingEmpleadoresAsync()
    {
        _logger.LogInformation("Verificando empleadores faltantes...");

        // Buscar usuarios tipo 1 (Empleador) que no tienen registro en Ofertantes
        var result = await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Ofertantes (userID, fechaPublicacion, descripcion)
            SELECT p.userID, GETUTCDATE(), CONCAT('Empleador: ', p.nombre, ' ', p.apellido)
            FROM Perfiles p
            LEFT JOIN Ofertantes o ON p.userID = o.userID
            WHERE p.tipo = 1 AND o.ofertanteID IS NULL;
        ");

        if (result > 0)
        {
            _logger.LogInformation("✅ {Count} registro(s) de empleador creados para usuarios existentes", result);
        }
        else
        {
            _logger.LogInformation("No hay empleadores faltantes que crear.");
        }
    }
}
