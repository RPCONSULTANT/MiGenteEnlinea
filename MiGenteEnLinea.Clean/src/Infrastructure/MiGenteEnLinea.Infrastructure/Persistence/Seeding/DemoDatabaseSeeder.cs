using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;

namespace MiGenteEnLinea.Infrastructure.Persistence.Seeding;

/// <summary>
/// Seeder de datos demo para Development/Staging.
/// </summary>
public class DemoDatabaseSeeder
{
    private const string TestPasswordHash = "seed-hash";

    private readonly MiGenteDbContext _context;
    private readonly ILogger<DemoDatabaseSeeder> _logger;

    public DemoDatabaseSeeder(MiGenteDbContext context, ILogger<DemoDatabaseSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        _logger.LogInformation("Iniciando seed de datos demo...");

        await SeedTestCredencialesAndPerfilesAsync();
        await SeedContratistasAsync();
        await SeedCalificacionesAsync();

        _logger.LogInformation("Seed de datos demo completado.");
    }

    private sealed record SeedProfile(string UserId, int Tipo, string Nombre, string Apellido, string Email);

    private async Task SeedTestCredencialesAndPerfilesAsync()
    {
        var profiles = GetSeedProfiles();

        foreach (var profile in profiles)
        {
            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                IF NOT EXISTS (SELECT 1 FROM Credenciales WHERE userID = {profile.UserId})
                    INSERT INTO Credenciales (userID, email, password, activo, created_at)
                    VALUES ({profile.UserId}, {profile.Email}, {TestPasswordHash}, 1, GETUTCDATE());
            ");

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                IF NOT EXISTS (SELECT 1 FROM Perfiles WHERE userID = {profile.UserId})
                    INSERT INTO Perfiles (fechaCreacion, userID, Tipo, Nombre, Apellido, Email)
                    VALUES (GETUTCDATE(), {profile.UserId}, {profile.Tipo}, {profile.Nombre}, {profile.Apellido}, {profile.Email});
            ");
        }
    }

