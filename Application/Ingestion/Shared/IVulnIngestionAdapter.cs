using CyberOps.Domain.Entities;

namespace CyberOps.Application.Ingestion.Shared;

public interface IVulnIngestionAdapter
{
    bool CanHandle(long clientId);
    IngestionResult<VulnerabilityAttackSurface> Ingest(string json, long clientId);
}
