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
    private const string TestPasswordHash = "seed-hash";

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
            await SeedCatalogsAsync();
            await SeedMissingEmpleadoresAsync();

            if (ShouldSeedTestData())
            {
                await SeedTestDataAsync();
            }
            else
            {
                _logger.LogInformation("Omitiendo seed de datos de prueba en este entorno.");
            }

            _logger.LogInformation("✅ Seeding completado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error durante el seeding de la base de datos");
        }
    }

    private static bool ShouldSeedTestData()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
        var seedTestData = string.Equals(
            Environment.GetEnvironmentVariable("MIGENTE_SEED_TEST_DATA"),
            "true",
            StringComparison.OrdinalIgnoreCase);

        if (seedTestData)
        {
            return true;
        }

        return environment.Equals("Development", StringComparison.OrdinalIgnoreCase);
    }

    private async Task SeedCatalogsAsync()
    {
        await SeedPlanesEmpleadoresAsync();
        await SeedPlanesContratistasAsync();
        await SeedProvinciasAsync();
        await SeedSectoresAsync();
        await SeedServiciosAsync();
    }

    private async Task SeedTestDataAsync()
    {
        await SeedTestCredencialesAndPerfilesAsync();
        await SeedContratistasAsync();
        await SeedCalificacionesAsync();
    }

    private async Task SeedTestCredencialesAndPerfilesAsync()
    {
        _logger.LogInformation("Insertando credenciales y perfiles de prueba...");

        const string empleador1 = "00000000-0000-0000-0000-000000000101";
        const string empleador2 = "00000000-0000-0000-0000-000000000102";
        const string contratista1 = "00000000-0000-0000-0000-000000000201";
        const string contratista2 = "00000000-0000-0000-0000-000000000202";
        const string contratista3 = "00000000-0000-0000-0000-000000000203";
        const string contratista4 = "00000000-0000-0000-0000-000000000204";
        const string contratista5 = "00000000-0000-0000-0000-000000000205";
        const string contratista6 = "00000000-0000-0000-0000-000000000206";
        const string contratista7 = "00000000-0000-0000-0000-000000000207";
        const string contratista8 = "00000000-0000-0000-0000-000000000208";
        const string contratista9 = "00000000-0000-0000-0000-000000000209";
        const string contratista10 = "00000000-0000-0000-0000-000000000210";
        const string contratista11 = "00000000-0000-0000-0000-000000000211";
        const string contratista12 = "00000000-0000-0000-0000-000000000212";
        const string contratista13 = "00000000-0000-0000-0000-000000000213";
        const string contratista14 = "00000000-0000-0000-0000-000000000214";
        const string contratista15 = "00000000-0000-0000-0000-000000000215";

        var seedSql = $@"
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{empleador1}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{empleador1}', 'empleador1@example.com', '{TestPasswordHash}', 1, GETUTCDATE());

            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{empleador2}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{empleador2}', 'empleador2@example.com', '{TestPasswordHash}', 1, GETUTCDATE());

            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista1}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista1}', 'contratista1@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista2}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista2}', 'contratista2@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista3}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista3}', 'contratista3@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista4}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista4}', 'contratista4@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista5}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista5}', 'contratista5@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista6}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista6}', 'contratista6@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista7}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista7}', 'contratista7@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista8}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista8}', 'contratista8@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista9}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista9}', 'contratista9@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista10}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista10}', 'contratista10@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista11}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista11}', 'contratista11@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista12}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista12}', 'contratista12@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista13}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista13}', 'contratista13@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista14}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista14}', 'contratista14@example.com', '{TestPasswordHash}', 1, GETUTCDATE());
            IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = '{contratista15}')
                INSERT INTO Credenciales (userID, email, password, activo, created_at)
                VALUES ('{contratista15}', 'contratista15@example.com', '{TestPasswordHash}', 1, GETUTCDATE());

            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{empleador1}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{empleador1}', 1, 'Empleador', 'Uno', 'empleador1@example.com');

            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{empleador2}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{empleador2}', 1, 'Empleador', 'Dos', 'empleador2@example.com');

            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista1}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista1}', 2, 'Juan', 'Perez', 'contratista1@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista2}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista2}', 2, 'Carlos', 'Rodriguez', 'contratista2@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista3}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista3}', 2, 'Pedro', 'Martinez', 'contratista3@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista4}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista4}', 2, 'Ana', 'Garcia', 'contratista4@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista5}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista5}', 2, 'Luis', 'Fernandez', 'contratista5@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista6}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista6}', 2, 'Miguel', 'Santos', 'contratista6@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista7}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista7}', 2, 'Roberto', 'Diaz', 'contratista7@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista8}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista8}', 2, 'Jose', 'Vargas', 'contratista8@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista9}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista9}', 2, 'Limpieza', 'Profesional', 'contratista9@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista10}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista10}', 2, 'Maria', 'Lopez', 'contratista10@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista11}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista11}', 2, 'Alberto', 'Ramirez', 'contratista11@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista12}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista12}', 2, 'Herreria', 'Artistica', 'contratista12@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista13}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista13}', 2, 'Laura', 'Jimenez', 'contratista13@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista14}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista14}', 2, 'Fernando', 'Castillo', 'contratista14@example.com');
            IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = '{contratista15}')
                INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                VALUES (GETUTCDATE(), '{contratista15}', 2, 'Patricia', 'Nunez', 'contratista15@example.com');
        ";

        await _context.Database.ExecuteSqlRawAsync(seedSql);

        _logger.LogInformation("Credenciales y perfiles de prueba insertados.");
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

    /// <summary>
    /// Seed de catálogo de servicios ofrecidos
    /// </summary>
    private async Task SeedServiciosAsync()
    {
        if (await _context.Servicios.AnyAsync())
        {
            _logger.LogInformation("Servicios ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando catálogo de servicios...");

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

        _logger.LogInformation("✅ 40 servicios insertados");
    }

    /// <summary>
    /// Seed de contratistas de prueba para desarrollo/testing
    /// </summary>
    private async Task SeedContratistasAsync()
    {
        if (await _context.Contratistas.AnyAsync())
        {
            _logger.LogInformation("Contratistas ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando contratistas de prueba...");

        var userIds = await _context.Perfiles
            .Where(p => p.Tipo == 2)
            .OrderBy(p => p.UserId)
            .Select(p => p.UserId)
            .Take(15)
            .ToListAsync();

        if (userIds.Count < 15)
        {
            _logger.LogWarning(
                "No hay suficientes perfiles tipo contratista para seedear (requeridos 15, encontrados {Count}).",
                userIds.Count);
            return;
        }

        var seedSql = $@"
            SET IDENTITY_INSERT Contratistas ON;
            
            INSERT INTO Contratistas (contratistaID, userID, fechaIngreso, titulo, tipo, identificacion,
                nombre, apellido, sector, experiencia, presentacion, telefono1, whatsapp1, email,
                activo, provincia, nivelNacional, imagenUrl)
            VALUES
                (1, {userIds[0]}, GETUTCDATE(), 'Electricista Certificado', 1, '00112233445',
                 'Juan', 'Pérez', 'Electricidad', 10,
                 'Electricista con 10 años de experiencia en instalaciones residenciales y comerciales. Certificado por la ONAMET. Trabajos garantizados.',
                 '809-555-0001', 1, 'juan.perez@example.com', 1, 'Distrito Nacional', 0, NULL),
                
                (2, {userIds[1]}, GETUTCDATE(), 'Plomero Profesional', 1, '00223344556',
                 'Carlos', 'Rodríguez', 'Plomería', 8,
                 'Especialista en reparaciones e instalaciones. Atención a emergencias 24/7. Más de 8 años de experiencia.',
                 '809-555-0002', 1, 'carlos.rodriguez@example.com', 1, 'Santiago', 0, NULL),
                
                (3, {userIds[2]}, GETUTCDATE(), 'Carpintero Maestro', 1, '00334455667',
                 'Pedro', 'Martínez', 'Carpintería', 15,
                 'Carpintero especializado en muebles a medida y trabajos de ebanistería. Diseños personalizados. 15 años en el mercado.',
                 '809-555-0003', 0, 'pedro.martinez@example.com', 1, 'La Vega', 0, NULL),
                
                (4, {userIds[3]}, GETUTCDATE(), 'Pintora Profesional', 1, '00445566778',
                 'Ana', 'García', 'Pintura', 5,
                 'Pintora con experiencia en pintura residencial y comercial. Trabajos de calidad garantizados.',
                 '809-555-0004', 1, 'ana.garcia@example.com', 1, 'Santo Domingo Este', 0, NULL),
                
                (5, {userIds[4]}, GETUTCDATE(), 'Jardinero y Paisajista', 1, '00556677889',
                 'Luis', 'Fernández', 'Jardinería', 7,
                 'Especialista en diseño de jardines y mantenimiento de áreas verdes. Servicio profesional.',
                 '809-555-0005', 1, 'luis.fernandez@example.com', 1, 'Puerto Plata', 0, NULL),
                
                (6, {userIds[5]}, GETUTCDATE(), 'Técnico en Aire Acondicionado', 1, '00667788990',
                 'Miguel', 'Santos', 'Aire Acondicionado', 12,
                 'Instalación, mantenimiento y reparación de sistemas de AC. Atención rápida y confiable.',
                 '809-555-0006', 1, 'miguel.santos@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (7, {userIds[6]}, GETUTCDATE(), 'Albañil Experimentado', 1, '00778899001',
                 'Roberto', 'Díaz', 'Albañilería', 20,
                 'Maestro albañil con 20 años de experiencia. Construcciones, remodelaciones y reparaciones.',
                 '809-555-0007', 0, 'roberto.diaz@example.com', 1, 'Santiago', 0, NULL),
                
                (8, {userIds[7]}, GETUTCDATE(), 'Mecánico Automotriz', 1, '00889900112',
                 'José', 'Vargas', 'Mecánica Automotriz', 14,
                 'Mecánico especializado en todo tipo de vehículos. Diagnóstico computarizado.',
                 '809-555-0008', 1, 'jose.vargas@example.com', 1, 'Santo Domingo Norte', 0, NULL),
                
                (9, {userIds[8]}, GETUTCDATE(), 'Limpieza Profesional SRL', 2, '131234567',
                 'Limpieza', 'Profesional SRL', 'Limpieza Comercial', 6,
                 'Empresa dedicada a limpieza de oficinas, edificios y locales comerciales. Personal capacitado.',
                 '809-555-0009', 1, 'info@limpiezapro.com', 1, 'Distrito Nacional', 1, NULL),
                
                (10, {userIds[9]}, GETUTCDATE(), 'Chef a Domicilio', 1, '00990011223',
                 'María', 'López', 'Cocina/Chef', 9,
                 'Chef profesional con experiencia en cocina internacional. Eventos, fiestas y comidas diarias.',
                 '809-555-0010', 1, 'maria.lopez@example.com', 1, 'La Romana', 0, NULL),
                
                (11, {userIds[10]}, GETUTCDATE(), 'Diseñador Web Freelance', 1, '00101122334',
                 'Alberto', 'Ramírez', 'Desarrollo Web', 7,
                 'Desarrollador web especializado en sitios modernos y responsivos. WordPress, React, diseño UX/UI.',
                 '809-555-0011', 1, 'alberto.ramirez@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (12, {userIds[11]}, GETUTCDATE(), 'Herrería Artística', 2, '131345678',
                 'Herrería', 'Artística', 'Herrería', 18,
                 'Especialistas en rejas, portones, verjas y estructuras metálicas. Diseños personalizados.',
                 '809-555-0012', 0, 'info@herreriartistica.com', 1, 'Santiago', 0, NULL),
                
                (13, {userIds[12]}, GETUTCDATE(), 'Estilista Profesional', 1, '00112233446',
                 'Laura', 'Jiménez', 'Peluquería', 11,
                 'Estilista con más de 10 años de experiencia. Cortes, coloración, tratamientos capilares.',
                 '809-555-0013', 1, 'laura.jimenez@example.com', 1, 'Puerto Plata', 0, NULL),
                
                (14, {userIds[13]}, GETUTCDATE(), 'Asesoría Legal', 1, '00223344557',
                 'Dr. Fernando', 'Castillo', 'Asesoría Legal', 16,
                 'Abogado especializado en derecho civil, laboral y comercial. Consultas y representación legal.',
                 '809-555-0014', 0, 'fernando.castillo@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (15, {userIds[14]}, GETUTCDATE(), 'Contador Público', 1, '00334455668',
                 'Lcda. Patricia', 'Núñez', 'Asesoría Contable', 13,
                 'Contadora certificada. Servicios de contabilidad, declaraciones de impuestos y auditoría.',
                 '809-555-0015', 1, 'patricia.nunez@example.com', 1, 'Santiago', 1, NULL);
            
            SET IDENTITY_INSERT Contratistas OFF;
        ";

        await _context.Database.ExecuteSqlRawAsync(seedSql);

        _logger.LogInformation("✅ 15 contratistas de prueba insertados");
    }

    /// <summary>
    /// Seed de calificaciones de prueba para contratistas
    /// </summary>
    private async Task SeedCalificacionesAsync()
    {
        if (await _context.Calificaciones.AnyAsync())
        {
            _logger.LogInformation("Calificaciones ya existen, omitiendo seed.");
            return;
        }

        _logger.LogInformation("Insertando calificaciones de prueba...");

        const string empleador1 = "00000000-0000-0000-0000-000000000101";

        await _context.Database.ExecuteSqlRawAsync($@"
            INSERT INTO Calificaciones (userID, tipo, identificacion, nombre, puntualidad, cumplimiento, conocimientos, recomendacion, fecha)
            VALUES
                ('{empleador1}', 'Contratista', '00112233445', 'Juan Perez', 5, 5, 4, 5, DATEADD(DAY, -30, GETUTCDATE())),
                ('{empleador1}', 'Contratista', '00223344556', 'Carlos Rodriguez', 5, 4, 4, 4, DATEADD(DAY, -20, GETUTCDATE())),
                ('{empleador1}', 'Contratista', '00334455667', 'Pedro Martinez', 5, 5, 5, 5, DATEADD(DAY, -15, GETUTCDATE())),
                ('{empleador1}', 'Contratista', '00445566778', 'Ana Garcia', 5, 4, 4, 5, DATEADD(DAY, -10, GETUTCDATE())),
                ('{empleador1}', 'Contratista', '00556677889', 'Luis Fernandez', 5, 5, 4, 5, DATEADD(DAY, -18, GETUTCDATE()));
        ");

        _logger.LogInformation("✅ 5 calificaciones de prueba insertadas");
    }
}
