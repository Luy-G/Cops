using CyberOps.Domain.Enums;

namespace CyberOps.Application.Clients.Sogilub.Vuln;

public static class SogilubVulnMapper
{
    public static VulnSeverity MapSeverity(string? raw) => raw?.Trim().ToUpperInvariant() switch
    {
        "CRITICAL" => VulnSeverity.Critical,
        "HIGH" => VulnSeverity.High,
        "MEDIUM" => VulnSeverity.Medium,
        "LOW" => VulnSeverity.Low,
        "INFO" => VulnSeverity.Info,
        _ => VulnSeverity.Unknown
    };

    public static decimal? ParseCvss(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw.Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return decimal.TryParse(
            raw,
            System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture,
            out var result)
            ? result
            : null;
    }
}
