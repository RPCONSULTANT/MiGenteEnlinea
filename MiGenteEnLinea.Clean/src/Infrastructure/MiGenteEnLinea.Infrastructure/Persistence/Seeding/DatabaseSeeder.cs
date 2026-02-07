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
            await SeedServiciosAsync();
            await SeedMissingEmpleadoresAsync();
            await SeedContratistasAsync();
            
            // TODO: Re-enable once we have test empleadores with valid userIDs
            // Calificaciones table requires valid Credenciales.userID (empleador who rates)
            // await SeedCalificacionesAsync();

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

        await _context.Database.ExecuteSqlRawAsync(@"
            SET IDENTITY_INSERT Contratistas ON;
            
            INSERT INTO Contratistas (contratistaID, userID, fechaIngreso, titulo, tipo, identificacion,
                nombre, apellido, sector, experiencia, presentacion, telefono1, whatsapp1, email,
                activo, provincia, nivelNacional, imagenUrl)
            VALUES
                (1, NULL, GETUTCDATE(), 'Electricista Certificado', 1, '00112233445',
                 'Juan', 'Pérez', 'Electricidad', 10,
                 'Electricista con 10 años de experiencia en instalaciones residenciales y comerciales. Certificado por la ONAMET. Trabajos garantizados.',
                 '809-555-0001', 1, 'juan.perez@example.com', 1, 'Distrito Nacional', 0, NULL),
                
                (2, NULL, GETUTCDATE(), 'Plomero Profesional', 1, '00223344556',
                 'Carlos', 'Rodríguez', 'Plomería', 8,
                 'Especialista en reparaciones e instalaciones. Atención a emergencias 24/7. Más de 8 años de experiencia.',
                 '809-555-0002', 1, 'carlos.rodriguez@example.com', 1, 'Santiago', 0, NULL),
                
                (3, NULL, GETUTCDATE(), 'Carpintero Maestro', 1, '00334455667',
                 'Pedro', 'Martínez', 'Carpintería', 15,
                 'Carpintero especializado en muebles a medida y trabajos de ebanistería. Diseños personalizados. 15 años en el mercado.',
                 '809-555-0003', 0, 'pedro.martinez@example.com', 1, 'La Vega', 0, NULL),
                
                (4, NULL, GETUTCDATE(), 'Pintora Profesional', 1, '00445566778',
                 'Ana', 'García', 'Pintura', 5,
                 'Pintora con experiencia en pintura residencial y comercial. Trabajos de calidad garantizados.',
                 '809-555-0004', 1, 'ana.garcia@example.com', 1, 'Santo Domingo Este', 0, NULL),
                
                (5, NULL, GETUTCDATE(), 'Jardinero y Paisajista', 1, '00556677889',
                 'Luis', 'Fernández', 'Jardinería', 7,
                 'Especialista en diseño de jardines y mantenimiento de áreas verdes. Servicio profesional.',
                 '809-555-0005', 1, 'luis.fernandez@example.com', 1, 'Puerto Plata', 0, NULL),
                
                (6, NULL, GETUTCDATE(), 'Técnico en Aire Acondicionado', 1, '00667788990',
                 'Miguel', 'Santos', 'Aire Acondicionado', 12,
                 'Instalación, mantenimiento y reparación de sistemas de AC. Atención rápida y confiable.',
                 '809-555-0006', 1, 'miguel.santos@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (7, NULL, GETUTCDATE(), 'Albañil Experimentado', 1, '00778899001',
                 'Roberto', 'Díaz', 'Albañilería', 20,
                 'Maestro albañil con 20 años de experiencia. Construcciones, remodelaciones y reparaciones.',
                 '809-555-0007', 0, 'roberto.diaz@example.com', 1, 'Santiago', 0, NULL),
                
                (8, NULL, GETUTCDATE(), 'Mecánico Automotriz', 1, '00889900112',
                 'José', 'Vargas', 'Mecánica Automotriz', 14,
                 'Mecánico especializado en todo tipo de vehículos. Diagnóstico computarizado.',
                 '809-555-0008', 1, 'jose.vargas@example.com', 1, 'Santo Domingo Norte', 0, NULL),
                
                (9, NULL, GETUTCDATE(), 'Limpieza Profesional SRL', 2, '131234567',
                 'Limpieza', 'Profesional SRL', 'Limpieza Comercial', 6,
                 'Empresa dedicada a limpieza de oficinas, edificios y locales comerciales. Personal capacitado.',
                 '809-555-0009', 1, 'info@limpiezapro.com', 1, 'Distrito Nacional', 1, NULL),
                
                (10, NULL, GETUTCDATE(), 'Chef a Domicilio', 1, '00990011223',
                 'María', 'López', 'Cocina/Chef', 9,
                 'Chef profesional con experiencia en cocina internacional. Eventos, fiestas y comidas diarias.',
                 '809-555-0010', 1, 'maria.lopez@example.com', 1, 'La Romana', 0, NULL),
                
                (11, NULL, GETUTCDATE(), 'Diseñador Web Freelance', 1, '00101122334',
                 'Alberto', 'Ramírez', 'Desarrollo Web', 7,
                 'Desarrollador web especializado en sitios modernos y responsivos. WordPress, React, diseño UX/UI.',
                 '809-555-0011', 1, 'alberto.ramirez@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (12, NULL, GETUTCDATE(), 'Herrería Artística', 2, '131345678',
                 'Herrería', 'Artística', 'Herrería', 18,
                 'Especialistas en rejas, portones, verjas y estructuras metálicas. Diseños personalizados.',
                 '809-555-0012', 0, 'info@herreriartistica.com', 1, 'Santiago', 0, NULL),
                
                (13, NULL, GETUTCDATE(), 'Estilista Profesional', 1, '00112233446',
                 'Laura', 'Jiménez', 'Peluquería', 11,
                 'Estilista con más de 10 años de experiencia. Cortes, coloración, tratamientos capilares.',
                 '809-555-0013', 1, 'laura.jimenez@example.com', 1, 'Puerto Plata', 0, NULL),
                
                (14, NULL, GETUTCDATE(), 'Asesoría Legal', 1, '00223344557',
                 'Dr. Fernando', 'Castillo', 'Asesoría Legal', 16,
                 'Abogado especializado en derecho civil, laboral y comercial. Consultas y representación legal.',
                 '809-555-0014', 0, 'fernando.castillo@example.com', 1, 'Distrito Nacional', 1, NULL),
                
                (15, NULL, GETUTCDATE(), 'Contador Público', 1, '00334455668',
                 'Lcda. Patricia', 'Núñez', 'Asesoría Contable', 13,
                 'Contadora certificada. Servicios de contabilidad, declaraciones de impuestos y auditoría.',
                 '809-555-0015', 1, 'patricia.nunez@example.com', 1, 'Santiago', 1, NULL);
            
            SET IDENTITY_INSERT Contratistas OFF;
        ");

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

        await _context.Database.ExecuteSqlRawAsync(@"
            INSERT INTO Calificaciones (contratacionID, contratistaID, ofertanteID, calificacion, comentario, fecha)
            VALUES
                -- Electricista (contratistaID 1) - Promedio: 4.67
                (NULL, 1, NULL, 5, 'Excelente trabajo, muy profesional y puntual', DATEADD(DAY, -30, GETUTCDATE())),
                (NULL, 1, NULL, 5, 'Resolvió el problema eléctrico rápidamente', DATEADD(DAY, -45, GETUTCDATE())),
                (NULL, 1, NULL, 4, 'Buen servicio, recomendado', DATEADD(DAY, -60, GETUTCDATE())),
                
                -- Plomero (contratistaID 2) - Promedio: 4.33
                (NULL, 2, NULL, 5, 'Excelente plomero, solucionó fuga compleja', DATEADD(DAY, -20, GETUTCDATE())),
                (NULL, 2, NULL, 4, 'Buen trabajo en la instalación', DATEADD(DAY, -35, GETUTCDATE())),
                (NULL, 2, NULL, 4, 'Servicio confiable', DATEADD(DAY, -50, GETUTCDATE())),
                
                -- Carpintero (contratistaID 3) - Promedio: 5.0
                (NULL, 3, NULL, 5, 'Muebles de excelente calidad, superó expectativas', DATEADD(DAY, -15, GETUTCDATE())),
                (NULL, 3, NULL, 5, 'Artesano excepcional, muy detallista', DATEADD(DAY, -40, GETUTCDATE())),
                (NULL, 3, NULL, 5, 'Trabajo impecable en la cocina integral', DATEADD(DAY, -65, GETUTCDATE())),
                
                -- Pintora (contratistaID 4) - Promedio: 4.5
                (NULL, 4, NULL, 5, 'Pintura perfecta, muy limpia y ordenada', DATEADD(DAY, -10, GETUTCDATE())),
                (NULL, 4, NULL, 4, 'Buen trabajo en toda la casa', DATEADD(DAY, -25, GETUTCDATE())),
                
                -- Jardinero (contratistaID 5) - Promedio: 4.67
                (NULL, 5, NULL, 5, 'Jardín espectacular, diseño hermoso', DATEADD(DAY, -18, GETUTCDATE())),
                (NULL, 5, NULL, 5, 'Excelente mantenimiento, muy profesional', DATEADD(DAY, -32, GETUTCDATE())),
                (NULL, 5, NULL, 4, 'Buen servicio de jardinería', DATEADD(DAY, -55, GETUTCDATE())),
                
                -- Técnico AC (contratistaID 6) - Promedio: 4.75
                (NULL, 6, NULL, 5, 'Instaló AC rápido y bien', DATEADD(DAY, -12, GETUTCDATE())),
                (NULL, 6, NULL, 5, 'Excelente servicio técnico', DATEADD(DAY, -28, GETUTCDATE())),
                (NULL, 6, NULL, 5, 'Resolvió problema de refrigeración', DATEADD(DAY, -42, GETUTCDATE())),
                (NULL, 6, NULL, 4, 'Buen trabajo de mantenimiento', DATEADD(DAY, -58, GETUTCDATE())),
                
                -- Albañil (contratistaID 7) - Promedio: 5.0
                (NULL, 7, NULL, 5, 'Maestro constructor, trabajo perfecto', DATEADD(DAY, -22, GETUTCDATE())),
                (NULL, 7, NULL, 5, 'Remodelación excelente', DATEADD(DAY, -48, GETUTCDATE())),
                
                -- Mecánico (contratistaID 8) - Promedio: 4.33
                (NULL, 8, NULL, 5, 'Reparó mi auto perfecto', DATEADD(DAY, -8, GETUTCDATE())),
                (NULL, 8, NULL, 4, 'Buen diagnóstico y reparación', DATEADD(DAY, -24, GETUTCDATE())),
                (NULL, 8, NULL, 4, 'Servicio confiable', DATEADD(DAY, -38, GETUTCDATE())),
                
                -- Limpieza (contratistaID 9) - Promedio: 4.75
                (NULL, 9, NULL, 5, 'Oficina impecable, muy profesionales', DATEADD(DAY, -5, GETUTCDATE())),
                (NULL, 9, NULL, 5, 'Excelente servicio de limpieza profunda', DATEADD(DAY, -19, GETUTCDATE())),
                (NULL, 9, NULL, 4, 'Buen trabajo en el edificio', DATEADD(DAY, -33, GETUTCDATE())),
                (NULL, 9, NULL, 5, 'Personal muy capacitado', DATEADD(DAY, -47, GETUTCDATE())),
                
                -- Chef (contratistaID 10) - Promedio: 5.0
                (NULL, 10, NULL, 5, 'Comida deliciosa en la fiesta', DATEADD(DAY, -14, GETUTCDATE())),
                (NULL, 10, NULL, 5, 'Chef increíble, menú espectacular', DATEADD(DAY, -29, GETUTCDATE()));
        ");

        _logger.LogInformation("✅ 30 calificaciones de prueba insertadas");
    }
}
