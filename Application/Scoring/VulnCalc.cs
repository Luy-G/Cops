using CyberOps.Common.Constants;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;

namespace CyberOps.Application.Scoring;

public static class VulnCalculations
{
    public static int CountMeaningful(IEnumerable<VulnerabilityAttackSurface> findings)
        => findings.Count(f => f.Severity != VulnSeverity.Info && f.Severity != VulnSeverity.Unknown);

    public static int CountCriticalByCvss(IEnumerable<VulnerabilityAttackSurface> findings)
        => findings.Count(f => f.Cvss.HasValue && f.Cvss.Value > CvssRange.CriticalMin);

    public static decimal CalculateCriticalVulnsScore(IEnumerable<VulnerabilityAttackSurface> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);
        if (total == 0)
        {
            return 0m;
        }

        var ratio = (decimal)CountCriticalByCvss(list) / total;
        var step = ratio == 0m ? VulnRatioScoreSteps.None
                 : ratio > CriticalVulnsThresholds.High ? VulnRatioScoreSteps.High
                 : ratio >= CriticalVulnsThresholds.Medium ? VulnRatioScoreSteps.Medium
                 : VulnRatioScoreSteps.Low;

        return step * VulnerabilityWeights.CriticalVulns;
    }

    public static int CountHigh(IEnumerable<VulnerabilityAttackSurface> findings)
        => findings.Count(f =>
            f.Cvss.HasValue
                ? f.Cvss.Value >= CvssRange.HighMin && f.Cvss.Value <= CvssRange.HighMax
                : f.Severity == VulnSeverity.High);

    public static decimal CalculateHighVulnsScore(IEnumerable<VulnerabilityAttackSurface> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);
        if (total == 0)
        {
            return 0m;
        }

        var ratio = (decimal)CountHigh(list) / total;
        var step = ratio == 0m ? VulnRatioScoreSteps.None
                 : ratio > HighVulnsThresholds.High ? VulnRatioScoreSteps.High
                 : ratio >= HighVulnsThresholds.Medium ? VulnRatioScoreSteps.Medium
                 : VulnRatioScoreSteps.Low;

        return step * VulnerabilityWeights.HighVulns;
    }

    public static decimal CalculatePublicExploitScore(IEnumerable<VulnerabilityAttackSurface> findings)
        => findings.Any(f => f.HasPublicExploit) ? VulnerabilityWeights.PublicExploit : 0m;

    public static decimal CalculateKevScore(IEnumerable<VulnerabilityAttackSurface> findings)
        => findings.Any(f => f.IsInKevCatalog) ? VulnerabilityWeights.KevCatalog : 0m;

    public static decimal CalculateInternetExposedScore(IEnumerable<VulnerabilityAttackSurface> findings)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);
        if (total == 0)
        {
            return 0m;
        }

        var ratio = (decimal)list.Count(f => f.IsInternetExposed) / total;

        var step = ratio == 0m ? InternetExposedScoreSteps.None
                 : ratio > InternetExposedThresholds.High ? InternetExposedScoreSteps.High
                 : ratio >= InternetExposedThresholds.Medium ? InternetExposedScoreSteps.Medium
                 : InternetExposedScoreSteps.Low;

        return step * VulnerabilityWeights.InternetExposed;
    }

    public static decimal CalculateMeanTimeToPatchScore(IEnumerable<VulnerabilityAttackSurface> _)
        => 0m;

    public static decimal CalculateScanCoverageScore(
        IEnumerable<VulnerabilityAttackSurface> findings,
        decimal highThreshold = ScanCoverageThresholds.High,
        decimal mediumThreshold = ScanCoverageThresholds.Medium)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);
        if (total == 0)
        {
            return 0m;
        }

        var ratio = (decimal)list.Count(f =>
            f.Severity != VulnSeverity.Info
            && f.Severity != VulnSeverity.Unknown
            && !string.IsNullOrWhiteSpace(f.Cve)) / total;

        var step = ratio >= highThreshold ? VulnRatioScoreSteps.None
                 : ratio >= mediumThreshold ? VulnRatioScoreSteps.Low
                 : ratio > 0m ? VulnRatioScoreSteps.Medium
                 : VulnRatioScoreSteps.High;

        return step * VulnerabilityWeights.ScanCoverage;
    }

    public static decimal CalculateVulnerabilityDomainTotal(IEnumerable<VulnerabilityAttackSurface> findings)
    {
        var list = findings.ToList();

        var weighted = CalculateCriticalVulnsScore(list)
            + CalculateHighVulnsScore(list)
            + CalculatePublicExploitScore(list)
            + CalculateKevScore(list)
            + CalculateScanCoverageScore(list);

        return weighted / VulnerabilityWeights.TotalActiveWeight;
    }
}
