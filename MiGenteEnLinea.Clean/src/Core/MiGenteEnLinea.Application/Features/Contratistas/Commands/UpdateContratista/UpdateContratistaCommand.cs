using MediatR;

namespace MiGenteEnLinea.Application.Features.Contratistas.Commands.UpdateContratista;

/// <summary>
/// Command: Actualiza el perfil de un contratista existente
/// </summary>
/// <remarks>
/// LÓGICA LEGACY: ContratistasService.GuardarPerfil()
/// REPLICACIÓN: index_contratista.aspx.cs → btnGuardar_Click()
/// COMPORTAMIENTO: Actualiza solo los campos que no sean null (partial update)
/// </remarks>
/// <param name="UserId">ID del usuario (identifica al contratista)</param>
/// <param name="Titulo">Título profesional (opcional, max 70 caracteres)</param>
/// <param name="Sector">Sector económico (opcional, max 40 caracteres)</param>
/// <param name="Experiencia">Años de experiencia (opcional)</param>
/// <param name="Presentacion">Descripción/biografía (opcional, max 250 caracteres)</param>
/// <param name="Provincia">Provincia donde opera (opcional, max 50 caracteres)</param>
/// <param name="NivelNacional">¿Trabaja a nivel nacional? (opcional)</param>
/// <param name="Telefono1">Teléfono principal (opcional, max 16 caracteres)</param>
/// <param name="Whatsapp1">¿Teléfono1 es WhatsApp? (opcional)</param>
/// <param name="Telefono2">Teléfono secundario (opcional, max 20 caracteres)</param>
/// <param name="Whatsapp2">¿Teléfono2 es WhatsApp? (opcional)</param>
/// <param name="Email">Email de contacto (opcional, max 50 caracteres)</param>
public record UpdateContratistaCommand(
    string UserId,
    int? Tipo = null,
    string? Identificacion = null,
    string? Nombre = null,
    string? Apellido = null,
    string? NombreComercial = null,
    string? Titulo = null,
    string? Sector = null,
    int? Experiencia = null,
    string? Presentacion = null,
    string? Provincia = null,
    bool? NivelNacional = null,
    string? Telefono1 = null,
    bool? Whatsapp1 = null,
    string? Telefono2 = null,
    bool? Whatsapp2 = null,
    string? Email = null
) : IRequest;
