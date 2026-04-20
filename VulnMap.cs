public static class SogilubVulnMapper
{
    private static bool IsOneOf(string? value, params string[] options)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var normalized = value.Trim();

        return options.Any(option => normalized.Equals(option, StringComparison.OrdinalIgnoreCase));
    }

    // normaliza para maiúsculas antes de comparar
    public static VulnSeverity MapSeverity(string? raw) => raw?.Trim().ToUpperInvariant() switch
    {
        "CRITICAL" => VulnSeverity.Critical,
        "HIGH"     => VulnSeverity.High,
        "MEDIUM"   => VulnSeverity.Medium,
        "LOW"      => VulnSeverity.Low,
        "INFO"     => VulnSeverity.Info,
        _          => VulnSeverity.Unknown
    };

    // converte o score CVSS de texto para decimal
    // InvariantCulture para que "8.8" seja lido igual qualquer que seja o idioma do servidor
    public static decimal? ParseCvss(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw) || raw.Trim().Equals("N/A", StringComparison.OrdinalIgnoreCase))
            return null;

        return decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : null;
    }
}
