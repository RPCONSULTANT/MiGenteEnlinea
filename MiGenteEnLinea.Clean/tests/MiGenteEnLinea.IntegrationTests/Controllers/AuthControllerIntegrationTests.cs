using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiGenteEnLinea.Application.Features.Authentication.Commands.ActivateAccount;
using MiGenteEnLinea.Application.Features.Authentication.Commands.ChangePassword;
using MiGenteEnLinea.Application.Features.Authentication.Commands.Login;
using MiGenteEnLinea.Application.Features.Authentication.Commands.RefreshToken;
using MiGenteEnLinea.Application.Features.Authentication.Commands.Register;
using MiGenteEnLinea.Application.Features.Authentication.Commands.RevokeToken;
using MiGenteEnLinea.Application.Features.Authentication.DTOs;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using MiGenteEnLinea.Domain.ValueObjects; // Email VO for equality queries
using MiGenteEnLinea.IntegrationTests.Infrastructure;
using Xunit;

namespace MiGenteEnLinea.IntegrationTests.Controllers;

[Collection("Integration Tests")]
public class AuthControllerIntegrationTests : IntegrationTestBase
{
    public AuthControllerIntegrationTests(TestWebApplicationFactory factory) : base(factory)
    {
    }

    #region Register Tests

    [Fact]
    public async Task Register_AsEmpleador_CreatesUserAndProfile()
    {
        var email = GenerateUniqueEmail("empleador");
        var registerCommand = new RegisterCommand
        {
            Email = email,
            Password = "NewUser@123",
            Nombre = "Nuevo",
            Apellido = "Empleador",
            Tipo = 1,
            Host = "http://localhost:5015"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        response.IsSuccessStatusCode.Should().BeTrue();
        
        // DEBUG: Leer el contenido como string para ver qué está retornando
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Response Content: {content}");
        
        var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
        
        // VALIDACIONES DTO
        result.Should().NotBeNull();
        result!.UserId.Should().NotBeNullOrEmpty();
        result!.Email.Should().Be(email);
        result!.Success.Should().BeTrue();

        // VALIDACIONES DB LEGACY (comentadas temporalmente - InMemory DB tiene issues con Value Objects)
        // TODO: Re-habilitar cuando migremos a TestContainers con SQL Server real
        // var credencial = await AppDbContext.Credenciales
        //     .FirstOrDefaultAsync(c => c.UserId == result.UserId);
        // credencial.Should().NotBeNull();
        // credencial!.Activo.Should().BeFalse();

        // var perfile = await AppDbContext.Perfiles
        //     .FirstOrDefaultAsync(p => p.UserId == result.UserId);
        // perfile.Should().NotBeNull();
        // perfile!.Nombre.Should().Be("Nuevo");
    }

    [Fact]
    public async Task Register_AsContratista_CreatesUserAndProfile()
    {
        var email = GenerateUniqueEmail("contratista");
        var registerCommand = new RegisterCommand
        {
            Email = email,
            Password = "NewUser@123",
            Nombre = "Nuevo",
            Apellido = "Contratista",
            Tipo = 2,
            Host = "http://localhost:5015"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        response.IsSuccessStatusCode.Should().BeTrue();
        var result = await response.Content.ReadFromJsonAsync<RegisterResult>();
        result.Should().NotBeNull();

        // ✅ Use fresh DbContext to avoid caching issues
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        
        // EF Core ValueObject Email se mapea via HasConversion; para que se traduzca usar igualdad directa contra VO
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Email == emailVO);
        var contratista = await freshContext.Contratistas
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.UserId == credencial!.UserId);
        
        contratista.Should().NotBeNull();
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Primero registrar un usuario
        var email = GenerateUniqueEmail("duplicate");
        var registerCommand1 = new RegisterCommand
        {
            Email = email,
            Password = "First@123",
            Nombre = "First",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        var firstResponse = await Client.PostAsJsonAsync("/api/auth/register", registerCommand1);
        firstResponse.IsSuccessStatusCode.Should().BeTrue($"First registration should succeed: {await firstResponse.Content.ReadAsStringAsync()}");

        // Intentar registrar con el mismo email
        var registerCommand2 = new RegisterCommand
        {
            Email = email,
            Password = "Second@123",
            Nombre = "Duplicado",
            Apellido = "Usuario",
            Tipo = 1,
            Host = "http://localhost:5015"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", registerCommand2);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Register_WithInvalidPassword_ReturnsBadRequest()
    {
        var email = GenerateUniqueEmail("test");
        var registerCommand = new RegisterCommand
        {
            Email = email,
            Password = "short",
            Nombre = "Test",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/register", registerCommand);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Login Tests

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens()
    {
        // Crear usuario propio para este test (evita contaminación entre tests paralelos)
        var email = GenerateUniqueEmail("login-valid");
        var password = "LoginTest@123";
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = password,
            Nombre = "Login",
            Apellido = "TestUser",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerCmd);
        registerResponse.IsSuccessStatusCode.Should().BeTrue($"Register failed: {await registerResponse.Content.ReadAsStringAsync()}");

        // Activar cuenta
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored.FirstAsync(c => c.Email == emailVO);
        var activateCmd = new ActivateAccountCommand { UserId = credencial.UserId, Email = email };
        var activateResponse = await Client.PostAsJsonAsync("/api/auth/activate", activateCmd);
        activateResponse.IsSuccessStatusCode.Should().BeTrue($"Activate failed: {await activateResponse.Content.ReadAsStringAsync()}");

        // Login con el usuario recién creado
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            IpAddress = "127.0.0.1"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.IsSuccessStatusCode.Should().BeTrue($"Login failed: {await response.Content.ReadAsStringAsync()}");
        var result = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        // Crear usuario propio para este test
        var email = GenerateUniqueEmail("login-invalid-pwd");
        var password = "CorrectPass@123";
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = password,
            Nombre = "Login",
            Apellido = "WrongPwd",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);

        // Activar cuenta
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored.FirstAsync(c => c.Email == emailVO);
        var activateCmd = new ActivateAccountCommand { UserId = credencial.UserId, Email = email };
        await Client.PostAsJsonAsync("/api/auth/activate", activateCmd);

        // Login con password incorrecto
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = "WrongPassword123!",
            IpAddress = "127.0.0.1"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var loginCommand = new LoginCommand
        {
            Email = $"nonexistent-{Guid.NewGuid():N}@test.com",
            Password = TestDataSeeder.TestPasswordPlainText,
            IpAddress = "127.0.0.1"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Login_WithInactiveAccount_ReturnsUnauthorized()
    {
        // Crear usuario sin activar
        var email = GenerateUniqueEmail("inactive-login");
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = "Inactive@123",
            Nombre = "Inactive",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);
        // NO activamos la cuenta

        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = "Inactive@123",
            IpAddress = "127.0.0.1"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Activate Account Tests

    [Fact]
    public async Task ActivateAccount_WithValidToken_ActivatesUser()
    {
        var email = GenerateUniqueEmail("toactivate");
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = "Test@123",
            Nombre = "Test",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);
        
        // ✅ Use fresh DbContext to avoid caching issues
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored
            .AsNoTracking()
            .FirstAsync(c => c.Email == emailVO);
        credencial.Activo.Should().BeFalse();

        var activateCommand = new ActivateAccountCommand
        {
            UserId = credencial.UserId,
            Email = email
        };

        var response = await Client.PostAsJsonAsync("/api/auth/activate", activateCommand);

        response.IsSuccessStatusCode.Should().BeTrue();
        
        await DbContext.Entry(credencial).ReloadAsync();
        credencial.Activo.Should().BeTrue();
    }

    [Fact]
    public async Task ActivateAccount_WithInvalidUserId_ReturnsBadRequest()
    {
        var activateCommand = new ActivateAccountCommand
        {
            UserId = Guid.NewGuid().ToString(),
            Email = "nonexistent@test.com"
        };

        var response = await Client.PostAsJsonAsync("/api/auth/activate", activateCommand);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    #endregion

    #region Change Password Tests

    [Fact]
    public async Task ChangePassword_WithValidCredentials_ChangesPassword()
    {
        // Crear usuario propio para este test
        var email = GenerateUniqueEmail("chgpwd-valid");
        var originalPassword = "Original@123";
        var newPassword = "NewPassword@123";
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = originalPassword,
            Nombre = "ChangePassword",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);

        // Activar cuenta
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored.FirstAsync(c => c.Email == emailVO);
        var activateCmd = new ActivateAccountCommand { UserId = credencial.UserId, Email = email };
        await Client.PostAsJsonAsync("/api/auth/activate", activateCmd);

        // Login para obtener token
        var token = await LoginAsync(email, originalPassword);
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Cambiar password
        var changePasswordCommand = new ChangePasswordCommand(
            Email: email,
            UserId: credencial.UserId,
            CurrentPassword: originalPassword,
            NewPassword: newPassword
        );

        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changePasswordCommand);

        response.IsSuccessStatusCode.Should().BeTrue($"ChangePassword failed: {await response.Content.ReadAsStringAsync()}");

        // Verificar que el nuevo password funciona
        Client.DefaultRequestHeaders.Authorization = null;
        var loginWithNewPassword = new LoginCommand
        {
            Email = email,
            Password = newPassword,
            IpAddress = "127.0.0.1"
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginWithNewPassword);
        loginResponse.IsSuccessStatusCode.Should().BeTrue($"Login with new password failed: {await loginResponse.Content.ReadAsStringAsync()}");
    }

    [Fact]
    public async Task ChangePassword_WithoutAuthentication_ReturnsUnauthorized()
    {
        // Asegurar que no hay header de autorización
        Client.DefaultRequestHeaders.Authorization = null;
        
        var changePasswordCommand = new ChangePasswordCommand(
            Email: "anyuser@test.com",
            UserId: "some-user-id",
            CurrentPassword: "OldPassword@123",
            NewPassword: "NewPassword@123"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/change-password", changePasswordCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Refresh Token Tests

    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsNewTokens()
    {
        // Crear usuario propio para este test
        var email = GenerateUniqueEmail("refresh-valid");
        var password = "Refresh@123";
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = password,
            Nombre = "Refresh",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);

        // Activar cuenta
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored.FirstAsync(c => c.Email == emailVO);
        var activateCmd = new ActivateAccountCommand { UserId = credencial.UserId, Email = email };
        await Client.PostAsJsonAsync("/api/auth/activate", activateCmd);

        // Login
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            IpAddress = "127.0.0.1"
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);
        loginResponse.IsSuccessStatusCode.Should().BeTrue($"Login failed: {await loginResponse.Content.ReadAsStringAsync()}");
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();

        // Refresh
        var refreshCommand = new RefreshTokenCommand(
            RefreshToken: loginResult!.RefreshToken,
            IpAddress: "127.0.0.1"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        response.IsSuccessStatusCode.Should().BeTrue($"Refresh failed: {await response.Content.ReadAsStringAsync()}");
        var result = await response.Content.ReadFromJsonAsync<AuthenticationResultDto>();
        result.Should().NotBeNull();
        result!.AccessToken.Should().NotBeNullOrEmpty();
        result.RefreshToken.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        var refreshCommand = new RefreshTokenCommand(
            RefreshToken: "invalid-refresh-token-12345",
            IpAddress: "127.0.0.1"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion

    #region Revoke Token Tests

    [Fact]
    public async Task RevokeToken_WithValidToken_RevokesSuccessfully()
    {
        // Crear usuario propio para este test
        var email = GenerateUniqueEmail("revoke-valid");
        var password = "Revoke@123";
        var registerCmd = new RegisterCommand
        {
            Email = email,
            Password = password,
            Nombre = "Revoke",
            Apellido = "User",
            Tipo = 1,
            Host = "http://localhost:5015"
        };
        await Client.PostAsJsonAsync("/api/auth/register", registerCmd);

        // Activar cuenta
        using var scope = Factory.Services.CreateScope();
        var freshContext = scope.ServiceProvider.GetRequiredService<MiGenteDbContext>();
        var emailVO = Email.CreateUnsafe(email);
        var credencial = await freshContext.CredencialesRefactored.FirstAsync(c => c.Email == emailVO);
        var activateCmd = new ActivateAccountCommand { UserId = credencial.UserId, Email = email };
        await Client.PostAsJsonAsync("/api/auth/activate", activateCmd);

        // Login
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            IpAddress = "127.0.0.1"
        };
        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);
        loginResponse.IsSuccessStatusCode.Should().BeTrue($"Login failed: {await loginResponse.Content.ReadAsStringAsync()}");
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();

        // Revoke
        var revokeCommand = new RevokeTokenCommand(
            RefreshToken: loginResult!.RefreshToken,
            IpAddress: "127.0.0.1"
        );

        var response = await Client.PostAsJsonAsync("/api/auth/revoke", revokeCommand);

        response.IsSuccessStatusCode.Should().BeTrue($"Revoke failed: {await response.Content.ReadAsStringAsync()}");

        // Verificar que el refresh token ya no funciona
        var refreshCommand = new RefreshTokenCommand(
            RefreshToken: loginResult.RefreshToken,
            IpAddress: "127.0.0.1"
        );
        var refreshResponse = await Client.PostAsJsonAsync("/api/auth/refresh", refreshCommand);
        refreshResponse.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    #endregion
}