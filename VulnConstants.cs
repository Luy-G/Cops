public static class VulnerabilityWeights
{
    public const decimal CriticalVulns = 0.25m;  // proporção de vulnerabilidades críticas
    public const decimal HighVulns = 0.15m;       // proporção de vulnerabilidades altas
    public const decimal PublicExploit = 0.15m;   // existência de exploit público
    public const decimal InternetExposed = 0.10m; // proporção de findings expostos à internet
    public const decimal TotalActiveWeight = CriticalVulns + HighVulns + PublicExploit + InternetExposed;
}

// High = muitos findings desta severidade = alto risco. None = zero findings = sem risco
public static class VulnRatioScoreSteps
{
    public const decimal High = 1.0m;   // acima do threshold alto
    public const decimal Medium = 0.5m; // entre o threshold médio e alto
    public const decimal Low = 0.25m;   // abaixo do threshold médio mas acima de zero
    public const decimal None = 0.0m;   // sem findings
}

// Scores possíveis para o indicador de exposição à internet.
// Usa uma escala ligeiramente diferente porque o impacto da exposição à internet é gradual.
public static class InternetExposedScoreSteps
{
    public const decimal High = 1.0m;  // maioria dos findings está exposta à internet
    public const decimal Medium = 0.5m;
    public const decimal Low = 0.3m;   // poucos findings expostos mas já é sinal de atenção
    public const decimal None = 0.0m;  // nenhum finding exposto à internet
}
