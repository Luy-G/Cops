using CyberOps.Domain.Entities;

namespace CyberOps.Application.Ingestion.Shared;

public interface IItsmIngestionAdapter
{
    bool CanHandle(long clientId);
    IngestionResult<Operationalsecitsm> Ingest(string json, long clientId);
}
