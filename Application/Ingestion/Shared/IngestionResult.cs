namespace CyberOps.Application.Ingestion.Shared;

public class IngestionResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public IReadOnlyList<string> Errors { get; init; } = [];
    public bool Success => Errors.Count == 0;
}
