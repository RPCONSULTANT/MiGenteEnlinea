using MiGenteEnLinea.Infrastructure;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using MiGenteEnLinea.Application;
using MiGenteEnLinea.API.Configuration;
using MiGenteEnLinea.Infrastructure.Persistence.Seeding;
using MiGenteEnLinea.Infrastructure.Options;
using MiGenteEnLinea.API.Services;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.Features;
using System.Text;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
ValidateApiCriticalConfiguration(builder.Configuration, builder.Environment);

// ========================================
// CONFIGURACIÓN DE LOGGING CON SERILOG
// ========================================
var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithProperty("Application", "MiGenteEnLinea.API")
    .Enrich.WithProperty("Environment", builder.Environment.EnvironmentName)
    .WriteTo.Console()
    .WriteTo.File("logs/migente-.txt", rollingInterval: RollingInterval.Day);

// Intentar agregar SQL Server sink (opcional si DB no está disponible)
try
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    if (!string.IsNullOrEmpty(connectionString))
    {
        loggerConfig.WriteTo.MSSqlServer(
            connectionString: connectionString,
            sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
            {
                TableName = "Logs",
                AutoCreateSqlTable = true
            });
        Console.WriteLine("✅ Serilog: SQL Server sink configurado");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"⚠️ Serilog: No se pudo conectar a SQL Server para logs. Continuando con Console y File sinks. Error: {ex.Message}");
}

Log.Logger = loggerConfig.CreateLogger();

builder.Host.UseSerilog();

// ========================================
// REGISTRAR CAPAS (Dependency Injection)
// ========================================

// Infrastructure Layer (DbContext, Identity, Services)
builder.Services.AddInfrastructure(builder.Configuration);

// Application Layer (MediatR, Validators, Mappings)
builder.Services.AddApplication();

// ========================================
// ASP.NET CORE SERVICES
// ========================================

// HttpContext para CurrentUserService
builder.Services.AddHttpContextAccessor();

// Controllers con configuración de JSON (camelCase para compatibilidad con JavaScript/REST estándar)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase; // camelCase para REST API
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Desactivar respuesta automática 400 para ModelState inválido (permite logging manual en controladores)
builder.Services.Configure<Microsoft.AspNetCore.Mvc.ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});

// API Explorer para Swagger
builder.Services.AddEndpointsApiExplorer();

// Swagger con autenticación JWT
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "MiGente En Línea API",
        Version = "v1",
        Description = "API para gestión de empleadores, contratistas y nómina en República Dominicana",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "MiGente Support",
            Email = "soporte@migenteenlinea.com"
        }
    });

    // JWT Authentication en Swagger
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando el esquema Bearer. Ejemplo: 'Bearer {token}'",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ========================================
// JWT AUTHENTICATION
// ========================================
var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["SecretKey"];

if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
{
    throw new InvalidOperationException("JWT SecretKey debe tener al menos 32 caracteres. Configurar en appsettings.json o User Secrets.");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // Solo HTTP en desarrollo
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Sin tolerancia de tiempo
    };

    // Logging de eventos de autenticación
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Log.Warning("JWT Authentication failed: {Error}", context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var userId = context.Principal?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            Log.Information("JWT validated for user: {UserId}", userId);
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Log.Warning("JWT Challenge: {Error}", context.Error);
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// ========================================
// CORS (Configuration-driven)
// ========================================
builder.Services.Configure<CorsOptions>(
    builder.Configuration.GetSection(CorsOptions.SectionName));
builder.Services.Configure<DatabaseInitializationOptions>(
    builder.Configuration.GetSection(DatabaseInitializationOptions.SectionName));
builder.Services.Configure<DatabaseSeedingSecurityOptions>(
    builder.Configuration.GetSection(DatabaseSeedingSecurityOptions.SectionName));
builder.Services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();

var corsOptions = builder.Configuration
    .GetSection(CorsOptions.SectionName)
    .Get<CorsOptions>() ?? new CorsOptions();

if (builder.Environment.IsProduction() && corsOptions.AllowedOrigins.Length == 0)
{
    throw new InvalidOperationException(
        "CorsConfiguration.AllowedOrigins no puede estar vacio en Production. Configure origenes explicitos.");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AppPolicy", policy =>
    {
        // AllowedOrigins
        if (corsOptions.AllowedOrigins.Length > 0)
            policy.WithOrigins(corsOptions.AllowedOrigins);
        else
            policy.AllowAnyOrigin();

        // AllowedMethods
        if (corsOptions.AllowedMethods.Length > 0)
            policy.WithMethods(corsOptions.AllowedMethods);
        else
            policy.AllowAnyMethod();

        // AllowedHeaders
        if (corsOptions.AllowedHeaders.Length > 0)
            policy.WithHeaders(corsOptions.AllowedHeaders);
        else
            policy.AllowAnyHeader();

        // ExposedHeaders
        if (corsOptions.ExposedHeaders.Length > 0)
            policy.WithExposedHeaders(corsOptions.ExposedHeaders);

        // AllowCredentials (cannot be used with AllowAnyOrigin)
        if (corsOptions.AllowCredentials && corsOptions.AllowedOrigins.Length > 0)
            policy.AllowCredentials();

        // Preflight cache
        policy.SetPreflightMaxAge(TimeSpan.FromSeconds(corsOptions.MaxAgeSeconds));
    });
});

