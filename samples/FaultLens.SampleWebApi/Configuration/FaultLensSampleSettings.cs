namespace FaultLens.SampleWebApi.Configuration;

public sealed class FaultLensSampleSettings
{
    public const string SectionName = "FaultLens";

    public string ApiKey { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string Environment { get; set; } = "staging";

    public string Release { get; set; } = "faultlens-dotnet-sample-1";

    public int BreadcrumbCapacity { get; set; } = 50;
}
