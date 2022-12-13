using commercetools.Sdk.Api.Models.Common;
using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Providers;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.Domain.Models;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedVariantMapper : IMapper<ProductVariantMappingContext, EnterspeedProductVariantEntity>
{
    private readonly IMapper<IEnumerable<IAttribute>, ObjectEnterspeedProperty> _propertyMapper;
    private readonly IMapper<List<IAsset>, List<IEnterspeedProperty>> _assetMapper;
    private readonly IMapper<IProductVariantAvailability, IEnterspeedProperty> _availabilityMapper;
    private readonly IMapper<List<IPrice>, IEnterspeedProperty> _priceMapper;
    private readonly IProductVariantIdFactory _productVariantIdFactory;
    private readonly IEnterspeedEntityTypeProvider _entityTypeProvider;

    public EnterspeedVariantMapper(
        IMapper<IEnumerable<IAttribute>, ObjectEnterspeedProperty> propertyMapper,
        IMapper<List<IAsset>, List<IEnterspeedProperty>> assetMapper,
        IMapper<IProductVariantAvailability, IEnterspeedProperty> availabilityMapper,
        IMapper<List<IPrice>, IEnterspeedProperty> priceMapper,
        IProductVariantIdFactory productVariantIdFactory,
        IEnterspeedEntityTypeProvider entityTypeProvider)
    {
        _propertyMapper = propertyMapper;
        _assetMapper = assetMapper;
        _priceMapper = priceMapper;
        _productVariantIdFactory = productVariantIdFactory;
        _entityTypeProvider = entityTypeProvider;
        _availabilityMapper = availabilityMapper;
    }

    public async Task<EnterspeedProductVariantEntity> MapAsync(ProductVariantMappingContext context)
    {
        var source = context.Variant;

        var propertiesTask = _propertyMapper.MapAsync(source.Attributes);
        var assetTask = _assetMapper.MapAsync(source.Assets);
        var priceTask = _priceMapper.MapAsync(source.Prices);
        var variantIdTask = _productVariantIdFactory.GetProductVariantIdAsync(context.Product, source);
        var typeTask = _entityTypeProvider.GetEntityTypeAsync(source);

        Task.WaitAll(propertiesTask, assetTask, priceTask, variantIdTask, typeTask);

        var properties = new Dictionary<string, IEnterspeedProperty>()
        {
            ["id"] = new StringEnterspeedProperty(await variantIdTask),
            ["prices"] = await priceTask,
            ["images"] = MapImage(source.Images),
            ["attributes"] = await propertiesTask,
            ["assets"] = new ArrayEnterspeedProperty(string.Empty, (await assetTask).ToArray())
        };

        if (!string.IsNullOrWhiteSpace(source.Key))
        {
            properties.Add("key", new StringEnterspeedProperty(source.Key));
        }

        if (!string.IsNullOrWhiteSpace(source.Sku))
        {
            properties.Add("sku", new StringEnterspeedProperty(source.Sku));
        }

        if (source.Availability != null)
        {
            properties.Add("availability", await _availabilityMapper.MapAsync(source.Availability));
        }
        
        return new EnterspeedProductVariantEntity(await variantIdTask, await typeTask)
        {
            Properties = properties,
            ParentId = context.Product.Id
        };
    }

    private static IEnterspeedProperty MapImage(List<IImage> source)
    {
        var images = source.Select(image => new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["url"] = new StringEnterspeedProperty(image.Url),
            ["label"] = new StringEnterspeedProperty(image.Label),
            ["dimensions"] = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
            {
                ["w"] = new NumberEnterspeedProperty(image.Dimensions.W),
                ["h"] = new NumberEnterspeedProperty(image.Dimensions.H)
            })
        })).ToArray();

        return new ArrayEnterspeedProperty(string.Empty, images);
    }
}