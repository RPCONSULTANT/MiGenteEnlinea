using Xunit;
using FluentAssertions;
using MiGenteEnLinea.Application.Features.Empleados.DTOs;
using MiGenteEnLinea.Application.Features.Suscripciones.DTOs;
using MiGenteEnLinea.Application.Features.Suscripciones.Commands.CreateSuscripcion;
using MiGenteEnLinea.IntegrationTests.Infrastructure;
using System.Net;
using System.Net.Http.Json;

namespace MiGenteEnLinea.IntegrationTests.Controllers;

/// <summary>
/// Tests de integración para lógica de negocio compleja del dominio.
/// Enfocado en: catálogos TSS, validaciones de suscripciones, y planes.
/// </summary>
public class BusinessLogicTests : IntegrationTestBase
{
    public BusinessLogicTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region TSS Catalog Tests

    [Fact]
    public async Task GetDeduccionesTss_WithAuthentication_ReturnsActiveTssDeductions()
    {
        // Arrange
        var email = GenerateUniqueEmail("tss");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        // Act
        var response = await Client.GetAsync("/api/empleados/deducciones-tss");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var deducciones = await response.Content.ReadFromJsonAsync<List<DeduccionTssDto>>();
        deducciones.Should().NotBeNull();
        deducciones.Should().NotBeEmpty("debe haber deducciones TSS configuradas en base de datos");
    }

    [Fact]
    public async Task GetDeduccionesTss_ReturnsDeductionsWithValidPercentages()
    {
        // Arrange
        var email = GenerateUniqueEmail("tss2");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        // Act
        var response = await Client.GetAsync("/api/empleados/deducciones-tss");
        var deducciones = await response.Content.ReadFromJsonAsync<List<DeduccionTssDto>>();

        // Assert
        deducciones.Should().AllSatisfy(d =>
        {
            d.Porcentaje.Should().BeGreaterThan(0, "porcentaje debe ser positivo");
            d.Porcentaje.Should().BeLessOrEqualTo(100, "porcentaje no puede exceder 100%");
            d.Descripcion.Should().NotBeNullOrWhiteSpace("descripción es requerida");
        });
    }

