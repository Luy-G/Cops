public static class VulnerabilityWeights
{
    public const decimal CriticalVulns   = 0.25m;
    public const decimal HighVulns       = 0.15m;
    public const decimal PublicExploit   = 0.15m;
    public const decimal KevCatalog      = 0.15m;
    public const decimal InternetExposed = 0.10m;
    public const decimal MeanTimeToPatch = 0.05m;
    public const decimal ScanCoverage    = 0.05m;
    public const decimal AssetsExposed   = 0.10m;

    // soma dos pesos com dados actualmente disponíveis (exclui MtTP, AssetsExposed e InternetExposed — scan local)
    public const decimal TotalActiveWeight = CriticalVulns + HighVulns + PublicExploit + KevCatalog + ScanCoverage;
}

//fixos para o calc do CVSS > 9 / total
public static class CriticalVulnsThresholds
{
    public const decimal High   = 0.05m;
    public const decimal Medium = 0.03m;
}

//fixos para o calc do CVSS 7.0–8.9 / total
public static class HighVulnsThresholds
{
    public const decimal High   = 0.10m;
    public const decimal Medium = 0.05m;
}

//fixos para findings expostos à internet
public static class InternetExposedThresholds
{
    public const decimal High   = 0.66m;  // Se > 0.66 =  0.8%
    public const decimal Medium = 0.33m;  // Se  >= 0.33 =  0.5%
    // Se > 0 && < 0.33 → step 0.1
}

// bandas CVSS usadas para classificar findings por severidade numérica
public static class CvssRange
{
    public const decimal CriticalMin = 9.0m;
    public const decimal HighMin     = 7.0m;
    public const decimal HighMax     = 8.9m;
}

// multiplicadores de step para indicadores com Critical e High
public static class VulnRatioScoreSteps
{
    public const decimal High   = 1.00m;
    public const decimal Medium = 0.50m;
    public const decimal Low    = 0.25m;
    public const decimal None   = 0.00m;
}

// findings com CVE / total findings
// baixa = maior risco (thresholds sao usados como mínimos aceitáveis)
public static class ScanCoverageThresholds
{
    public const decimal High   = 0.80m;  // >= 80% findings com CVE = risco baixo
    public const decimal Medium = 0.50m;  // >= 50% = risco médio
    // < 50% = risco máximo
}

// multiplicadores de step para o indicador de exposição à internet
public static class InternetExposedScoreSteps
{
    public const decimal High   = 0.8m;  // > 66% exposto
    public const decimal Medium = 0.5m;  // 33–66%
    public const decimal Low    = 0.1m;  // > 0% && < 33%
    public const decimal None   = 0.0m;  // 0 exposto
}
