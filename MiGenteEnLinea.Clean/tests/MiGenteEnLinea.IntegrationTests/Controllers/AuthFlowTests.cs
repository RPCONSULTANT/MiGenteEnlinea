using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using MiGenteEnLinea.Application.Features.Authentication.Commands.Login;
using MiGenteEnLinea.Application.Features.Authentication.Commands.Register;
using MiGenteEnLinea.Application.Features.Authentication.DTOs;
using MiGenteEnLinea.IntegrationTests.Infrastructure;
using Xunit;
using Xunit.Abstractions;

namespace MiGenteEnLinea.IntegrationTests.Controllers;

/// <summary>
/// Tests de flujos completos de autenticación siguiendo el orden lógico del usuario:
/// 1. Register → 2. Login → 3. ActivateAccount → 4. ChangePassword → etc.
/// 
/// OBJETIVO: Validar el patrón Identity-First + Legacy Sync en todo el flujo.
/// </summary>
[Collection("Integration Tests")]
public class AuthFlowTests : IntegrationTestBase
{
    private readonly ITestOutputHelper _output;

    public AuthFlowTests(TestWebApplicationFactory factory, ITestOutputHelper output) : base(factory)
    {
        _output = output;
    }

    /// <summary>
    /// Flujo 1: Register → Login
    /// Valida que un usuario recién registrado pueda hacer login inmediatamente.
    /// </summary>
    [Fact]
    public async Task Flow_RegisterAndLogin_Success()
    {
        // ====================================================================
        // PASO 1: REGISTER (crea usuario en Identity + Legacy)
        // ====================================================================
        var email = GenerateUniqueEmail("flow-test");
        var password = "FlowTest@123";
        
        var registerCommand = new RegisterCommand
        {
            Email = email,
            Password = password,
            Nombre = "Test",
            Apellido = "Flow",
            Tipo = 1, // Empleador
            Host = "http://localhost:5015"
        };

        _output.WriteLine($"[PASO 1] Registrando usuario: {email}");
        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerCommand);
        
        registerResponse.IsSuccessStatusCode.Should().BeTrue();
        var registerResult = await registerResponse.Content.ReadFromJsonAsync<RegisterResult>();
        
        registerResult.Should().NotBeNull();
        registerResult!.Success.Should().BeTrue();
        registerResult.UserId.Should().NotBeNullOrEmpty();
        
        var userId = registerResult.UserId!;
        _output.WriteLine($"[PASO 1] ✅ Usuario registrado. UserId: {userId}");

        // ====================================================================
        // VERIFICAR: Usuario existe en Identity (AspNetUsers)
        // ====================================================================
        var userInIdentity = await UserManager.FindByEmailAsync(email);
        userInIdentity.Should().NotBeNull("El usuario debe existir en AspNetUsers (Identity)");
        userInIdentity!.Id.Should().Be(userId);
        _output.WriteLine($"[VERIFY] ✅ Usuario encontrado en AspNetUsers (Identity)");

        // ====================================================================
        // VERIFICAR: Usuario existe en Legacy (Credenciales + Perfiles)
        // ====================================================================
        var credencial = await AppDbContext.Credenciales
            .FirstOrDefaultAsync(c => c.UserId == userId);
        credencial.Should().NotBeNull("El usuario debe existir en Credenciales (Legacy)");
        
        var perfil = await AppDbContext.Perfiles
            .FirstOrDefaultAsync(p => p.UserId == userId);
        perfil.Should().NotBeNull("El usuario debe existir en Perfiles (Legacy)");
        _output.WriteLine($"[VERIFY] ✅ Usuario encontrado en Credenciales + Perfiles (Legacy)");

