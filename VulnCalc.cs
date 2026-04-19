public static class VulnCalculations
{
    // conta os findings, exclui INFO e Unknown
    // total é usado em todos os cálculos de rácio.
    public static int CountMeaningful(IEnumerable<VulnerabilityFinding> findings)
    {
        return findings.Count(f => f.Severity != VulnSeverity.Info && f.Severity != VulnSeverity.Unknown);
    }

    // conta os findings de uma severidade específica
    public static int CountBySeverity(IEnumerable<VulnerabilityFinding> findings, VulnSeverity severity)
    {
        return findings.Count(f => f.Severity == severity);
    }

    // calcula o score do indicador de vulnerabilidades críticas.
    // exemplo: 2 críticos em 10 findings = rácio 0.20 → acima do threshold de 0.05 → score máximo (1.0)
    public static decimal CalculateCriticalVulnScore(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return VulnRatioScoreSteps.None;

        var ratio = (decimal)CountBySeverity(list, VulnSeverity.Critical) / total;

        if (ratio == 0m)                              return VulnRatioScoreSteps.None;
        if (ratio > calcs.CriticalRatioHighThreshold) return VulnRatioScoreSteps.High;
        if (ratio >= calcs.CriticalRatioMediumThreshold) return VulnRatioScoreSteps.Medium;
        return VulnRatioScoreSteps.Low;
    }

    // calcula o score de vulnerabilidades altas(mesma lógica que as críticas)
    public static decimal CalculateHighVulnScore(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return VulnRatioScoreSteps.None;

        var ratio = (decimal)CountBySeverity(list, VulnSeverity.High) / total;

        if (ratio == 0m)                          return VulnRatioScoreSteps.None;
        if (ratio > calcs.HighRatioHighThreshold) return VulnRatioScoreSteps.High;
        if (ratio >= calcs.HighRatioMediumThreshold) return VulnRatioScoreSteps.Medium;
        return VulnRatioScoreSteps.Low;
    }

    // ve se existe pelo menos um finding com exploit público conhecido
    // é binário, qualquer exploit público é suficiente para o score máximo
    public static decimal CalculatePublicExploitScore(IEnumerable<VulnerabilityFinding> findings)
    {
        return findings.Any(f => f.HasPublicExploit) ? 1.0m : 0.0m;
    }

    // calcula o score de exposição à internet, proporção de findings em serviços expostos externamente
    public static decimal CalculateInternetExposedScore(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
    {
        var list = findings.ToList();
        var total = CountMeaningful(list);

        if (total == 0) return InternetExposedScoreSteps.None;

        var ratio = (decimal)list.Count(f => f.IsInternetExposed) / total;

        if (ratio == 0m)                               return InternetExposedScoreSteps.None;
        if (ratio > calcs.ExposedRatioHighThreshold)   return InternetExposedScoreSteps.High;
        if (ratio >= calcs.ExposedRatioMediumThreshold) return InternetExposedScoreSteps.Medium;
        return InternetExposedScoreSteps.Low;
    }

    // funções Weighted multiplicam o score pelo peso do indicador no domínio
    public static decimal CalculateCriticalVulnWeighted(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
        => CalculateCriticalVulnScore(findings, calcs) * VulnerabilityWeights.CriticalVulns;

    public static decimal CalculateHighVulnWeighted(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
        => CalculateHighVulnScore(findings, calcs) * VulnerabilityWeights.HighVulns;

    public static decimal CalculatePublicExploitWeighted(IEnumerable<VulnerabilityFinding> findings)
        => CalculatePublicExploitScore(findings) * VulnerabilityWeights.PublicExploit;

    public static decimal CalculateInternetExposedWeighted(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
        => CalculateInternetExposedScore(findings, calcs) * VulnerabilityWeights.InternetExposed;

    // calcula o score total do domínio
    // divide pelo peso total ativo para normalizar o resultado entre 0 e 1.
    public static decimal CalculateVulnerabilityDomainTotal(IEnumerable<VulnerabilityFinding> findings, ClientVulnCalcs calcs)
    {
        var list = findings.ToList();

        var weighted = CalculateCriticalVulnWeighted(list, calcs)
            + CalculateHighVulnWeighted(list, calcs)
            + CalculatePublicExploitWeighted(list)
            + CalculateInternetExposedWeighted(list, calcs);

        return weighted / VulnerabilityWeights.TotalActiveWeight;
    }
}
