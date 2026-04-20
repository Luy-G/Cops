public class SogilubVulnMetadataDto
{
    [JsonPropertyName("CVE")]
    public string? Cve { get; set; }

    [JsonPropertyName("CVSS")]
    public string? Cvss { get; set; }

    public string? Host { get; set; }

    public string? Port { get; set; }

    public string? Status { get; set; }
}

// finding individual, uma vulnerabilidade encontrada no scan
public class SogilubVulnFindingDto
{
    public string? Id { get; set; }

    public string? Severity { get; set; }

    public string? Title { get; set; }

    // metadados: CVE, CVSS, host, porto, evidência
    public SogilubVulnMetadataDto? Metadata { get; set; }

    public string? Description { get; set; }

    public string? Impact { get; set; }

    // recomendação de remediação
    public string? Recommendation { get; set; }
}

public class SogilubVulnReportDto
{
    public List<SogilubVulnFindingDto>? vulnerability_findings { get; set; }
}
