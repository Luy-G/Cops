public static class VulnCalculations
{
    public static int CountMeaningful(IEnumerable<VulnerabilityFinding> findings)
        => findings.Count(f => f.Severity != VulnSeverity.Info && f.Severity != VulnSeverity.Unknown);

    // conta findings com CVSS > 9
    public static int CountCriticalByCvss(IEnumerable<VulnerabilityFinding> findings)
        => findings.Count(f => f.Cvss.HasValue && f.Cvss.Value > CvssRange.CriticalMin);

    //CVSS / total
    public static decimal CalculateCriticalVulnsScore(IEnumerable<VulnerabilityFinding> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return 0m;

        var ratio = (decimal)CountCriticalByCvss(list) / total;

        var step = ratio == 0m               ? VulnRatioScoreSteps.None
                 : ratio > CriticalVulnsThresholds.High   ? VulnRatioScoreSteps.High
                 : ratio >= CriticalVulnsThresholds.Medium ? VulnRatioScoreSteps.Medium
                 : VulnRatioScoreSteps.Low;

        return step * VulnerabilityWeights.CriticalVulns;
    }

    // conta findings HIGH CVSS 7.0–8.9,  Severity == High se CVSS null
    public static int CountHigh(IEnumerable<VulnerabilityFinding> findings)
        => findings.Count(f =>
            f.Cvss.HasValue
                ? f.Cvss.Value >= CvssRange.HighMin && f.Cvss.Value <= CvssRange.HighMax
                : f.Severity == VulnSeverity.High);

    public static decimal CalculateHighVulnsScore(IEnumerable<VulnerabilityFinding> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return 0m;

        var ratio = (decimal)CountHigh(list) / total;

        var step = ratio == 0m             ? VulnRatioScoreSteps.None
                 : ratio > HighVulnsThresholds.High   ? VulnRatioScoreSteps.High
                 : ratio >= HighVulnsThresholds.Medium ? VulnRatioScoreSteps.Medium
                 : VulnRatioScoreSteps.Low;

        return step * VulnerabilityWeights.HighVulns;
    }

    //qualquer exploit público tem peso máximo
    public static decimal CalculatePublicExploitScore(IEnumerable<VulnerabilityFinding> findings)
        => findings.Any(f => f.HasPublicExploit) ? VulnerabilityWeights.PublicExploit : 0m;

    //qualquer finding no KEV catalog tem peso máximo
    public static decimal CalculateKevScore(IEnumerable<VulnerabilityFinding> findings)
        => findings.Any(f => f.IsInKevCatalog) ? VulnerabilityWeights.KevCatalog : 0m;

    public static decimal CalculateInternetExposedScore(IEnumerable<VulnerabilityFinding> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return 0m;

        var ratio = (decimal)list.Count(f => f.IsInternetExposed) / total;

        var step = ratio == 0m                                ? InternetExposedScoreSteps.None
                 : ratio > InternetExposedThresholds.High   ? InternetExposedScoreSteps.High    // > 0.66
                 : ratio >= InternetExposedThresholds.Medium ? InternetExposedScoreSteps.Medium  // 0.33–0.66
                 : InternetExposedScoreSteps.Low;                                                // > 0

        return step * VulnerabilityWeights.InternetExposed;
    }

    //dados de datas não disponíveis no JSON actual
    public static decimal CalculateMeanTimeToPatchScore(IEnumerable<VulnerabilityFinding> _)
        => 0m; // DATA_UNAVAILABLE

    // findings com CVE / total findings
    public static decimal CalculateScanCoverageScore(IEnumerable<VulnerabilityFinding> findings,
        decimal highThreshold = ScanCoverageThresholds.High,
        decimal mediumThreshold = ScanCoverageThresholds.Medium)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return 0m;

        var ratio = (decimal)list.Count(f =>
            f.Severity != VulnSeverity.Info && f.Severity != VulnSeverity.Unknown
            && !string.IsNullOrWhiteSpace(f.Cve)) / total;

        var step = ratio >= highThreshold   ? VulnRatioScoreSteps.None    // boa cobertura == risco baixo
                 : ratio >= mediumThreshold ? VulnRatioScoreSteps.Low      // cobertura média
                 : ratio > 0m              ? VulnRatioScoreSteps.Medium   // cobertura fraca
                 : VulnRatioScoreSteps.High;                               // sem CVEs identificados

        return step * VulnerabilityWeights.ScanCoverage;
    }

    public static decimal CalculateVulnerabilityDomainTotal(IEnumerable<VulnerabilityFinding> findings)
    {
        var list = findings.ToList();

        var weighted = CalculateCriticalVulnsScore(list)+ CalculateHighVulnsScore(list)+ CalculatePublicExploitScore(list)+ CalculateKevScore(list)+ CalculateScanCoverageScore(list);

        return weighted / VulnerabilityWeights.TotalActiveWeight;
    }
}
