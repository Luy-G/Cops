using CyberOps.Application.Scoring;
using CyberOps.Common.Constants;
using CyberOps.Domain.Entities;
using CyberOps.Domain.Enums;
using Xunit;

namespace CyberOps.Tests;

public class VulnCalculationsTests
{
    private static VulnerabilityAttackSurface Finding(
        VulnSeverity severity,
        decimal? cvss = null,
        bool hasExploit = false,
        bool isInternetExposed = false,
        bool isInKev = false)
        => new()
        {
            FindingKey = "F-test",
            Title = "Test",
            ClientId = 1,
            SourceSystem = SourceSystem.Other,
            ScanEngine = ScanEngine.Unknown,
            Severity = severity,
            Cvss = cvss,
            HasPublicExploit = hasExploit,
            IsInternetExposed = isInternetExposed,
            IsInKevCatalog = isInKev
        };

    [Fact]
    public void CountCriticalByCvss_ShouldCount_WhenCvssAbove9()
    {
        var findings = new[] { Finding(VulnSeverity.Critical, cvss: 9.5m) };
        Assert.Equal(1, VulnCalculations.CountCriticalByCvss(findings));
    }

    [Fact]
    public void CountCriticalByCvss_ShouldNotCount_WhenCvssIs8_8()
    {
        var findings = new[] { Finding(VulnSeverity.High, cvss: 8.8m) };
        Assert.Equal(0, VulnCalculations.CountCriticalByCvss(findings));
    }

    [Fact]
    public void CountCriticalByCvss_ShouldNotCount_WhenCvssIsNull()
    {
        var findings = new[] { Finding(VulnSeverity.Critical, cvss: null) };
        Assert.Equal(0, VulnCalculations.CountCriticalByCvss(findings));
    }

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturn0_WhenNoCriticals()
    {
        var findings = new[] { Finding(VulnSeverity.Medium, cvss: 5.0m) };
        Assert.Equal(0m, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturnFullWeight_WhenRatioAbove5Percent()
    {
        var findings = new List<VulnerabilityAttackSurface> { Finding(VulnSeverity.Critical, cvss: 9.5m) };
        for (var i = 0; i < 9; i++)
        {
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));
        }

        Assert.Equal(VulnerabilityWeights.CriticalVulns, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturnHalfWeight_WhenRatioBetween3And5Percent()
    {
        var findings = new List<VulnerabilityAttackSurface> { Finding(VulnSeverity.Critical, cvss: 9.5m) };
        for (var i = 0; i < 29; i++)
        {
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));
        }

        var expected = VulnerabilityWeights.CriticalVulns * VulnRatioScoreSteps.Medium;
        Assert.Equal(expected, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    [Fact]
    public void CountHigh_ShouldCount_WhenCvssIs7_5()
    {
        var findings = new[] { Finding(VulnSeverity.High, cvss: 7.5m) };
        Assert.Equal(1, VulnCalculations.CountHigh(findings));
    }

    [Fact]
    public void CountHigh_ShouldNotCount_WhenCvssIs9_5()
    {
        var findings = new[] { Finding(VulnSeverity.Critical, cvss: 9.5m) };
        Assert.Equal(0, VulnCalculations.CountHigh(findings));
    }

    [Fact]
    public void CountHigh_ShouldCountAsFallback_WhenSeverityHighAndCvssNull()
    {
        var findings = new[] { Finding(VulnSeverity.High, cvss: null) };
        Assert.Equal(1, VulnCalculations.CountHigh(findings));
    }

    [Fact]
    public void CalculateHighVulnsScore_ShouldReturnFullWeight_WhenRatioAbove10Percent()
    {
        var findings = new List<VulnerabilityAttackSurface>
        {
            Finding(VulnSeverity.High, cvss: 7.5m),
            Finding(VulnSeverity.High, cvss: 8.0m)
        };

        for (var i = 0; i < 8; i++)
        {
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));
        }

        Assert.Equal(VulnerabilityWeights.HighVulns, VulnCalculations.CalculateHighVulnsScore(findings));
    }

    [Fact]
    public void CalculateHighVulnsScore_ShouldReturnHalfWeight_WhenRatioBetween5And10Percent()
    {
        var findings = new List<VulnerabilityAttackSurface> { Finding(VulnSeverity.High, cvss: 7.5m) };
        for (var i = 0; i < 14; i++)
        {
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));
        }

        var expected = VulnerabilityWeights.HighVulns * VulnRatioScoreSteps.Medium;
        Assert.Equal(expected, VulnCalculations.CalculateHighVulnsScore(findings));
    }

    [Fact]
    public void CalculatePublicExploitScore_ShouldReturnFullWeight_WhenOneFindingHasExploit()
    {
        var findings = new[] { Finding(VulnSeverity.High, hasExploit: true) };
        Assert.Equal(VulnerabilityWeights.PublicExploit, VulnCalculations.CalculatePublicExploitScore(findings));
    }

    [Fact]
    public void CalculatePublicExploitScore_ShouldReturn0_WhenNoExploit()
    {
        var findings = new[] { Finding(VulnSeverity.High, hasExploit: false) };
        Assert.Equal(0m, VulnCalculations.CalculatePublicExploitScore(findings));
    }

    private static VulnerabilityAttackSurface FindingWithCve(VulnSeverity severity, string? cve)
        => new()
        {
            FindingKey = "F-test",
            Title = "Test",
            ClientId = 1,
            SourceSystem = SourceSystem.Other,
            ScanEngine = ScanEngine.Unknown,
            Severity = severity,
            Cve = cve
        };

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturn0_WhenAllFindingsHaveCve()
    {
        var findings = Enumerable.Range(0, 10)
            .Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"));

        Assert.Equal(0m, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnLowStep_WhenCoverageBetween50And80Percent()
    {
        var findings = Enumerable.Range(0, 6).Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"))
            .Concat(Enumerable.Range(0, 4).Select(_ => FindingWithCve(VulnSeverity.Medium, null)))
            .ToList();

        var expected = VulnRatioScoreSteps.Low * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnMediumStep_WhenCoverageBelow50Percent()
    {
        var findings = Enumerable.Range(0, 4).Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"))
            .Concat(Enumerable.Range(0, 6).Select(_ => FindingWithCve(VulnSeverity.Medium, null)))
            .ToList();

        var expected = VulnRatioScoreSteps.Medium * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnFullWeight_WhenNoCvesIdentified()
    {
        var findings = Enumerable.Range(0, 5).Select(_ => FindingWithCve(VulnSeverity.High, null));
        var expected = VulnRatioScoreSteps.High * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }
}
