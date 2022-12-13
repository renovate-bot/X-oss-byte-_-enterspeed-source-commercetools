using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedAvailabilityMapper : IMapper<IProductVariantAvailability, IEnterspeedProperty>
{
    public Task<IEnterspeedProperty> MapAsync(IProductVariantAvailability availability)
    {
        var stock = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>());

        if (availability.IsOnStock.HasValue)
        {
            stock.Properties.Add("isOnStock", new BooleanEnterspeedProperty(availability.IsOnStock.Value));
        }

        if (availability.AvailableQuantity.HasValue)
        {
            stock.Properties.Add("availabiltyQuantity", new NumberEnterspeedProperty(availability.AvailableQuantity.Value));
        }

        if (availability.RestockableInDays.HasValue)
        {
            stock.Properties.Add("restockableInDays", new NumberEnterspeedProperty(availability.RestockableInDays.Value));
        }

        if (availability.Channels != null)
        {
            stock.Properties.Add("channels", MapChannelAvailability(availability.Channels));
        }

        return Task.FromResult<IEnterspeedProperty>(stock);
    }

    private static IEnterspeedProperty MapChannelAvailability(IProductVariantChannelAvailabilityMap availabilityMap)
    {
        var channels = availabilityMap.Select(x =>
        {
            var channel = new ObjectEnterspeedProperty(
                new Dictionary<string, IEnterspeedProperty>
                {
                    ["channelId"] = new StringEnterspeedProperty(x.Key),
                    ["stockId"] = new StringEnterspeedProperty(x.Value.Id),
                    ["isOnStock"] = new BooleanEnterspeedProperty(x.Value.IsOnStock ?? false),
                    ["availableQuantity"] = new NumberEnterspeedProperty(x.Value.AvailableQuantity ?? 0),
                    ["version"] = new NumberEnterspeedProperty(x.Value.Version)
                });

            if (x.Value.RestockableInDays.HasValue)
            {
                channel.Properties.Add("restockableInDays", new NumberEnterspeedProperty(x.Value.RestockableInDays.Value));
            }

            return channel;
        }).ToArray();

        return new ArrayEnterspeedProperty(string.Empty, channels);
    }
}