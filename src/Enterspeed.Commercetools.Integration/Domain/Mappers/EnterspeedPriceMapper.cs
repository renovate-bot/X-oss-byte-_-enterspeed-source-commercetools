using commercetools.Sdk.Api.Models.Common;
using commercetools.Sdk.Api.Models.Types;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Domain.Extensions;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedPriceMapper: IMapper<List<IPrice>, IEnterspeedProperty>
{
    private readonly IMapper<ICustomFields, IEnterspeedProperty> _customFieldsMapper;
    private readonly IMapper<ITypedMoney, IEnterspeedProperty> _moneyMapper;
    private readonly IMapper<DateTime, IEnterspeedProperty> _dateTimeMapper;

    public EnterspeedPriceMapper(
        IMapper<ICustomFields, IEnterspeedProperty> customFieldsMapper,
        IMapper<ITypedMoney, IEnterspeedProperty> moneyMapper,
        IMapper<DateTime, IEnterspeedProperty> dateTimeMapper)
    {
        _customFieldsMapper = customFieldsMapper;
        _moneyMapper = moneyMapper;
        _dateTimeMapper = dateTimeMapper;
    }

    public async Task<IEnterspeedProperty> MapAsync(List<IPrice> source)
    {
        var prices = await Task.WhenAll(source.Select(MapPrice));
        return new ArrayEnterspeedProperty(string.Empty, prices);
    }

    private async Task<IEnterspeedProperty> MapPrice(IPrice source)
    {
        var price = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["id"] = new StringEnterspeedProperty(source.Id),
            ["value"] = await _moneyMapper.MapAsync(source.Value)
        });

        if (!string.IsNullOrWhiteSpace(source.Key))
        {
            price.Properties.Add("key", new StringEnterspeedProperty(source.Key));
        }

        if (!string.IsNullOrWhiteSpace(source.Country))
        {
            price.Properties.Add("country", new StringEnterspeedProperty(source.Country));
        }

        if (source.Channel != null)
        {
            price.Properties.Add("channel", source.Channel.ToEnterspeedProperty(source.Channel.Obj?.Key));
        }

        if (source.ValidFrom.HasValue)
        {
            price.Properties.Add("validFrom", await _dateTimeMapper.MapAsync(source.ValidFrom.Value));
        }

        if (source.ValidUntil.HasValue)
        {
            price.Properties.Add("validUntil", await _dateTimeMapper.MapAsync(source.ValidUntil.Value));
        }

        if (source.Tiers != null)
        {

            var tiers = await Task.WhenAll(source.Tiers.Select(async tier => new ObjectEnterspeedProperty(
                new Dictionary<string, IEnterspeedProperty>
                {
                    ["minimumQuantity"] = new NumberEnterspeedProperty(tier.MinimumQuantity),
                    ["value"] = await _moneyMapper.MapAsync(tier.Value)
                })));

            price.Properties.Add("tiers", new ArrayEnterspeedProperty(string.Empty, tiers));
        }

        if (source.Custom != null)
        {
            price.Properties.Add("custom", await _customFieldsMapper.MapAsync(source.Custom));
        }

        return price;
    }
}