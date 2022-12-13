using commercetools.Sdk.Api.Models.Common;
using Enterspeed.Source.Sdk.Api.Models.Properties;

namespace Enterspeed.Commercetools.Integration.Domain.Extensions;

public static class ReferenceExtensions
{
    public static IEnterspeedProperty ToEnterspeedProperty(this IReference reference, string? key)
    {
        var property = new ObjectEnterspeedProperty(new Dictionary<string, IEnterspeedProperty>
        {
            ["id"] = new StringEnterspeedProperty(reference.Id)
        });

        if (!string.IsNullOrWhiteSpace(reference.TypeId.ToString()))
        {
            property.Properties.Add("type", new StringEnterspeedProperty(reference.TypeId.ToString()));
        }

        if (!string.IsNullOrWhiteSpace(key))
        {
            property.Properties.Add("key", new StringEnterspeedProperty(key));
        }

        return property;
    }
}