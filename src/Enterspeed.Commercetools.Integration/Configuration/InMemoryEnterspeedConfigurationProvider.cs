using Enterspeed.Source.Sdk.Api.Providers;
using Enterspeed.Source.Sdk.Configuration;

namespace Enterspeed.Commercetools.Integration.Configuration;

public class InMemoryEnterspeedConfigurationProvider: IEnterspeedConfigurationProvider
{
    public InMemoryEnterspeedConfigurationProvider(EnterspeedConfiguration configuration)
    {
        Configuration = configuration;
    }

    public EnterspeedConfiguration Configuration { get; }
}