using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeder de catalogos base para entornos productivos.
/// </summary>
public class CatalogDatabaseSeeder
{
    private readonly MiGenteDbContext _context;
    private readonly ILogger<CatalogDatabaseSeeder> _logger;

    public CatalogDatabaseSeeder(MiGenteDbContext context, ILogger<CatalogDatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Iniciando seed de catalogos...");

        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            await RunBlockAsync("PlanesEmpleadores", SeedPlanesEmpleadoresAsync);
            await RunBlockAsync("PlanesContratistas", SeedPlanesContratistasAsync);
            await RunBlockAsync("Provincias", SeedProvinciasAsync);
            await RunBlockAsync("Sectores", SeedSectoresAsync);
            await RunBlockAsync("Servicios", SeedServiciosAsync);
            await RunBlockAsync("MissingEmpleadores", SeedMissingEmpleadoresAsync);

            await transaction.CommitAsync();
            _logger.LogInformation("Seed de catalogos completado.");
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task RunBlockAsync(string blockName, Func<Task> action)
    {
        _logger.LogInformation("Seed block {Block} iniciado", blockName);
        await action();
        _logger.LogInformation("Seed block {Block} completado", blockName);
    }

    private async Task SeedPlanesEmpleadoresAsync()
    {
        if (await _context.PlanesEmpleadores.AnyAsync())
        {
            return;
        }

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Planes_empleadores ON;

            INSERT INTO Planes_empleadores (planID, nombre, precio, empleados, historico, nomina)
            VALUES
                (1, 'Mi Gente, Soy Yo', 495.00, 1, 12, 0),
                (2, 'Mi Gente en Familia', 1695.00, 5, 12, 0),
                (3, 'Mi Gente Somos Todos', 3750.00, 15, 12, 1);

            SET IDENTITY_INSERT Planes_empleadores OFF;
        ");
    }

    private async Task SeedPlanesContratistasAsync()
    {
        if (await _context.PlanesContratistas.AnyAsync())
        {
            return;
        }

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Planes_Contratistas ON;

            INSERT INTO Planes_Contratistas (planID, nombrePlan, precio)
            VALUES (4, 'Plan Ofertantes', 499.00);

            SET IDENTITY_INSERT Planes_Contratistas OFF;
        ");
    }

    private async Task SeedProvinciasAsync()
    {
        if (await _context.Provincias.AnyAsync())
        {
            return;
        }

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
    }

    private async Task SeedSectoresAsync()
    {
        if (await _context.Sectores.AnyAsync())
        {
            return;
        }

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
    }

    private async Task SeedServiciosAsync()
    {
        if (await _context.Servicios.AnyAsync())
        {
            return;
        }

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Servicios ON;

            INSERT INTO Servicios (servicioID, descripcion, userID) VALUES
                (1, 'Plomería', NULL),
                (2, 'Electricidad', NULL),
                (3, 'Carpintería', NULL),
                (4, 'Pintura', NULL),
                (5, 'Albañilería', NULL),
                (6, 'Jardinería', NULL),
                (7, 'Limpieza Residencial', NULL),
                (8, 'Limpieza Comercial', NULL),
                (9, 'Mecánica Automotriz', NULL),
                (10, 'Aire Acondicionado', NULL),
                (11, 'Refrigeración', NULL),
                (12, 'Herrería', NULL),
                (13, 'Cerrajería', NULL),
                (14, 'Techado', NULL),
                (15, 'Instalación de Pisos', NULL),
                (16, 'Instalación de Vidrios', NULL),
                (17, 'Mudanzas', NULL),
                (18, 'Transporte', NULL),
                (19, 'Cuidado de Niños', NULL),
                (20, 'Cuidado de Adultos Mayores', NULL),
                (21, 'Cocina/Chef', NULL),
                (22, 'Repostería', NULL),
                (23, 'Peluquería', NULL),
                (24, 'Barbería', NULL),
                (25, 'Estética', NULL),
                (26, 'Masajes', NULL),
                (27, 'Entrenador Personal', NULL),
                (28, 'Clases Particulares', NULL),
                (29, 'Traducción', NULL),
                (30, 'Diseño Gráfico', NULL),
                (31, 'Fotografía', NULL),
                (32, 'Videografía', NULL),
                (33, 'Desarrollo Web', NULL),
                (34, 'Reparación de Computadoras', NULL),
                (35, 'Reparación de Celulares', NULL),
                (36, 'Asesoría Legal', NULL),
                (37, 'Asesoría Contable', NULL),
                (38, 'Asesoría Financiera', NULL),
                (39, 'Marketing Digital', NULL),
                (40, 'Redes Sociales', NULL);

            SET IDENTITY_INSERT Servicios OFF;
        ");
    }

    private async Task SeedMissingEmpleadoresAsync()
    {
        await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Ofertantes (userID, fechaPublicacion, descripcion)
            SELECT p.userID, GETUTCDATE(), CONCAT('Empleador: ', p.nombre, ' ', p.apellido)
            FROM Perfiles p
            LEFT JOIN Ofertantes o ON p.userID = o.userID
            WHERE p.tipo = 1 AND o.ofertanteID IS NULL;
        ");
    }
}
