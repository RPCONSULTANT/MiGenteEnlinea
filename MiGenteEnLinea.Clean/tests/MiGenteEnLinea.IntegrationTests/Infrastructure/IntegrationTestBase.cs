using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Application.Features.Authentication.DTOs;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using Xunit;

namespace MiGenteEnLinea.IntegrationTests.Infrastructure;

/// <summary>
/// Clase base para todos los tests de integración.
/// Proporciona HttpClient configurado y métodos auxiliares para autenticación y limpieza.
/// </summary>
public abstract class IntegrationTestBase : IClassFixture<TestWebApplicationFactory>, IDisposable
{
    protected readonly HttpClient Client;
    protected readonly TestWebApplicationFactory Factory;
    protected readonly MiGenteDbContext DbContext;
    protected readonly IApplicationDbContext AppDbContext; // Interfaz para acceder a DbSets
    
    // Token JWT para autenticación (se obtiene después de login)
    protected string? AccessToken { get; set; }

    protected IntegrationTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
        
        // Configurar JSON options para serialización consistente
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        
        // Obtener DbContext para poder hacer seed de datos y assertions
        var scope = factory.Services.CreateScope();
        DbContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        AppDbContext = DbContext; // MiGenteDbContext implementa IApplicationDbContext
        
        // Seedear datos de prueba automáticamente
        SeedTestData().GetAwaiter().GetResult();
    }
    
    /// <summary>
    /// Seedea datos de prueba en la base de datos SQL Server
    /// </summary>
    private async Task SeedTestData()
    {
        // Verificar si hay datos COMPLETOS (incluyendo DeduccionesTss y Planes)
        var hasDeduccionesTss = await AppDbContext.DeduccionesTss.AnyAsync();
        var hasPlanes = await AppDbContext.PlanesEmpleadores.AnyAsync();
        var hasPlanesContratistas = await AppDbContext.PlanesContratistas.AnyAsync();
        var hasCredenciales = await AppDbContext.Credenciales.AnyAsync();
        
        // Verificar que los PlanesContratistas tengan NombrePlan correctamente seteado
        var planesContratistasConNombre = await AppDbContext.PlanesContratistas
            .Where(p => p.NombrePlan != null && p.NombrePlan != "")
            .AnyAsync();
        
        // Si faltan datos críticos o los datos son incorrectos, limpiar y recrear
        if (!hasDeduccionesTss)
        {
            await TestDataSeeder.SeedDeduccionesTssAsync(AppDbContext);
        }
        
        if (!hasPlanes)
        {
            await TestDataSeeder.SeedPlanesAsync(AppDbContext);
        }
        
        // Si hay planes pero sin nombre, eliminarlos y recrear
        if (hasPlanesContratistas && !planesContratistasConNombre)
        {
            // Eliminar planes corruptos
            var planesCorruptos = await AppDbContext.PlanesContratistas.ToListAsync();
            foreach (var plan in planesCorruptos)
            {
                DbContext.PlanesContratistas.Remove(plan);
            }
            await DbContext.SaveChangesAsync();
            
            // Recrear planes
            await TestDataSeeder.SeedPlanesContratistasAsync(AppDbContext);
        }
        else if (!hasPlanesContratistas)
        {
            await TestDataSeeder.SeedPlanesContratistasAsync(AppDbContext);
        }
        
        if (!hasCredenciales)
        {
            await TestDataSeeder.SeedUsuariosAsync(AppDbContext);
        }
    }

    /// <summary>
    /// Configura el token JWT en los headers del HttpClient para requests autenticados
    /// </summary>
    protected void SetAuthToken(string token)
    {
        AccessToken = token;
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Remueve el token de autenticación (para tests de endpoints no autenticados)
    /// </summary>
    protected void ClearAuthToken()
    {
        AccessToken = null;
        Client.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Helper para hacer login y obtener token JWT
    /// </summary>
    protected async Task<string> LoginAsync(string email, string password)
    {
        var loginRequest = new
        {
            email,
            password
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginRequest);
        response.EnsureSuccessStatusCode();

        // ✅ FIX: Deserializar a AuthenticationResultDto directamente (PascalCase)
        var loginResult = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
        
        loginResult.Should().NotBeNull("El login debe devolver un AuthenticationResultDto");
        loginResult!.AccessToken.Should().NotBeNullOrEmpty("El login debe devolver un access token");
        
        SetAuthToken(loginResult.AccessToken);
        return loginResult.AccessToken;
    }

    /// <summary>
    /// Helper para registrar un usuario de prueba
    /// Retorna el UserId como string (GUID de Identity)
    /// </summary>
    protected async Task<string> RegisterUserAsync(
        string email, 
        string password, 
        string nombre, 
        string apellido,
        string tipo = "Empleador", // o "Contratista"
        string? identificacion = null)
    {
        // ✅ FIX: RegisterCommand expects Tipo as int (1=Empleador, 2=Contratista) and Host property
        int tipoInt = tipo.Equals("Contratista", StringComparison.OrdinalIgnoreCase) ? 2 : 1;
        
        var registerRequest = new
        {
            email,
            password,
            nombre,
            apellido,
            tipo = tipoInt, // ✅ Changed from string to int
            host = "https://localhost:5015" // ✅ Added required Host property
            // ✅ Removed identificacion - not used by RegisterCommand
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", registerRequest);
        response.EnsureSuccessStatusCode();

        // ✅ FIX: Deserializar a RegisterResult directamente (más seguro que JsonElement)
        var registerResult = await response.Content.ReadFromJsonAsync<RegisterResult>();
        
        registerResult.Should().NotBeNull("El registro debe devolver un RegisterResult");
        registerResult!.Success.Should().BeTrue("El registro debe ser exitoso");
        registerResult.UserId.Should().NotBeNullOrEmpty("El registro debe devolver un UserId válido (GUID)");
        
        // ✅ AUTO-ACTIVATE: Activar la cuenta inmediatamente después del registro para tests
        var activateRequest = new
        {
            userId = registerResult.UserId,
            email = email
        };
        var activateResponse = await Client.PostAsJsonAsync("/api/auth/activate", activateRequest);
        activateResponse.EnsureSuccessStatusCode();
        
        return registerResult.UserId!;
    }

    /// <summary>
    /// Genera una identificación aleatoria (cédula dominicana simulada)
    /// </summary>
    protected string GenerateRandomIdentification()
    {
        var random = new Random();
        // Formato: XXX-XXXXXXX-X (11 dígitos)
        return $"{random.Next(100, 999)}{random.Next(1000000, 9999999)}{random.Next(0, 9)}";
    }

    /// <summary>
    /// Genera un email único para pruebas
    /// </summary>
    protected string GenerateUniqueEmail(string prefix = "test")
    {
        return $"{prefix}_{Guid.NewGuid():N}@test.com";
    }
    
    /// <summary>
    /// Limpia el ChangeTracker de EF Core para forzar queries frescas desde la base de datos.
    /// Útil cuando el test necesita verificar cambios hechos por HTTP requests.
    /// </summary>
    protected void ClearChangeTracker()
    {
        DbContext.ChangeTracker.Clear();
    }

    /// <summary>
    /// Limpia la base de datos entre tests (opcional, InMemory se recrea automáticamente)
    /// </summary>
    protected async Task CleanupDatabaseAsync()
    {
        // Para InMemory Database, simplemente recreamos
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.Database.EnsureCreatedAsync();
    }

    /// <summary>
    /// Assertions helper para verificar response status codes
    /// </summary>
    protected void AssertSuccessStatusCode(HttpResponseMessage response, string because = "")
    {
        response.IsSuccessStatusCode.Should().BeTrue(
            $"Expected success status code but got {response.StatusCode}. {because}");
    }

    /// <summary>
    /// Assertions helper para verificar que la response es 400 Bad Request
    /// </summary>
    protected void AssertBadRequest(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    /// <summary>
    /// Assertions helper para verificar que la response es 401 Unauthorized
    /// </summary>
    protected void AssertUnauthorized(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
    }

    /// <summary>
    /// Assertions helper para verificar que la response es 404 Not Found
    /// </summary>
    protected void AssertNotFound(HttpResponseMessage response)
    {
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    public virtual void Dispose()
    {
        Client?.Dispose();
        DbContext?.Dispose();
    }
}
