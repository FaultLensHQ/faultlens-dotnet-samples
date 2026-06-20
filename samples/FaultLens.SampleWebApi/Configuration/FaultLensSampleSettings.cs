namespace FaultLens.SampleWebApi.Configuration;

public sealed class FaultLensSampleSettings
{
    public const string SectionName = "FaultLens";

    public string ApiKey { get; set; } = string.Empty;

    public string Endpoint { get; set; } = string.Empty;

    public string Environment { get; set; } = "staging";

    public string Release { get; set; } = "faultlens-dotnet-sample-1";

    public string ServiceName { get; set; } = "checkout-api";

    public string ServiceVersion { get; set; } = "v1.8.4";

    public string TenantId { get; set; } = "tenant_demo_retail";

    public string AccountId { get; set; } = "acct_demo_standard";

    public string UserId { get; set; } = "user_demo_123";

    public string AnonymousId { get; set; } = "anon_demo_browser_456";

    public int BreadcrumbCapacity { get; set; } = 50;
}
