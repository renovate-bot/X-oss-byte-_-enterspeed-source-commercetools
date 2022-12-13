using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Channels;
using commercetools.Sdk.Api.Models.Common;
using commercetools.Sdk.Api.Models.Products;
using commercetools.Sdk.Api.Models.ProductTypes;
using commercetools.Sdk.Api.Models.Stores;
using commercetools.Sdk.Api.Models.TaxCategories;
using commercetools.Sdk.Api.Models.Types;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.Domain.Extensions;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedPropertyMapper :
    IMapper<IEnumerable<IAttribute>, ObjectEnterspeedProperty>,
    IMapper<ICustomFields, IEnterspeedProperty>
{
    private readonly IMapper<ITypedMoney, IEnterspeedProperty> _moneyMapper;
    private readonly IEnterspeedPropertyKeyFactory _propertyKeyFactory;
    private readonly IMapper<DateTime, IEnterspeedProperty> _dateTimeMapper;

    public EnterspeedPropertyMapper(
        IMapper<ITypedMoney, IEnterspeedProperty> moneyMapper,
        IEnterspeedPropertyKeyFactory propertyKeyFactory,
        IMapper<DateTime, IEnterspeedProperty> dateTimeMapper)
    {
        _moneyMapper = moneyMapper;
        _propertyKeyFactory = propertyKeyFactory;
        _dateTimeMapper = dateTimeMapper;
    }

    public async Task<ObjectEnterspeedProperty> MapAsync(IEnumerable<IAttribute> source)
    {
        var properties = new Dictionary<string, IEnterspeedProperty>();

        // Iterate fields
        foreach (var attribute in source)
        {
            var property = await MapValue(attribute.Value);

            // Add if not null
            if (property != null)
            {
                var key = await _propertyKeyFactory.CreatePropertyKeyAsync(attribute.Name);
                properties.Add(key, property);
            }
        }

        return new ObjectEnterspeedProperty(properties);
    }

    public async Task<IEnterspeedProperty> MapAsync(ICustomFields source)
    {
        var properties = new Dictionary<string, IEnterspeedProperty>();
        if (source.Fields == null)
        {
            // No properties
            return new ObjectEnterspeedProperty(properties);
        }

        // Iterate fields
        foreach (var sourceField in source.Fields)
        {
            var property = await MapValue(sourceField.Value);

            // Add if not null
            if (property != null)
            {
                var key = await _propertyKeyFactory.CreatePropertyKeyAsync(sourceField.Key);
                properties.Add(key, property);
            }
        }

        return new ObjectEnterspeedProperty(properties);
    }

    private Task<IEnterspeedProperty?> MapValue(object rawValue)
    {
        return MapValue<object>(rawValue);
    }

    private async Task<IEnterspeedProperty?> MapValue<T>(T rawValue)
    {
        var property = rawValue switch
        {
            // Check if null
            null => null,
            // Check if string
            string value => new StringEnterspeedProperty(value),
            // Check if boolean
            bool value => new BooleanEnterspeedProperty(value),
            // Check if numeric
            short value => new NumberEnterspeedProperty(value),
            int value => new NumberEnterspeedProperty(value),
            long value => new NumberEnterspeedProperty(value),
            // Check if floating-point
            double value => new NumberEnterspeedProperty(value),
            decimal value => new NumberEnterspeedProperty(Convert.ToDouble(value)),
            // Check if date time
            DateTime value => await _dateTimeMapper.MapAsync(value),
            // Check if enum
            IAttributePlainEnumValue value => new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
            {
                ["label"] = new StringEnterspeedProperty(value.Label),
                ["key"] = new StringEnterspeedProperty(value.Key)
            }),
            IAttributeLocalizedEnumValue value => new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
            {
                ["key"] = new StringEnterspeedProperty(value.Key),
                ["label"] = value.Label.ToEnterspeedProperty()
            }),
            // Check if localized string
            ILocalizedString value => value.ToEnterspeedProperty(),
            // Check if money string
            ITypedMoney value => await _moneyMapper.MapAsync(value),
            // Check if reference
            ITaxCategoryReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            IChannelReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            IProductTypeReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            ICategoryReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            IProductReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            IStoreReference value => value.ToEnterspeedProperty(value.Obj?.Key),
            // Check if array of nested types
            IEnumerable<IAttribute> value => await MapAsync(value),
            // Check if array
            IEnumerable<object> value => await MapEnumerable(value),
            // ReSharper disable UseSwitchCasePatternVariable
            // The following cases are not matched by the IEnumerable<object> case,
            // it would result in a empty enumerable collection causing runtime exceptions on iteration
            IEnumerable<long> => await MapEnumerable(rawValue as IEnumerable<long>),
            IEnumerable<int> => await MapEnumerable(rawValue as IEnumerable<int>),
            IEnumerable<bool> => await MapEnumerable(rawValue as IEnumerable<bool>),
            // ReSharper restore UseSwitchCasePatternVariable
            // Default
            _ => null,
        };

        return property;
    }

    private async Task<IEnterspeedProperty> MapEnumerable<T>(IEnumerable<T>? rawValue)
    {
        if (rawValue == null)
        {
            return new ArrayEnterspeedProperty(string.Empty, Array.Empty<IEnterspeedProperty>());
        }

        var results = await Task.WhenAll(rawValue.Select(MapValue));

        var properties = results
            .Where(x => x != null)
            .ToArray() as IEnterspeedProperty[];

        return new ArrayEnterspeedProperty(string.Empty, properties);
    }
}