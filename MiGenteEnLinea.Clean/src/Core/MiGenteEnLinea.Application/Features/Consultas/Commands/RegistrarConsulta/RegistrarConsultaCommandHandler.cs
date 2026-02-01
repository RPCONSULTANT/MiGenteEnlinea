using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MiGenteEnLinea.Application.Common.Interfaces;
using MiGenteEnLinea.Domain.Entities.Consultas;

namespace MiGenteEnLinea.Application.Features.Consultas.Commands.RegistrarConsulta;

/// <summary>
/// Handler para registrar una consulta de perfil
/// Evita duplicados si el mismo empleador consultó el mismo contratista en los últimos 5 minutos
/// </summary>
public class RegistrarConsultaCommandHandler : IRequestHandler<RegistrarConsultaCommand, int>
{
    private readonly IApplicationDbContext _context;
    private readonly ILogger<RegistrarConsultaCommandHandler> _logger;

    public RegistrarConsultaCommandHandler(
        IApplicationDbContext context,
        ILogger<RegistrarConsultaCommandHandler> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> Handle(RegistrarConsultaCommand request, CancellationToken cancellationToken)
    {
        // Evitar registrar duplicados si la misma consulta fue hecha en los últimos 5 minutos
        var limiteAntidupe = DateTime.UtcNow.AddMinutes(-5);
        
        var consultaReciente = await _context.ConsultasPerfil
            .AsNoTracking()
            .Where(c => c.EmpleadorUserId == request.EmpleadorUserId 
                     && c.ContratistaIdentificacion == request.ContratistaIdentificacion
                     && c.FechaConsulta > limiteAntidupe)
            .AnyAsync(cancellationToken);

        if (consultaReciente)
        {
            _logger.LogDebug(
                "Consulta duplicada ignorada - Empleador: {EmpleadorUserId}, Contratista: {ContratistaIdentificacion}",
                request.EmpleadorUserId,
                request.ContratistaIdentificacion);
            return 0; // No se registró porque ya existe una consulta reciente
        }

        var consulta = ConsultaPerfil.Crear(
            request.EmpleadorUserId,
            request.ContratistaIdentificacion,
            request.IpAddress);

        await _context.ConsultasPerfil.AddAsync(consulta, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        _logger.LogInformation(
            "Consulta registrada - ID: {ConsultaId}, Empleador: {EmpleadorUserId}, Contratista: {ContratistaIdentificacion}",
            consulta.Id,
            request.EmpleadorUserId,
            request.ContratistaIdentificacion);

        return consulta.Id;
    }
}
