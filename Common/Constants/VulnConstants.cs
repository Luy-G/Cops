namespace CyberOps.Common.Constants;

public static class VulnerabilityWeights
{
    public const decimal CriticalVulns = 0.25m;
    public const decimal HighVulns = 0.15m;
    public const decimal PublicExploit = 0.15m;
    public const decimal KevCatalog = 0.15m;
    public const decimal InternetExposed = 0.10m;
    public const decimal MeanTimeToPatch = 0.05m;
    public const decimal ScanCoverage = 0.05m;
    public const decimal AssetsExposed = 0.10m;
    public const decimal TotalActiveWeight = CriticalVulns + HighVulns + PublicExploit + KevCatalog + ScanCoverage;
}

public static class CriticalVulnsThresholds
{
    public const decimal High = 0.05m;
    public const decimal Medium = 0.03m;
}

public static class HighVulnsThresholds
{
    public const decimal High = 0.10m;
    public const decimal Medium = 0.05m;
}

public static class InternetExposedThresholds
{
    public const decimal High = 0.66m;
    public const decimal Medium = 0.33m;
}

public static class CvssRange
{
    public const decimal CriticalMin = 9.0m;
    public const decimal HighMin = 7.0m;
    public const decimal HighMax = 8.9m;
}

public static class VulnRatioScoreSteps
{
    public const decimal High = 1.00m;
    public const decimal Medium = 0.50m;
    public const decimal Low = 0.25m;
    public const decimal None = 0.00m;
}

public static class ScanCoverageThresholds
{
    public const decimal High = 0.80m;
    public const decimal Medium = 0.50m;
}

public static class InternetExposedScoreSteps
{
    public const decimal High = 0.8m;
    public const decimal Medium = 0.5m;
    public const decimal Low = 0.1m;
    public const decimal None = 0.0m;
}