    private async Task SeedContratistasAsync()
    {
        var rows = await _context.Perfiles
            .Where(p => p.Tipo == 2)
            .OrderBy(p => p.UserId)
            .Select(p => p.UserId)
            .Take(15)
            .ToListAsync();

        if (rows.Count < 15)
        {
            _logger.LogWarning("No hay suficientes perfiles tipo contratista para seedear datos demo. Encontrados={Count}", rows.Count);
            return;
        }

        var catalog = new[]
        {
            ("Electricista Certificado", 1, "00112233445", "Juan", "Perez", "Electricidad", 10, "809-555-0001", true, "juan.perez@example.com", "Distrito Nacional", false),
            ("Plomero Profesional", 1, "00223344556", "Carlos", "Rodriguez", "Plomeria", 8, "809-555-0002", true, "carlos.rodriguez@example.com", "Santiago", false),
            ("Carpintero Maestro", 1, "00334455667", "Pedro", "Martinez", "Carpinteria", 15, "809-555-0003", false, "pedro.martinez@example.com", "La Vega", false),
            ("Pintora Profesional", 1, "00445566778", "Ana", "Garcia", "Pintura", 5, "809-555-0004", true, "ana.garcia@example.com", "Santo Domingo Este", false),
            ("Jardinero y Paisajista", 1, "00556677889", "Luis", "Fernandez", "Jardineria", 7, "809-555-0005", true, "luis.fernandez@example.com", "Puerto Plata", false),
            ("Tecnico en Aire Acondicionado", 1, "00667788990", "Miguel", "Santos", "Aire Acondicionado", 12, "809-555-0006", true, "miguel.santos@example.com", "Distrito Nacional", true),
            ("Albanil Experimentado", 1, "00778899001", "Roberto", "Diaz", "Albanileria", 20, "809-555-0007", false, "roberto.diaz@example.com", "Santiago", false),
            ("Mecanico Automotriz", 1, "00889900112", "Jose", "Vargas", "Mecanica Automotriz", 14, "809-555-0008", true, "jose.vargas@example.com", "Santo Domingo Norte", false),
            ("Limpieza Profesional SRL", 2, "131234567", "Limpieza", "Profesional SRL", "Limpieza Comercial", 6, "809-555-0009", true, "info@limpiezapro.com", "Distrito Nacional", true),
            ("Chef a Domicilio", 1, "00990011223", "Maria", "Lopez", "Cocina/Chef", 9, "809-555-0010", true, "maria.lopez@example.com", "La Romana", false),
            ("Disenador Web Freelance", 1, "00101122334", "Alberto", "Ramirez", "Desarrollo Web", 7, "809-555-0011", true, "alberto.ramirez@example.com", "Distrito Nacional", true),
            ("Herreria Artistica", 2, "131345678", "Herreria", "Artistica", "Herreria", 18, "809-555-0012", false, "info@herreriartistica.com", "Santiago", false),
            ("Estilista Profesional", 1, "00112233446", "Laura", "Jimenez", "Peluqueria", 11, "809-555-0013", true, "laura.jimenez@example.com", "Puerto Plata", false),
            ("Asesoria Legal", 1, "00223344557", "Fernando", "Castillo", "Asesoria Legal", 16, "809-555-0014", false, "fernando.castillo@example.com", "Distrito Nacional", true),
            ("Contador Publico", 1, "00334455668", "Patricia", "Nunez", "Asesoria Contable", 13, "809-555-0015", true, "patricia.nunez@example.com", "Santiago", true)
        };

        for (var i = 0; i < catalog.Length; i++)
        {
            var row = catalog[i];
            var userId = rows[i];

            await _context.Database.ExecuteSqlInterpolatedAsync($@"
                IF NOT EXISTS (SELECT 1 FROM Contratistas WHERE userID = {userId})
                    INSERT INTO Contratistas (
                        userID, fechaIngreso, titulo, tipo, identificacion,
                        Nombre, Apellido, sector, experiencia, presentacion,
                        telefono1, whatsapp1, telefono2, whatsapp2, email,
                        activo, provincia, nivelNacional, imagenURL)
                    VALUES (
                        {userId}, GETUTCDATE(), {row.Item1}, {row.Item2}, {row.Item3},
                        {row.Item4}, {row.Item5}, {row.Item6}, {row.Item7}, {"Demo profile: " + row.Item1},
                        {row.Item8}, {row.Item9}, {null as string}, {false}, {row.Item10},
                        {true}, {row.Item11}, {row.Item12}, {null as string});
            ");
        }
    }

    private async Task SeedCalificacionesAsync()
    {
        if (await _context.Calificaciones.AnyAsync())
        {
            return;
        }

        const string empleadorUserId = "00000000-0000-0000-0000-000000000101";

        await _context.Database.ExecuteSqlInterpolatedAsync($@"
            INSERT INTO Calificaciones (userID, tipo, identificacion, nombre, puntualidad, cumplimiento, conocimientos, recomendacion, fecha)
            VALUES
                ({empleadorUserId}, 'Contratista', '00112233445', 'Juan Perez', 5, 5, 4, 5, DATEADD(DAY, -30, GETUTCDATE())),
                ({empleadorUserId}, 'Contratista', '00223344556', 'Carlos Rodriguez', 5, 4, 4, 4, DATEADD(DAY, -20, GETUTCDATE())),
                ({empleadorUserId}, 'Contratista', '00334455667', 'Pedro Martinez', 5, 5, 5, 5, DATEADD(DAY, -15, GETUTCDATE())),
                ({empleadorUserId}, 'Contratista', '00445566778', 'Ana Garcia', 5, 4, 4, 5, DATEADD(DAY, -10, GETUTCDATE())),
                ({empleadorUserId}, 'Contratista', '00556677889', 'Luis Fernandez', 5, 5, 4, 5, DATEADD(DAY, -18, GETUTCDATE()));
        ");
    }

    private static IReadOnlyList<SeedProfile> GetSeedProfiles() =>
    [
        new("00000000-0000-0000-0000-000000000101", 1, "Empleador", "Uno", "empleador1@example.com"),
        new("00000000-0000-0000-0000-000000000102", 1, "Empleador", "Dos", "empleador2@example.com"),
        new("00000000-0000-0000-0000-000000000201", 2, "Juan", "Perez", "contratista1@example.com"),
        new("00000000-0000-0000-0000-000000000202", 2, "Carlos", "Rodriguez", "contratista2@example.com"),
        new("00000000-0000-0000-0000-000000000203", 2, "Pedro", "Martinez", "contratista3@example.com"),
        new("00000000-0000-0000-0000-000000000204", 2, "Ana", "Garcia", "contratista4@example.com"),
        new("00000000-0000-0000-0000-000000000205", 2, "Luis", "Fernandez", "contratista5@example.com"),
        new("00000000-0000-0000-0000-000000000206", 2, "Miguel", "Santos", "contratista6@example.com"),
        new("00000000-0000-0000-0000-000000000207", 2, "Roberto", "Diaz", "contratista7@example.com"),
        new("00000000-0000-0000-0000-000000000208", 2, "Jose", "Vargas", "contratista8@example.com"),
        new("00000000-0000-0000-0000-000000000209", 2, "Limpieza", "Profesional", "contratista9@example.com"),
        new("00000000-0000-0000-0000-000000000210", 2, "Maria", "Lopez", "contratista10@example.com"),
        new("00000000-0000-0000-0000-000000000211", 2, "Alberto", "Ramirez", "contratista11@example.com"),
        new("00000000-0000-0000-0000-000000000212", 2, "Herreria", "Artistica", "contratista12@example.com"),
        new("00000000-0000-0000-0000-000000000213", 2, "Laura", "Jimenez", "contratista13@example.com"),
        new("00000000-0000-0000-0000-000000000214", 2, "Fernando", "Castillo", "contratista14@example.com"),
        new("00000000-0000-0000-0000-000000000215", 2, "Patricia", "Nunez", "contratista15@example.com")
    ];
}
