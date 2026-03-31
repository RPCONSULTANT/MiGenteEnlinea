using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Features.Empleados.Queries.ConsultarPadron;

/// <summary>
/// Handler para consultar cédula en el Padrón Nacional Dominicano.
/// Utiliza IPadronService para integración con API externa.
/// </summary>
public class ConsultarPadronQueryHandler : IRequestHandler<ConsultarPadronQuery, PadronResultDto?>
{
    private readonly IPadronService _padronService;
    private readonly ILogger<ConsultarPadronQueryHandler> _logger;

    public ConsultarPadronQueryHandler(
        IPadronService padronService,
        ILogger<ConsultarPadronQueryHandler> logger)
    {
        _padronService = padronService;
        _logger = logger;
    }

    public async Task<PadronResultDto?> Handle(ConsultarPadronQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Consultando Padrón Nacional para cédula: {Cedula}", request.Cedula);

        // PASO 1: Consultar Padrón usando el servicio
        var padronData = await _padronService.ConsultarCedulaAsync(request.Cedula, cancellationToken);

        if (padronData == null)
        {
            _logger.LogWarning("No se encontró información en el Padrón para la cédula: {Cedula}", request.Cedula);
            return null;
        }

        _logger.LogInformation(
            "Información del Padrón obtenida exitosamente: {Cedula} - {Nombre}",
            padronData.Cedula,
            padronData.NombreCompleto);

        // PASO 2: Mapear a DTO de respuesta
        return new PadronResultDto
        {
            Cedula = padronData.Cedula,
            NombreCompleto = padronData.NombreCompleto,
            Nombres = padronData.Nombres,
            PrimerApellido = padronData.Apellido1,
            SegundoApellido = padronData.Apellido2,
            Photo = padronData.Photo,
            FechaNacimiento = padronData.FechaNacimiento,
            Edad = padronData.Edad, // Edad calculada en PadronModel
            LugarNacimiento = padronData.LugarNacimiento,
            EstadoCivil = padronData.EstadoCivil,
            Ocupacion = padronData.Ocupacion
        };
    }
}
