using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using MiGenteEnLinea.Application.Features.Contratistas.Commands.CreateContratista;
using MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratista;
using MiGenteEnLinea.Application.Features.Contratistas.Common;
using MiGenteEnLinea.Application.Features.Contratistas.Queries.SearchContratistas;
using MiGenteEnLinea.IntegrationTests.Infrastructure;
using Xunit;

namespace MiGenteEnLinea.IntegrationTests.Controllers;

/// <summary>
/// Integration tests for ContratistasController
/// BLOQUE 3: Contratistas CRUD operations (6 tests)
/// </summary>
[Collection("IntegrationTests")]
public class ContratistasControllerTests : IntegrationTestBase
{
    public ContratistasControllerTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region CreateContratista Tests (2 tests)

    [Fact]
    public async Task CreateContratista_ForRegisteredUser_ReturnsContratistaData()
    {
        // Note: Per GAP-010, ALL users get a Contratista auto-created on registration
        // (Legacy behavior: everyone can offer services)
        // This test verifies we can retrieve the auto-created contratista
        
        // Arrange - Register and login (contratista is auto-created)
        var email = GenerateUniqueEmail("contratista");
        var userId = await RegisterUserAsync(email, "Password123!", "Pedro", "García", "Contratista");
        await LoginAsync(email, "Password123!");

        // Act - Get the auto-created contratista
        var response = await Client.GetAsync($"/api/contratistas/by-user/{userId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contratista = await response.Content.ReadFromJsonAsync<ContratistaDto>();
        contratista.Should().NotBeNull();
        contratista!.UserId.Should().Be(userId.ToString());
        contratista.Nombre.Should().Be("Pedro");
        contratista.Apellido.Should().Be("García");
    }

    [Fact]
    public async Task CreateContratista_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - No authentication token
        ClearAuthToken();

        var command = new CreateContratistaCommand(
            UserId: "some-user-id",
            Nombre: "Test",
            Apellido: "User"
        );

        // Act
        var response = await Client.PostAsJsonAsync("/api/contratistas", command);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region GetContratistaById Tests (1 test)

    [Fact]
    public async Task GetContratistaById_WithValidId_ReturnsContratistaDto()
    {
        // Arrange - Register as contratista (auto-creates Contratista per GAP-010)
        var email = GenerateUniqueEmail("contratista");
        var userId = await RegisterUserAsync(email, "Password123!", "María", "López", "Contratista");
        await LoginAsync(email, "Password123!");

        // Get the auto-created contratista by userId
        var byUserResponse = await Client.GetAsync($"/api/contratistas/by-user/{userId}");
        byUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var autoCreatedContratista = await byUserResponse.Content.ReadFromJsonAsync<ContratistaDto>();
        var contratistaId = autoCreatedContratista!.ContratistaId;

        // Act
        var response = await Client.GetAsync($"/api/contratistas/{contratistaId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var contratistaDto = await response.Content.ReadFromJsonAsync<ContratistaDto>();
        contratistaDto.Should().NotBeNull();
        contratistaDto!.ContratistaId.Should().Be(contratistaId);
        contratistaDto.UserId.Should().Be(userId.ToString());
        contratistaDto.Nombre.Should().Be("María");
        contratistaDto.Apellido.Should().Be("López");
    }

    #endregion

    #region GetContratistasList Tests (1 test)

    [Fact]
    public async Task GetContratistasList_ReturnsListOfContratistas()
    {
        // Arrange - Register and login
        var email = GenerateUniqueEmail("contratista");
        await RegisterUserAsync(email, "Password123!", "Carlos", "Martínez", "Contratista");
        await LoginAsync(email, "Password123!");

        // Act
        var response = await Client.GetAsync("/api/contratistas");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var result = await response.Content.ReadFromJsonAsync<SearchContratistasResult>();
        result.Should().NotBeNull();
        result!.Contratistas.Should().NotBeNull();
        // Note: List might be empty or contain test data
    }

    #endregion

    #region UpdateContratista Tests (2 tests)

    [Fact]
    public async Task UpdateContratista_WithValidData_UpdatesSuccessfully()
    {
        // Arrange - Register as contratista (auto-creates Contratista per GAP-010)
        var email = GenerateUniqueEmail("contratista");
        var userId = await RegisterUserAsync(email, "Password123!", "Ana", "Rodríguez", "Contratista");
        await LoginAsync(email, "Password123!");

        // Get the auto-created contratista by userId
        var byUserResponse = await Client.GetAsync($"/api/contratistas/by-user/{userId}");
        byUserResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        var autoCreatedContratista = await byUserResponse.Content.ReadFromJsonAsync<ContratistaDto>();
        var contratistaId = autoCreatedContratista!.ContratistaId;

        // Update contratista - Note: API uses userId in the route, not contratistaId
        var updateCommand = new UpdateContratistaCommand(
            UserId: userId.ToString(),
            Titulo: "Carpintera profesional certificada",
            Sector: "Carpintería y Ebanistería",
            Experiencia: 7,
            Presentacion: "Updated: Carpintera especializada en muebles a medida",
            Provincia: "Santo Domingo",
            Telefono1: "8092222222",
            Whatsapp1: true,
            Telefono2: "8093333333",
            Email: "ana.carpintera@test.com"
        );

        // Act - Use userId in the route, not contratistaId
        var response = await Client.PutAsJsonAsync($"/api/contratistas/{userId}", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        // Verify update - GET uses contratistaId
        var getResponse = await Client.GetAsync($"/api/contratistas/{contratistaId}");
        var updatedContratista = await getResponse.Content.ReadFromJsonAsync<ContratistaDto>();
        updatedContratista.Should().NotBeNull();
        updatedContratista!.Titulo.Should().Be("Carpintera profesional certificada");
        updatedContratista.Sector.Should().Be("Carpintería y Ebanistería");
        updatedContratista.Experiencia.Should().Be(7);
        updatedContratista.Presentacion.Should().Be("Updated: Carpintera especializada en muebles a medida");
        updatedContratista.Provincia.Should().Be("Santo Domingo");
        updatedContratista.Telefono1.Should().Be("8092222222");
        updatedContratista.Email.Should().Be("ana.carpintera@test.com");
    }

    [Fact]
    public async Task UpdateContratista_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Arrange - No authentication token
        ClearAuthToken();

        var updateCommand = new UpdateContratistaCommand(
            UserId: "some-user-id",
            Titulo: "Test title"
        );

        // Act
        var response = await Client.PutAsJsonAsync("/api/contratistas/123", updateCommand);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}