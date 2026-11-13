namespace Servora.Ai.Options;

public sealed class ServoraApiOptions
{
    public const string SectionName = "ServoraApi";

    public string BaseUrl { get; set; } = "http://localhost:5137";
}