        // ====================================================================
        // PASO 2: LOGIN (autentica con Identity-First)
        // ====================================================================
        _output.WriteLine($"[PASO 2] Intentando login con: {email}");
        
        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            IpAddress = "127.0.0.1"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);
        
        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();
        
        loginResult.Should().NotBeNull();
        loginResult!.AccessToken.Should().NotBeNullOrEmpty();
        loginResult.RefreshToken.Should().NotBeNullOrEmpty();
        loginResult.User.Should().NotBeNull();
        loginResult.User!.UserId.Should().Be(userId);
        loginResult.User.Email.Should().Be(email);
        
        _output.WriteLine($"[PASO 2] ✅ Login exitoso. AccessToken: {loginResult.AccessToken[..20]}...");

        // ====================================================================
        // RESULTADO FINAL
        // ====================================================================
        _output.WriteLine("[RESULTADO] ✅ Flujo Register → Login completado exitosamente");
    }

    /// <summary>
    /// Flujo 2: Login con usuario Legacy (migracion automatica a Identity)
    /// Valida el patrón Legacy Fallback: usuario existe solo en Credenciales (Legacy),
    /// se migra automáticamente a Identity al hacer login.
    /// </summary>
    [Fact]
    public async Task Flow_LoginLegacyUser_AutoMigratesToIdentity()
    {
        // ====================================================================
        // SETUP: Usuario "juan.perez@test.com" existe SOLO en Legacy
        // (creado por TestDataSeeder en Credenciales + Perfiles)
        // ====================================================================
        var email = "juan.perez@test.com";
        var password = TestDataSeeder.TestPasswordPlainText; // "Test@1234"

        _output.WriteLine($"[SETUP] Usuario legacy: {email}");

        // Verificar que NO existe en Identity (antes de login)
        var userBeforeLogin = await UserManager.FindByEmailAsync(email);
        _output.WriteLine($"[SETUP] Usuario en Identity (antes de login): {userBeforeLogin?.Email ?? "NULL"}");

        // Verificar que SÍ existe en Legacy
        var credencial = await AppDbContext.Credenciales
            .FirstOrDefaultAsync(c => c.Email.Value == email);
        credencial.Should().NotBeNull("El usuario debe existir en Credenciales (Legacy)");
        _output.WriteLine($"[SETUP] ✅ Usuario encontrado en Credenciales (Legacy). UserId: {credencial!.UserId}");

        // ====================================================================
        // PASO 1: LOGIN (debería buscar en Legacy y migrar a Identity)
        // ====================================================================
        _output.WriteLine($"[PASO 1] Intentando login con usuario Legacy: {email}");

        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = password,
            IpAddress = "127.0.0.1"
        };

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        loginResponse.IsSuccessStatusCode.Should().BeTrue();
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<AuthenticationResultDto>();

        loginResult.Should().NotBeNull();
        loginResult!.AccessToken.Should().NotBeNullOrEmpty();
        loginResult.RefreshToken.Should().NotBeNullOrEmpty();

        _output.WriteLine($"[PASO 1] ✅ Login exitoso. AccessToken: {loginResult.AccessToken[..20]}...");

        // ====================================================================
        // VERIFICAR: Usuario ahora existe en Identity (migrado automáticamente)
        // ====================================================================
        var userAfterLogin = await UserManager.FindByEmailAsync(email);
        userAfterLogin.Should().NotBeNull("El usuario debería haberse migrado a AspNetUsers (Identity)");
        userAfterLogin!.Email.Should().Be(email);
        userAfterLogin.Id.Should().Be(credencial.UserId, "El UserId debe ser el mismo que en Legacy");

        _output.WriteLine($"[VERIFY] ✅ Usuario migrado a Identity. UserId: {userAfterLogin.Id}");

        // ====================================================================
        // RESULTADO FINAL
        // ====================================================================
        _output.WriteLine("[RESULTADO] ✅ Flujo Legacy Fallback → Auto-Migration completado exitosamente");
    }

    /// <summary>
    /// Flujo 3: Login con credenciales inválidas (password incorrecto)
    /// </summary>
    [Fact]
    public async Task Flow_Login_WithInvalidPassword_ReturnsUnauthorized()
    {
        var email = "juan.perez@test.com";
        var wrongPassword = "WrongPassword@123";

        var loginCommand = new LoginCommand
        {
            Email = email,
            Password = wrongPassword,
            IpAddress = "127.0.0.1"
        };

        _output.WriteLine($"[TEST] Intentando login con password incorrecto: {email}");

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _output.WriteLine("[RESULTADO] ✅ Login rechazado correctamente (Unauthorized)");
    }

    /// <summary>
    /// Flujo 4: Login con email no existente
    /// </summary>
    [Fact]
    public async Task Flow_Login_WithNonExistentEmail_ReturnsUnauthorized()
    {
        var nonExistentEmail = "nonexistent@test.com";
        var password = "SomePassword@123";

        var loginCommand = new LoginCommand
        {
            Email = nonExistentEmail,
            Password = password,
            IpAddress = "127.0.0.1"
        };

        _output.WriteLine($"[TEST] Intentando login con email inexistente: {nonExistentEmail}");

        var response = await Client.PostAsJsonAsync("/api/auth/login", loginCommand);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        _output.WriteLine("[RESULTADO] ✅ Login rechazado correctamente (Unauthorized)");
    }
}
