namespace SurrealDb.Client;

public class SurrealClientOptions 
{
    public const string SurrealClient = "SurrealClient";
    public string BaseAddress { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string Namespace { get; init; } = string.Empty;
    public string Database { get; init; } = string.Empty;
}
