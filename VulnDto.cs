public class SogilubVulnMetadataDto
{
    // codigo CVE da vulnerabilidade, pode ser nulo se não houver CVE associado
    [JsonPropertyName("CVE")]
    public string? Cve { get; set; }

    // score de severidade CVSS, vem como texto no JSON
    [JsonPropertyName("CVSS")]
    public string? Cvss { get; set; }

    // hosts afetados, pode ter múltiplos hosts num só campo
    [JsonPropertyName("Host")]
    public string? Host { get; set; }

    // porto afetados
    [JsonPropertyName("Port")]
    public string? Port { get; set; }

    //nota sobre como o finding foi confirmado
    [JsonPropertyName("Status")]
    public string? Status { get; set; }
}

// finding individual, uma vulnerabilidade encontrada no scan
public class SogilubVulnFindingDto
{
    // identificador do finding no relatório
    [JsonPropertyName("id")]
    public string? Id { get; set; }

    // severidade em texto
    [JsonPropertyName("severity")]
    public string? Severity { get; set; }

    [JsonPropertyName("title")]
    public string? Title { get; set; }

    // etadados: CVE, CVSS, host, porto, evidência
    [JsonPropertyName("metadata")]
    public SogilubVulnMetadataDto? Metadata { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    // impacto se a vulnerabilidade for explorada
    [JsonPropertyName("impact")]
    public string? Impact { get; set; }

    // recomendação de remediação
    [JsonPropertyName("recommendation")]
    public string? Recommendation { get; set; }
}

public class SogilubVulnReportDto
{
    [JsonPropertyName("vulnerability_findings")]
    public List<SogilubVulnFindingDto>? Findings { get; set; }
}
