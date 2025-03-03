namespace erd_dotnet;

public record Relationship
{
    public string Name1 { get; init; } = string.Empty;
    public string Label1 { get; init; } = string.Empty;
    public string Name2 { get; init; } = string.Empty;
    public string Label2 { get; init; } = string.Empty;
}
