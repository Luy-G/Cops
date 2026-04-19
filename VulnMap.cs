public static class SogilubVulnMapper
{
    //lista de palavras que indicam que um finding tem um exploit público conhecido,
    // se alguma destas palavras aparecer no título ou descrição: HasPublicExploit = true.
    private static readonly string[] PublicExploitKeywords =
    [
        "poodle", "logjam", "eternalblue", "bluekeep",
        "actively exploited", "public exploit", "exploit available",
        "known exploit", "weaponized"
    ];

    //lista de palavras que indicam que o finding está exposto à internet.
    //JSON da Sogilub tem o valor sempre false(o scan foi feito na rede interna)
    // quando um cliente tiver serviços expostos externamente, estas palavras aparecerão nas descrições.
    private static readonly string[] InternetExposedKeywords =
    [
        "internet-facing", "internet facing", "externally exposed",
        "external exposure", "public-facing", "accessible from internet",
        "publicly accessible", "exposed to internet"
    ];

    // converte a severidade em texto para o enum VulnSeverity
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

    //ve se o finding tem exploit público, procura palavras no título e descrição
    public static bool DetectPublicExploit(string? title, string? description)
    {
        var text = $"{title} {description}".ToLowerInvariant();
        return PublicExploitKeywords.Any(text.Contains);
    }

    //ve se o finding está exposto à internet, procura palavras em todos os campos de texto
    public static bool DetectInternetExposed(string? title, string? description, string? host, string? port)
    {
        var text = $"{title} {description} {host} {port}".ToLowerInvariant();
        return InternetExposedKeywords.Any(text.Contains);
    }

    // converte o score CVSS de texto para decimal
    // InvariantCulture para que "8.8" seja lido igual, qulquer que seja do idioma do servidor
    public static decimal? ParseCvss(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;
        return decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
            System.Globalization.CultureInfo.InvariantCulture, out var result) ? result : null;
    }
}
