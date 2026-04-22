using Xunit;

public class VulnCalculationsTests
{
    // helpers
    private static VulnerabilityAttackSurface Finding(VulnSeverity severity, decimal? cvss = null,
        bool hasExploit = false, bool isInternetExposed = false, bool isInKev = false)
        => new VulnerabilityAttackSurface
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

    // --- CountCriticalByCvss ---

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

    // --- CalculateCriticalVulnsScore ---

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturn0_WhenNoCriticals()
    {
        var findings = new[] { Finding(VulnSeverity.Medium, cvss: 5.0m) };
        Assert.Equal(0m, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturnFullWeight_WhenRatioAbove5Percent()
    {
        // 1 crítico CVSS 9.5 em 10 findings = ratio 0.10 → > 0.05 → score máximo = 0.25
        var findings = new List<VulnerabilityAttackSurface>
        {
            Finding(VulnSeverity.Critical, cvss: 9.5m)
        };
        for (var i = 0; i < 9; i++)
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));

        Assert.Equal(VulnerabilityWeights.CriticalVulns, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    [Fact]
    public void CalculateCriticalVulnsScore_ShouldReturnHalfWeight_WhenRatioBetween3And5Percent()
    {
        // 1 crítico CVSS 9.5 em 30 findings = ratio ~0.033 → >= 0.03 && <= 0.05 → 0.125
        var findings = new List<VulnerabilityAttackSurface>
        {
            Finding(VulnSeverity.Critical, cvss: 9.5m)
        };
        for (var i = 0; i < 29; i++)
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));

        var expected = VulnerabilityWeights.CriticalVulns * VulnRatioScoreSteps.Medium;
        Assert.Equal(expected, VulnCalculations.CalculateCriticalVulnsScore(findings));
    }

    // --- CountHigh ---

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

    // --- CalculateHighVulnsScore ---

    [Fact]
    public void CalculateHighVulnsScore_ShouldReturnFullWeight_WhenRatioAbove10Percent()
    {
        // 2 highs CVSS 7.5 em 10 findings = ratio 0.20 → > 0.10 → score máximo = 0.15
        var findings = new List<VulnerabilityAttackSurface>
        {
            Finding(VulnSeverity.High, cvss: 7.5m),
            Finding(VulnSeverity.High, cvss: 8.0m)
        };
        for (var i = 0; i < 8; i++)
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));

        Assert.Equal(VulnerabilityWeights.HighVulns, VulnCalculations.CalculateHighVulnsScore(findings));
    }

    [Fact]
    public void CalculateHighVulnsScore_ShouldReturnHalfWeight_WhenRatioBetween5And10Percent()
    {
        // 1 high CVSS 7.5 em 15 findings = ratio ~0.067 → >= 0.05 && <= 0.10 → 0.075
        var findings = new List<VulnerabilityAttackSurface>
        {
            Finding(VulnSeverity.High, cvss: 7.5m)
        };
        for (var i = 0; i < 14; i++)
            findings.Add(Finding(VulnSeverity.Medium, cvss: 5.0m));

        var expected = VulnerabilityWeights.HighVulns * VulnRatioScoreSteps.Medium;
        Assert.Equal(expected, VulnCalculations.CalculateHighVulnsScore(findings));
    }

    // --- CalculatePublicExploitScore ---

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

    // --- CalculateScanCoverageScore ---

    private static VulnerabilityAttackSurface FindingWithCve(VulnSeverity severity, string? cve)
        => new VulnerabilityAttackSurface
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
        // >= 80% → cobertura boa → risco zero
        var findings = Enumerable.Range(0, 10)
            .Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"));
        Assert.Equal(0m, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnLowStep_WhenCoverageBetween50And80Percent()
    {
        // 6 com CVE + 4 sem = 60% → medium threshold (0.50) mas < high (0.80) → step Low
        var findings = Enumerable.Range(0, 6).Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"))
            .Concat(Enumerable.Range(0, 4).Select(_ => FindingWithCve(VulnSeverity.Medium, null)))
            .ToList();
        var expected = VulnRatioScoreSteps.Low * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnMediumStep_WhenCoverageBelow50Percent()
    {
        // 4 com CVE + 6 sem = 40% → < 50% → step Medium
        var findings = Enumerable.Range(0, 4).Select(_ => FindingWithCve(VulnSeverity.High, "CVE-2021-1234"))
            .Concat(Enumerable.Range(0, 6).Select(_ => FindingWithCve(VulnSeverity.Medium, null)))
            .ToList();
        var expected = VulnRatioScoreSteps.Medium * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }

    [Fact]
    public void CalculateScanCoverageScore_ShouldReturnFullWeight_WhenNoCvesIdentified()
    {
        // nenhum CVE identificado → risco máximo
        var findings = Enumerable.Range(0, 5).Select(_ => FindingWithCve(VulnSeverity.High, null));
        var expected = VulnRatioScoreSteps.High * VulnerabilityWeights.ScanCoverage;
        Assert.Equal(expected, VulnCalculations.CalculateScanCoverageScore(findings));
    }
}