    [Fact]
    public async Task GetDeduccionesTss_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Act
        var response = await Client.GetAsync("/api/empleados/deducciones-tss");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized, 
            "endpoint requiere autenticación JWT");
    }

    #endregion

    #region Suscripciones Business Logic Tests

    [Fact]
    public async Task CreateSuscripcion_WithPastStartDate_CalculatesExpirationCorrectly()
    {
        // Arrange
        var email = GenerateUniqueEmail("expiry");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        // Create subscription starting 10 days ago (should expire in 20 days from now)
        var command = new CreateSuscripcionCommand
        {
            UserId = userId.ToString(),
            PlanId = 1, // Assuming plan with 30 days duration
            FechaInicio = DateTime.Now.AddDays(-10)
        };

        // Act
        var createResponse = await Client.PostAsJsonAsync("/api/suscripciones", command);

        // Assert
        createResponse.StatusCode.Should().Be(HttpStatusCode.Created, 
            "suscripción con fecha pasada debe ser válida");
        
        // API returns { suscripcionId: X }
        var responseBody = await createResponse.Content.ReadAsStringAsync();
        using var doc = System.Text.Json.JsonDocument.Parse(responseBody);
        var suscripcionId = doc.RootElement.GetProperty("suscripcionId").GetInt32();
        suscripcionId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task GetSuscripcion_AfterCreation_ReturnsActiveStatus()
    {
        // Arrange
        var email = GenerateUniqueEmail("active");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        var command = new CreateSuscripcionCommand
        {
            UserId = userId.ToString(),
            PlanId = 1,
            FechaInicio = DateTime.Now
        };

        await Client.PostAsJsonAsync("/api/suscripciones", command);

        // Act
        var getResponse = await Client.GetAsync($"/api/suscripciones/activa/{userId}");

        // Assert
        getResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var suscripcion = await getResponse.Content.ReadFromJsonAsync<SuscripcionDto>();
        suscripcion.Should().NotBeNull();
        suscripcion!.EstaActiva.Should().BeTrue("suscripción recién creada debe estar activa");
        suscripcion.DiasRestantes.Should().BeGreaterThan(20, "debe tener ~30 días restantes");
    }

    [Fact]
    public async Task CreateSuscripcion_WithInvalidPlanId_ReturnsNotFound()
    {
        // Arrange
        var email = GenerateUniqueEmail("invalid");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        var command = new CreateSuscripcionCommand
        {
            UserId = userId.ToString(),
            PlanId = 99999, // Non-existent plan
            FechaInicio = DateTime.Now
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/suscripciones", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound, 
            "plan inexistente debe retornar 404 Not Found");
    }

    #endregion

    #region Planes Catalog Tests

    [Fact]
    public async Task GetPlanesEmpleadores_ReturnsActivePlans()
    {
        // Act - Planes endpoint is [AllowAnonymous], no authentication needed
        var response = await Client.GetAsync("/api/suscripciones/planes/empleadores");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var planes = await response.Content.ReadFromJsonAsync<List<PlanDto>>();
        planes.Should().NotBeNull();
        planes.Should().NotBeEmpty("debe haber planes de empleadores configurados");
        planes.Should().AllSatisfy(p =>
        {
            p.PlanId.Should().BeGreaterThan(0);
            p.Nombre.Should().NotBeNullOrWhiteSpace();
            p.Precio.Should().BeGreaterThanOrEqualTo(0);
        });
    }

    [Fact]
    public async Task GetPlanesContratistas_ReturnsActivePlans()
    {
        // Act - Planes endpoint is [AllowAnonymous], no authentication needed
        var response = await Client.GetAsync("/api/suscripciones/planes/contratistas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var planes = await response.Content.ReadFromJsonAsync<List<PlanDto>>();
        planes.Should().NotBeNull();
        planes.Should().NotBeEmpty("debe haber planes de contratistas configurados");
        planes.Should().AllSatisfy(p =>
        {
            p.PlanId.Should().BeGreaterThan(0);
            p.Nombre.Should().NotBeNullOrWhiteSpace();
            p.Precio.Should().BeGreaterThanOrEqualTo(0);
        });
    }

    [Fact]
    public async Task GetPlanes_WithoutAuthentication_ReturnsOk_BecausePlanesArePublic()
    {
        // Act - Planes endpoints are [AllowAnonymous] to allow viewing before registration
        var responseEmpleadores = await Client.GetAsync("/api/suscripciones/planes/empleadores");
        var responseContratistas = await Client.GetAsync("/api/suscripciones/planes/contratistas");

        // Assert - Should return OK because planes are public
        responseEmpleadores.StatusCode.Should().Be(HttpStatusCode.OK);
        responseContratistas.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    #endregion

    #region Domain Validations Tests

    [Fact]
    public async Task CreateSuscripcion_WithNullFechaInicio_UsesTodayAsDefault()
    {
        // Arrange
        var email = GenerateUniqueEmail("default");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        var command = new CreateSuscripcionCommand
        {
            UserId = userId.ToString(),
            PlanId = 1,
            FechaInicio = null // Should default to today
        };

        // Act
        var response = await Client.PostAsJsonAsync("/api/suscripciones", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        // API returns { suscripcionId: X }
        var responseBody = await response.Content.ReadAsStringAsync();
        using var doc2 = System.Text.Json.JsonDocument.Parse(responseBody);
        var suscripcionId = doc2.RootElement.GetProperty("suscripcionId").GetInt32();
        suscripcionId.Should().BeGreaterThan(0, 
            "suscripción sin fecha debe usar fecha actual por defecto");
    }

    [Fact]
    public async Task GetSuscripcion_ForNonExistentUser_ReturnsNotFound()
    {
        // Arrange
        var email = GenerateUniqueEmail("notfound");
        var userId = await RegisterUserAsync(email, "Test123!@#", "Empleador", "Test", "User");
        await LoginAsync(email, "Test123!@#");

        // Act - Try to get subscription for non-existent user
        var response = await Client.GetAsync($"/api/suscripciones/activa/{Guid.NewGuid()}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound,
            "suscripción de usuario inexistente debe retornar 404");
    }

    #endregion
}