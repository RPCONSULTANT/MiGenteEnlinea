using System.Globalization;
using System.Text;
using MiGenteEnLinea.Application.Common.Interfaces;

namespace MiGenteEnLinea.Application.Common.Helpers;

internal static class PadronIdentityValidator
{
    public static bool Matches(
        string nombre,
        string apellido,
        DateTime? fechaNacimiento,
        PadronModel padronData,
        out string validationMessage)
    {
        var mismatches = new List<string>();

        var nombreIngresado = Normalize(nombre);
        var apellidoIngresado = Normalize(apellido);
        var nombrePadron = Normalize(padronData.Nombres);
        var apellidoPadron = Normalize($"{padronData.Apellido1} {padronData.Apellido2}".Trim());

        if (!string.IsNullOrWhiteSpace(nombreIngresado) &&
            !string.Equals(nombreIngresado, nombrePadron, StringComparison.Ordinal))
        {
            mismatches.Add($"los nombres no coinciden con la cédula ({padronData.Nombres})");
        }

        if (!string.IsNullOrWhiteSpace(apellidoIngresado) &&
            !string.Equals(apellidoIngresado, apellidoPadron, StringComparison.Ordinal))
        {
            mismatches.Add($"los apellidos no coinciden con la cédula ({($"{padronData.Apellido1} {padronData.Apellido2}".Trim())})");
        }

        if (fechaNacimiento.HasValue &&
            padronData.FechaNacimiento.HasValue &&
            fechaNacimiento.Value.Date != padronData.FechaNacimiento.Value.Date)
        {
            mismatches.Add($"la fecha de nacimiento no coincide con la cédula ({padronData.FechaNacimiento:yyyy-MM-dd})");
        }

        validationMessage = mismatches.Count == 0
            ? string.Empty
            : $"Los datos del colaborador no coinciden con el padrón nacional: {string.Join(", ", mismatches)}.";

        return mismatches.Count == 0;
    }

    private static string Normalize(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Trim().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return string.Join(" ", builder.ToString()
            .Normalize(NormalizationForm.FormC)
            .ToUpperInvariant()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries));
    }
}
