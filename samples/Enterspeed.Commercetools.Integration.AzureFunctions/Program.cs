using System.Threading.Tasks;
using commercetools.Sdk.Api;
using Enterspeed.Commercetools.Integration.Configuration;
using Enterspeed.Source.Sdk.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Enterspeed.Commercetools.Integration.AzureFunctions;

public class Program
{
    public static async Task Main(string[] args)
    {
        using var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        new HostBuilder()
            .ConfigureFunctionsWorkerDefaults()
            .ConfigureLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddConsole();
            })
            .ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddServiceBus();
            })
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("local.settings.json", true);
            })
            .ConfigureServices((hostingContext, services) =>
            {
                services.UseCommercetoolsApi(hostingContext.Configuration);
                services.AddEnterspeedCommercetoolsIntegration(config =>
                {
                    config.SetEnterspeedConfiguration(new EnterspeedConfiguration
                    {
                        ApiKey = hostingContext.Configuration.GetValue<string>("Enterspeed:ApiKey"),
                        BaseUrl = hostingContext.Configuration.GetValue<string>("Enterspeed:BaseUrl")
                    });

                    config.SetCommercetoolsProjectKey(hostingContext.Configuration.GetValue<string>("CommercetoolsApi:ProjectKey"));
                });
            });
}
