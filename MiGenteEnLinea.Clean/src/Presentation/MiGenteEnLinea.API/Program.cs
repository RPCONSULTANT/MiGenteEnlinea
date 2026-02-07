using MiGenteEnLinea.Infrastructure;
using MiGenteEnLinea.Infrastructure.Persistence.Contexts;
using MiGenteEnLinea.Application;
using MiGenteEnLinea.API.Configuration;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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

var corsOptions = builder.Configuration
    .GetSection(CorsOptions.SectionName)
    .Get<CorsOptions>() ?? new CorsOptions();

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

// ========================================
// BUILD APP
// ========================================
var app = builder.Build();

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

// Swagger (solo en desarrollo)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "MiGente API v1");
        options.RoutePrefix = string.Empty; // Swagger en raíz: https://localhost:5001/
    });
}

// CORS - DEBE IR ANTES DE HttpsRedirection para permitir preflight requests
app.UseCors("AppPolicy");

// HTTPS Redirection (después de CORS para no bloquear preflight)
app.UseHttpsRedirection();

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

// ========================================
// INICIALIZAR BASE DE DATOS Y SEEDING
// ========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    
    try
    {
        var dbContext = services.GetRequiredService<MiGenteDbContext>();
        
        // Verificar conexión a base de datos
        if (await dbContext.Database.CanConnectAsync())
        {
            logger.LogInformation("✅ Conexión a base de datos exitosa");
            
            // Ejecutar seeding si las tablas están vacías
            var seeder = new MiGenteEnLinea.Infrastructure.Persistence.Seeding.DatabaseSeeder(
                dbContext, 
                services.GetRequiredService<ILogger<MiGenteEnLinea.Infrastructure.Persistence.Seeding.DatabaseSeeder>>());
            
            await seeder.SeedAsync();
        }
        else
        {
            logger.LogWarning("⚠️ No se pudo conectar a la base de datos");
        }
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Error durante la inicialización de la base de datos");
    }
}

// ========================================
// RUN APP
// ========================================
try
{
    Log.Information("Iniciando MiGente En Línea API...");
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

// Make Program class accessible to integration tests
public partial class Program { }
