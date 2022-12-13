using commercetools.Sdk.Api.Models.Products;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Providers;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.Domain.Extensions;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedProductMapper : IMapper<IProductProjection, EnterspeedProductEntity>
{
    private readonly IEnterspeedEntityTypeProvider _typeProvider;
    private readonly IEnterspeedUrlBuilder _urlBuilder;
    private readonly IProductVariantIdFactory _productVariantIdFactory;

    public EnterspeedProductMapper(
        IEnterspeedEntityTypeProvider typeProvider,
        IEnterspeedUrlBuilder urlBuilder,
        IProductVariantIdFactory productVariantIdFactory)
    {
        _typeProvider = typeProvider;
        _urlBuilder = urlBuilder;
        _productVariantIdFactory = productVariantIdFactory;
    }

    public async Task<EnterspeedProductEntity> MapAsync(IProductProjection source)
    {
        var entityTypeTask = _typeProvider.GetEntityTypeAsync(source);
        var urlBuilderTask = _urlBuilder.BuildLocalizedUrlsAsync(source);
        var masterVariantIdTask = _productVariantIdFactory.GetProductVariantIdAsync(source, source.MasterVariant);
        var variantIdsTask = new List<IProductVariant>(source.Variants) { source.MasterVariant }
            .Select(x => _productVariantIdFactory.GetProductVariantIdAsync(source, x))
            .ToList();

        await Task.WhenAll(new List<Task>(variantIdsTask)
        {
            entityTypeTask,
            urlBuilderTask,
            masterVariantIdTask
        });

        var variantIds = (await Task.WhenAll(variantIdsTask)).Select(x => new StringEnterspeedProperty(x)).ToArray();

        var enterspeedModel = new EnterspeedProductEntity(source.Id, await entityTypeTask)
        {
            Properties = new Dictionary<string, IEnterspeedProperty>()
            {
                ["key"] = new StringEnterspeedProperty(source.Key),
                ["productType"] = new StringEnterspeedProperty(source.ProductType.Id),
                ["name"] = source.Name.ToEnterspeedProperty(),
                ["categoryIds"] = new ArrayEnterspeedProperty("categoryIds", source.Categories.Select(x => new StringEnterspeedProperty(x.Id)).ToArray()),
                ["slug"] = source.Slug.ToEnterspeedProperty(),
                ["url"] = (await urlBuilderTask).ToEnterspeedProperty(),
                ["masterVariantId"] = new StringEnterspeedProperty(await masterVariantIdTask),
                ["variantIds"] = new ArrayEnterspeedProperty("variantIds", variantIds)
            }
        };

        if (source.Description != null)
        {
            enterspeedModel.Properties.Add("description", source.Description.ToEnterspeedProperty());
        }

        if (source.TaxCategory != null)
        {
            enterspeedModel.Properties.Add("taxCategory", source.TaxCategory.ToEnterspeedProperty(source.TaxCategory.Obj?.Key));
        }

        if (source.MetaTitle != null)
        {
            enterspeedModel.Properties.Add("metaTitle", source.MetaTitle.ToEnterspeedProperty());
        }

        if (source.MetaDescription != null)
        {
            enterspeedModel.Properties.Add("metaDescription", source.MetaDescription.ToEnterspeedProperty());
        }

        if (source.MetaKeywords != null)
        {
            enterspeedModel.Properties.Add("metaKeywords", source.MetaKeywords.ToEnterspeedProperty());
        }

        return enterspeedModel;
    }
}