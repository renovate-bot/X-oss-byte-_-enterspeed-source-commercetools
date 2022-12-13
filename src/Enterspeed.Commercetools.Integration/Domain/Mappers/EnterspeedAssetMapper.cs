using commercetools.Sdk.Api.Models.Common;
using commercetools.Sdk.Api.Models.Types;
using Enterspeed.Commercetools.Integration.Api.Mappers;
using Enterspeed.Commercetools.Integration.Domain.Extensions;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Mappers;

public class EnterspeedAssetMapper : IMapper<List<IAsset>, List<IEnterspeedProperty>>
{
    private readonly IMapper<ICustomFields, IEnterspeedProperty> _customFieldsMapper;

    public EnterspeedAssetMapper(IMapper<ICustomFields, IEnterspeedProperty> customFieldsMapper)
    {
        _customFieldsMapper = customFieldsMapper;
    }

    public async Task<List<IEnterspeedProperty>> MapAsync(List<IAsset> source)
    {
        var result = await Task.WhenAll(source.Select(Map));
        return result.ToList();
    }

    private async Task<IEnterspeedProperty> Map(IAsset source)
    {
        var sources = source.Sources.Select(MapSources).ToArray();
        var tags = source.Tags.Select(x => new StringEnterspeedProperty(x)).ToArray();
        var property = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["id"] = new StringEnterspeedProperty(source.Id), 
            ["sources"] = new ArrayEnterspeedProperty(string.Empty, sources),
            ["name"] = source.Name.ToEnterspeedProperty(),
            ["tags"] = new ArrayEnterspeedProperty(string.Empty, tags)
        });

        if (!string.IsNullOrWhiteSpace(source.Key))
        {
            property.Properties.Add("key", new StringEnterspeedProperty(source.Key));
        }

        if (source.Description != null)
        {
            property.Properties.Add("description", source.Description.ToEnterspeedProperty());
        }

        if (source.Custom != null)
        {
            property.Properties.Add("custom", await _customFieldsMapper.MapAsync(source.Custom));
        }

        return property;
    }

    private static IEnterspeedProperty MapSources(IAssetSource source)
    {
        var property = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["uri"] = new StringEnterspeedProperty(source.Uri)
        });

        if (!string.IsNullOrWhiteSpace(source.Key))
        {
            property.Properties.Add("key", new StringEnterspeedProperty(source.Key));
        }

        if (source.Dimensions != null)
        {
            property.Properties.Add("dimensions", new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
            {
                ["w"] = new NumberEnterspeedProperty(source.Dimensions.W),
                ["h"] = new NumberEnterspeedProperty(source.Dimensions.H),
            }));
        }

        if (!string.IsNullOrWhiteSpace(source.ContentType))
        {
            property.Properties.Add("contentType", new StringEnterspeedProperty(source.ContentType));
        }

        return property;
    }
}