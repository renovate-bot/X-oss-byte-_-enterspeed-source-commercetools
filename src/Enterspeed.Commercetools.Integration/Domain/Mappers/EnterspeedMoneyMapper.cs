using commercetools.Sdk.Api.Models.Common;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedMoneyMapper : IMapper<ITypedMoney, IEnterspeedProperty>
{
    public Task<IEnterspeedProperty> MapAsync(ITypedMoney source)
    {
        var money = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["currencyCode"] = new StringEnterspeedProperty(source.CurrencyCode),
            ["amount"] = new NumberEnterspeedProperty(Convert.ToDouble(source.AmountToDecimal()))
        });

        return Task.FromResult<IEnterspeedProperty>(money);
    }
}