var fileStorageSettings = builder.Configuration.GetSection(FileStorageOptions.SectionName).Get<FileStorageOptions>() ?? new FileStorageOptions();
var maxUploadBytes = Math.Max(1, fileStorageSettings.MaxFileSizeMB) * 1024L * 1024L;
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = maxUploadBytes;
    options.MultipartHeadersLengthLimit = 64 * 1024;
});

// ========================================
// BUILD APP
// ========================================
var app = builder.Build();
var dbInitOptions = builder.Configuration
    .GetSection(DatabaseInitializationOptions.SectionName)
    .Get<DatabaseInitializationOptions>() ?? DatabaseInitializationOptions.CreateDefaults(app.Environment.EnvironmentName);
var paymentOptions = builder.Configuration
    .GetSection(PaymentProcessingOptions.SectionName)
    .Get<PaymentProcessingOptions>() ?? new PaymentProcessingOptions();

var programAssembly = typeof(Program).Assembly;
var informationalVersion = programAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
    ?? programAssembly.GetName().Version?.ToString()
    ?? "unknown";
var deploymentCommit = Environment.GetEnvironmentVariable("BUILD_COMMIT")
    ?? Environment.GetEnvironmentVariable("BUILD_SOURCEVERSION")
    ?? Environment.GetEnvironmentVariable("GITHUB_SHA")
    ?? "not-set";

// ========================================
// MIDDLEWARE PIPELINE
// ========================================

// Serilog Request Logging
app.UseSerilogRequestLogging();

// Exception Handling - Usar nuestro middleware global para todos los ambientes
// Este convierte excepciones de dominio a respuestas HTTP apropiadas
app.UseMiddleware<MiGenteEnLinea.API.Middleware.GlobalExceptionHandlerMiddleware>();

// En desarrollo, también agregar detalles adicionales
if (app.Environment.IsDevelopment())
{
    // Swagger antes del error handler para que esté disponible
}

// Swagger habilitado para diagnostico y validacion en ambientes desplegados.
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "MiGente API v1");
    options.RoutePrefix = string.Empty; // Swagger en raiz: https://api-dominio/
});

// Routing debe ejecutarse antes de CORS para que el middleware resuelva endpoint metadata correctamente.
app.UseRouting();

// Diagnostico temporal para requests CORS/preflight en ambientes desplegados.
app.Use(async (context, next) =>
{
    await next();

    var isCorsRequest = context.Request.Headers.ContainsKey("Origin");
    if (!isCorsRequest)
    {
        return;
    }

    var method = context.Request.Method;
    var isPreflight = HttpMethods.IsOptions(method) &&
        context.Request.Headers.ContainsKey("Access-Control-Request-Method");

    var origin = context.Request.Headers.Origin.ToString();
    var acao = context.Response.Headers.AccessControlAllowOrigin.ToString();

    Log.Information(
        "CORS request processed. Method={Method}, Path={Path}, IsPreflight={IsPreflight}, Origin={Origin}, StatusCode={StatusCode}, ACAO={ACAO}",
        method,
        context.Request.Path.Value,
        isPreflight,
        origin,
        context.Response.StatusCode,
        string.IsNullOrWhiteSpace(acao) ? "<none>" : acao);
});

// CORS - DEBE IR ANTES DE HttpsRedirection para permitir preflight requests
app.UseCors("AppPolicy");

// HTTPS Redirection (después de CORS para no bloquear preflight)
app.UseHttpsRedirection();

// Static Files - Servir archivos desde wwwroot (imágenes, documentos, etc.)
app.UseStaticFiles();

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Health Check Endpoint
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Environment = app.Environment.EnvironmentName
}));

await InitializeDatabaseAsync(app, dbInitOptions);

// ========================================
// RUN APP
// ========================================
try
{
    Log.Information("Iniciando MiGente En Línea API...");
    Log.Information("Runtime version info. AssemblyVersion={AssemblyVersion}, DeploymentCommit={DeploymentCommit}",
        informationalVersion,
        deploymentCommit);
    Log.Information(
        "payment.mode.selected PaymentMode={PaymentMode}, AllowSimpleCheckout={AllowSimpleCheckout}, RequireCardValidationInFakeMode={RequireCardValidationInFakeMode}",
        paymentOptions.Mode,
        paymentOptions.AllowSimpleCheckout,
        paymentOptions.RequireCardValidationInFakeMode);
    app.Run();
    Log.Information("API detenida correctamente.");
}
catch (Exception ex)
{
    Log.Fatal(ex, "La aplicación falló al iniciar.");
}
finally
{
    Log.CloseAndFlush();
}

