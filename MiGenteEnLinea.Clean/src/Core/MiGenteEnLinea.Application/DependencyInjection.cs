using System.Reflection;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using MiGenteEnLinea.Application.Common.Behaviors;
using MiGenteEnLinea.Application.Features.Dashboard.Services;

namespace MiGenteEnLinea.Application;

/// <summary>
/// Extensión para registrar todos los servicios de Application en el contenedor DI
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // ========================================
        // MEDIATR (CQRS Pattern)
        // ========================================
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
        });

        // ========================================
        // FLUENT VALIDATION
        // ========================================
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        // ========================================
        // AUTOMAPPER (Object Mapping)
        // ========================================
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // ========================================
        // MEMORY CACHE & DASHBOARD CACHING
        // ========================================
        services.AddMemoryCache();
        services.AddScoped<IDashboardCacheService, DashboardCacheService>();

        return services;
    }
}
