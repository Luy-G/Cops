namespace ControloQualidade.Common.Ingestion;

public class SogilubVulnIngestion
{
    public VulnerabilityFinding Map(SogilubVulnFindingDto dto, long clientId)
    {
        return new VulnerabilityFinding
        {
            ClientId = clientId,

            FindingKey = dto.Id!,

            SourceSystem = SourceSystem.Other,

            //confirmar ******************************************************
            ScanEngine = ScanEngine.Unknown,

            // converte a severidade para o enum VulnSeverity
            Severity = SogilubVulnMapper.MapSeverity(dto.Severity),

            Title = dto.Title!,

            // campos do bloco metadata do JSON nullable
            Cve = dto.Metadata?.Cve,
            Cvss = SogilubVulnMapper.ParseCvss(dto.Metadata?.Cvss),
            Host = dto.Metadata?.Host,
            Port = dto.Metadata?.Port,

            // nota de evidência do scanner
            Evidence = dto.Metadata?.Status,

            // texto descritivo do finding
            Description = dto.Description,
            Impact = dto.Impact,
            Recommendation = dto.Recommendation,

            // deteta se o finding tem exploit público conhecido, com base em palavras no título e descrição
            HasPublicExploit = SogilubVulnMapper.DetectPublicExploit(dto.Title, dto.Description),

            // deteta se o finding está exposto à internet, com base em palavras
            IsInternetExposed = SogilubVulnMapper.DetectInternetExposed(
                dto.Title, dto.Description, dto.Metadata?.Host, dto.Metadata?.Port)
        };
    }

    // transforma o relatório completo (que contém a lista de findings) numa coleção de entidades.
    public IEnumerable<VulnerabilityFinding> MapAll(SogilubVulnReportDto report, long clientId)
    {
        // se vier nula no JSON, trata como lista vazia para não crashar
        return (report.Findings ?? []).Select(f => Map(f, clientId));
    }
}
