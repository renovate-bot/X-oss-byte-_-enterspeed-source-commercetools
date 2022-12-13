using commercetools.Sdk.Api.Models.Categories;
using commercetools.Sdk.Api.Models.Common;
using commercetools.Sdk.Api.Models.Types;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Api.Models;
using Enterspeed.Commercetools.Integration.Api.Providers;
using Enterspeed.Commercetools.Integration.Api.Services;
using Enterspeed.Commercetools.Integration.Domain.Extensions;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedCategoryMapper : IMapper<ICategory, EnterspeedCategoryEntity>
{
    private readonly IEnterspeedEntityTypeProvider _typeProvider;
    private readonly IMapper<List<IAsset>, List<IEnterspeedProperty>> _assetMapper;
    private readonly IMapper<ICustomFields, IEnterspeedProperty> _customFieldsMapper;
    private readonly IEnterspeedUrlBuilder _urlBuilder;

    public EnterspeedCategoryMapper(
        IEnterspeedEntityTypeProvider typeProvider,
        IMapper<List<IAsset>,List<IEnterspeedProperty>> assetMapper,
        IMapper<ICustomFields, IEnterspeedProperty> customFieldsMapper,
        IEnterspeedUrlBuilder urlBuilder)
    {
        _typeProvider = typeProvider;
        _assetMapper = assetMapper;
        _customFieldsMapper = customFieldsMapper;
        _urlBuilder = urlBuilder;
    }

    public async Task<EnterspeedCategoryEntity> MapAsync(ICategory source)
    {
        var properties = new Dictionary<string, IEnterspeedProperty>
        {
            ["name"] = source.Name.ToEnterspeedProperty(),
            ["slug"] = source.Slug.ToEnterspeedProperty(),
            ["url"] = (await _urlBuilder.BuildLocalizedUrlsAsync(source, source.Ancestors)).ToEnterspeedProperty(),
            ["ancestors"] = MapAncestors(source.Ancestors),
            ["assets"] = new ArrayEnterspeedProperty(string.Empty, (await _assetMapper.MapAsync(source.Assets)).ToArray())
        };

        if (source.Parent != null)
        {
            properties.Add("parent", source.Parent.ToEnterspeedProperty(source.Parent.Obj?.Key));
        }

        if (source.MetaTitle != null)
        {
            properties.Add("metaTitle", source.MetaTitle.ToEnterspeedProperty());
        }

        if (source.MetaDescription != null)
        {
            properties.Add("metaDescription", source.MetaDescription.ToEnterspeedProperty());
        }

        if (source.MetaKeywords != null)
        {
            properties.Add("metaKeywords", source.MetaKeywords.ToEnterspeedProperty());
        }

        if (source.Description != null)
        {
            properties.Add("description", source.Description.ToEnterspeedProperty());
        }

        if (!string.IsNullOrWhiteSpace(source.Key))
        {
            properties.Add("key", new StringEnterspeedProperty(source.Key));
        }

        if (!string.IsNullOrWhiteSpace(source.ExternalId))
        {
            properties.Add("externalId", new StringEnterspeedProperty(source.ExternalId));
        }

        if (source.Custom != null)
        {
            properties.Add("custom", await _customFieldsMapper.MapAsync(source.Custom));
        }

        var enterspeedModel = new EnterspeedCategoryEntity(source.Id, await _typeProvider.GetEntityTypeAsync(source))
        {
            Properties = properties
        };

        return enterspeedModel;
    }

    private IEnterspeedProperty MapAncestors(List<ICategoryReference> ancestors)
    {
        var list = ancestors.Select(x => x.ToEnterspeedProperty(x.Obj?.Key));
        return new ArrayEnterspeedProperty(string.Empty, list.ToArray());
    }
}