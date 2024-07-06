using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureAppConfiguration((context, config) =>
    {
    })
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        // Add Application Insights telemetry services
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
    })
    .Build();

host.Run();

