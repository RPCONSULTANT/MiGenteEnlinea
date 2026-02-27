namespace MiGenteEnLinea.Infrastructure.Persistence.Seeding;

public sealed class SeedBlockResult
{
    public string BlockName { get; init; } = string.Empty;
    public int Inserted { get; init; }
    public int Updated { get; init; }
    public int Skipped { get; init; }
    public string? Error { get; init; }
}

public sealed class SeedExecutionReport
{
    public DateTime StartedAtUtc { get; init; }
    public DateTime CompletedAtUtc { get; init; }
    public bool Success { get; init; }
    public bool Locked { get; init; }
    public List<SeedBlockResult> Blocks { get; init; } = [];
    public List<string> Errors { get; init; } = [];
    public double DurationMs => (CompletedAtUtc - StartedAtUtc).TotalMilliseconds;
}
