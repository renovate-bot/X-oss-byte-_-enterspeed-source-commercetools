using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedDateTimeMapper : IMapper<DateTime, IEnterspeedProperty>
{
    public Task<IEnterspeedProperty> MapAsync(DateTime source)
    {
        return Task.FromResult<IEnterspeedProperty>(new StringEnterspeedProperty(source.ToString("O")));
    }
}