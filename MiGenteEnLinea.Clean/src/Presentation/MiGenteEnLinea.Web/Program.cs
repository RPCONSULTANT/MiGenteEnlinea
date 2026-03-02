using MiGenteEnLinea.Web.Configuration;
using MiGenteEnLinea.Web.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);
ValidateWebCriticalConfiguration(builder.Configuration, builder.Environment);

// ========================================
// CONFIGURACIÓN DE API
// ========================================

// Registrar ApiOptions desde appsettings.json
builder.Services.Configure<ApiOptions>(
    builder.Configuration.GetSection(ApiOptions.SectionName));
builder.Services.Configure<PaymentOptions>(
    builder.Configuration.GetSection(PaymentOptions.SectionName));

// Logging inicial de configuración
var apiOptions = builder.Configuration
    .GetSection(ApiOptions.SectionName)
    .Get<ApiOptions>() ?? new ApiOptions();
var paymentOptions = builder.Configuration
    .GetSection(PaymentOptions.SectionName)
    .Get<PaymentOptions>() ?? new PaymentOptions();

builder.Logging.AddConsole();
builder.Logging.AddDebug();

Console.WriteLine($"🌐 Environment: {builder.Environment.EnvironmentName}");
Console.WriteLine($"🔗 API Base URL: {apiOptions.BaseUrl}");
Console.WriteLine($"⏱️ API Timeout: {apiOptions.TimeoutSeconds}s");
Console.WriteLine($"💳 Payment Mode: {paymentOptions.Mode}");

// ========================================
// CONFIGURACIÓN DE HTTP CLIENT Y API SERVICES
// ========================================

// Register HttpClient for ApiService with base URL and timeout
builder.Services.AddHttpClient<IApiService, ApiService>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<ApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Register typed API services
builder.Services.AddScoped<EmpleadoresApiService>();
builder.Services.AddScoped<ContratistasApiService>();
builder.Services.AddScoped<SuscripcionesApiService>();

Console.WriteLine("✅ API Services registered successfully");

// Add services to the container.
builder.Services.AddControllersWithViews();

// Session configuration (for logout functionality)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
    app.UseHttpsRedirection(); // Solo en producción
}

app.UseStaticFiles();
app.UseRouting();

app.UseSession(); // Must be before UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

static void ValidateWebCriticalConfiguration(IConfiguration configuration, IWebHostEnvironment environment)
{
    var errors = new List<string>();
    var sensitivePlaceholders = new[] { "YOUR_", "CHANGE_ME", "REPLACE_ME", "TODO", "example", "placeholder" };
    var strictPlaceholderCheck = environment.IsStaging() || environment.IsProduction();

    string? Read(string key) => configuration[key];

    void Require(string key, string description)
    {
        var value = Read(key);
        if (string.IsNullOrWhiteSpace(value))
        {
            errors.Add($"{key} requerido ({description}).");
            return;
        }

        if (strictPlaceholderCheck && sensitivePlaceholders.Any(p => value.Contains(p, StringComparison.OrdinalIgnoreCase)))
        {
            errors.Add($"{key} no puede usar placeholder en {environment.EnvironmentName}.");
        }
    }

    Require("ApiConfiguration:BaseUrl", "URL base del API");
    Require("ApiConfiguration:StaticFilesBaseUrl", "URL base de archivos estáticos");

    var timeoutRaw = Read("ApiConfiguration:TimeoutSeconds");
    if (int.TryParse(timeoutRaw, out var timeoutSeconds) && timeoutSeconds <= 0)
    {
        errors.Add("ApiConfiguration:TimeoutSeconds debe ser mayor que 0.");
    }

    var paymentMode = Read("PaymentConfiguration:Mode");
    if (!string.IsNullOrWhiteSpace(paymentMode) &&
        !string.Equals(paymentMode, "fake", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(paymentMode, "real", StringComparison.OrdinalIgnoreCase))
    {
        errors.Add("PaymentConfiguration:Mode debe ser 'fake' o 'real'.");
    }

    if (errors.Count > 0)
    {
        throw new InvalidOperationException(
            "Configuración crítica incompleta o insegura en Web:\n - " + string.Join("\n - ", errors));
    }
}
