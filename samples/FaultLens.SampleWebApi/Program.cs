using FaultLens.Sdk;
using FaultLens.SampleWebApi.Configuration;
using FaultLens.SampleWebApi.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<FaultLensSampleSettings>(builder.Configuration.GetSection(FaultLensSampleSettings.SectionName));
builder.Services.AddSingleton<FaultLensClient>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var environment = sp.GetRequiredService<IHostEnvironment>();
    var settings = configuration.GetSection(FaultLensSampleSettings.SectionName).Get<FaultLensSampleSettings>() ?? new FaultLensSampleSettings();

    var apiKey = settings.ApiKey?.Trim();
    if (string.IsNullOrWhiteSpace(apiKey))
    {
        throw new InvalidOperationException(
            "FaultLens configuration is missing. Set FaultLens:ApiKey or FaultLens__ApiKey before starting the sample.");
    }

    var release = string.IsNullOrWhiteSpace(settings.Release)
        ? typeof(Program).Assembly.GetName().Version?.ToString() ?? "1.0.0"
        : settings.Release.Trim();
    var endpoint = string.IsNullOrWhiteSpace(settings.Endpoint)
        ? new Uri("https://TENANT-SLUG.staging.faultlens.in")
        : new Uri(settings.Endpoint.Trim());
    var environmentName = string.IsNullOrWhiteSpace(settings.Environment)
        ? environment.EnvironmentName
        : settings.Environment.Trim();

    return new FaultLensClient(new FaultLensOptions(
        apiKey: apiKey,
        environment: environmentName,
        release: release,
        endpoint: endpoint,
        breadcrumbCapacity: settings.BreadcrumbCapacity));
});
builder.Services.AddSingleton<IFaultLensClient>(sp => sp.GetRequiredService<FaultLensClient>());
builder.Services.AddHttpClient("demo-upstream", client =>
{
    client.Timeout = TimeSpan.FromSeconds(2);
});

builder.Services.AddControllers();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<FaultLensExceptionHandler>();
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();
