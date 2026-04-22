namespace ControloQualidade.Common.Ingestion;

public class SogilubVulnIngestion
{
    public VulnerabilityFinding Map(SogilubVulnFindingDto dto, long clientId, IReadOnlySet<string>? kevCveIds = null)
    {
        var cve = dto.Metadata?.CVE?.Trim().ToUpperInvariant();
        var isInKev = cve != null && kevCveIds != null && kevCveIds.Contains(cve);

        return new VulnerabilityFinding
        {
            ClientId = clientId,
            FindingKey = dto.Id!,
            SourceSystem = SourceSystem.Other,
            ScanEngine = ScanEngine.Unknown,
            Severity = SogilubVulnMapper.MapSeverity(dto.Severity),
            Title = dto.Title!,
            // campos do bloco metadata do JSON, nullable
            Cve = dto.Metadata?.CVE,
            Cvss = SogilubVulnMapper.ParseCvss(dto.Metadata?.CVSS),
            Host = dto.Metadata?.Host,
            Port = dto.Metadata?.Port,
            Evidence = dto.Metadata?.Status,
            Description = dto.Description,
            Impact = dto.Impact,
            Recommendation = dto.Recommendation,
            HasPublicExploit = false,
            IsInternetExposed = false,
            IsInKevCatalog = isInKev
        };
    }

    // findings INFO são excluídos dos cálculos de score
    public IEnumerable<VulnerabilityFinding> MapAll(SogilubVulnReportDto report, long clientId, IReadOnlySet<string>? kevCveIds = null)
    {
        // se vier nula no JSON, trata como lista vazia para não crashar
        return (report.vulnerability_findings ?? []).Select(f => Map(f, clientId, kevCveIds));
    }
}