static async Task InitializeDatabaseAsync(WebApplication app, DatabaseInitializationOptions options)
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();

    if (!options.ApplyMigrationsOnStartup && !options.RunCatalogSeedOnStartup && !options.RunDemoSeedOnStartup)
    {
        logger.LogInformation("Inicializacion de BD deshabilitada por configuracion.");
        return;
    }

    try
    {
        var dbContext = services.GetRequiredService<MiGenteDbContext>();

        if (options.ApplyMigrationsOnStartup)
        {
            logger.LogInformation("Aplicando migraciones de base de datos...");
            await dbContext.Database.MigrateAsync();
            logger.LogInformation("Migraciones aplicadas.");
        }
        else if (!await dbContext.Database.CanConnectAsync())
        {
            throw new InvalidOperationException("No se pudo conectar a la base de datos y ApplyMigrationsOnStartup=false.");
        }

        if (options.RunCatalogSeedOnStartup || options.RunDemoSeedOnStartup)
        {
            var seeder = services.GetRequiredService<DatabaseSeeder>();
            await seeder.SeedAsync(options.RunDemoSeedOnStartup);

            if (!options.RunCatalogSeedOnStartup && options.RunDemoSeedOnStartup)
            {
                logger.LogWarning("RunDemoSeedOnStartup=true implica catalogos + demo via DatabaseSeeder.");
            }
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Error durante la inicializacion de base de datos.");

        if (options.FailFastOnInitializationError)
        {
            throw;
        }
    }
}

static void ValidateApiCriticalConfiguration(IConfiguration configuration, IWebHostEnvironment environment)
{
    var errors = new List<string>();
    var sensitivePlaceholders = new[] { "YOUR_", "CHANGE_ME", "REPLACE_ME", "TODO", "example", "placeholder" };
    var strictPlaceholderCheck = environment.IsStaging() || environment.IsProduction();

    string? Read(string key) => configuration[key];

    void Require(string key, string description, bool secret = false, bool minJwtLength = false)
    {
        var value = Read(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{key} requerido ({description}).");
            return;
        }

        if (minJwtLength && value.Length < 32)
        {
            errors.Add($"{key} debe tener al menos 32 caracteres.");
        }

        if (strictPlaceholderCheck)
        {
            var normalized = value.Trim();
            if (sensitivePlaceholders.Any(p => normalized.Contains(p, StringComparison.OrdinalIgnoreCase)))
            {
                errors.Add($"{key} no puede usar placeholder en {environment.EnvironmentName}.");
            }
        }
    }

    Require("ConnectionStrings:DefaultConnection", "cadena de conexión principal");
    Require("Jwt:SecretKey", "firma JWT", secret: true, minJwtLength: true);
    Require("Jwt:Issuer", "issuer JWT");
    Require("Jwt:Audience", "audience JWT");

    Require("EmailSettings:FromEmail", "remitente de email");
    Require("EmailSettings:SmtpServer", "host SMTP");
    Require("EmailSettings:Username", "usuario SMTP", secret: true);
    Require("EmailSettings:Password", "password SMTP", secret: true);

    Require("PadronAPI:BaseUrl", "URL del padrón");
    Require("PadronAPI:Username", "usuario del padrón", secret: true);
    Require("PadronAPI:Password", "password del padrón", secret: true);

    Require("AuthLinks:PublicWebBaseUrl", "base URL pública web");
    Require("FileStorage:PublicBaseUrl", "base URL pública para archivos");

    var paymentMode = Read("PaymentProcessing:Mode");
    if (!string.IsNullOrWhiteSpace(paymentMode) &&
        !string.Equals(paymentMode, "Fake", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(paymentMode, "Real", StringComparison.OrdinalIgnoreCase))
    {
        errors.Add("PaymentProcessing:Mode debe ser 'Fake' o 'Real'.");
    }

    if (environment.IsProduction())
    {
        var corsOrigins = configuration.GetSection("CorsConfiguration:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
        if (corsOrigins.Length == 0)
        {
            errors.Add("CorsConfiguration:AllowedOrigins debe tener al menos un origen en Production.");
        }
    }

    if (errors.Count > 0)
    {
        throw new InvalidOperationException(
            "Configuración crítica incompleta o insegura:\n - " + string.Join("\n - ", errors));
    }
}

// Make Program class accessible to integration tests
public partial class Program { }
